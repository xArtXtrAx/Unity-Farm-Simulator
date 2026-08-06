using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public static class CozyInteriorFullPackImporter
    {
        public const string FullAssetRoot =
            "Assets/_Project/Art/ThirdParty/CozyInterior/Full";
        public const string PreviewRoot =
            "LocalAssets/CozyInterior/Previews";
        public const string ManifestAssetPath =
            FullAssetRoot + "/cozy-interior-full-pack-manifest.json";
        public const string ImportSignature =
            "cozy-interior-full-pack-v1";

        public const int ExpectedPngCount = 39;
        public const int ExpectedGifCount = 154;
        public const int ExpectedTextCount = 2;

        private static readonly string[] RequiredEntries =
        {
            "16x16/bathroom16x16.png",
            "16x16/beds16x16.png",
            "basics/doors.png",
            "basics/rugs.png",
            "basics/wallpapers.png",
            "furniture/beds.png",
            "furniture/decorations.png",
            "furniture/kitchens_assembled.png",
            "global.png",
            "info.txt",
            "read me.txt",
        };

        [MenuItem("Tools/Farm Simulator/Import Full Cozy Interior Pack...")]
        public static void ImportFromMenu()
        {
            string archivePath = EditorUtility.OpenFilePanel(
                "Select the purchased Cozy Interior full-version ZIP",
                string.Empty,
                "zip");
            if (!string.IsNullOrWhiteSpace(archivePath))
            {
                ImportArchive(archivePath, true);
            }
        }

        [MenuItem("Tools/Farm Simulator/Validate Full Cozy Interior Pack Import")]
        public static void ValidateImportedPack()
        {
            FullPackManifest manifest = LoadManifest();
            if (manifest == null)
            {
                EditorUtility.DisplayDialog(
                    "Cozy Interior full pack",
                    "The full interior pack has not been imported locally yet.",
                    "OK");
                return;
            }

            int pngCount = CountFiles(FullAssetRoot, "*.png");
            int gifCount = CountFiles(PreviewRoot, "*.gif");
            int textCount = CountFiles(FullAssetRoot, "*.txt");
            bool valid =
                pngCount == manifest.pngCount &&
                gifCount == manifest.gifCount &&
                textCount == manifest.textCount &&
                manifest.pngCount == ExpectedPngCount &&
                manifest.gifCount == ExpectedGifCount &&
                manifest.textCount == ExpectedTextCount;

            EditorUtility.DisplayDialog(
                "Cozy Interior full pack",
                valid
                    ? $"Import validated.\n\n" +
                      $"PNG sheets in Unity: {pngCount}\n" +
                      $"GIF previews outside Assets: {gifCount}\n" +
                      $"Text files: {textCount}\n" +
                      $"Archive SHA-256: {manifest.archiveSha256}"
                    : $"The import is incomplete.\n\n" +
                      $"PNG {pngCount}/{ExpectedPngCount}\n" +
                      $"GIF {gifCount}/{ExpectedGifCount}\n" +
                      $"TXT {textCount}/{ExpectedTextCount}",
                "OK");
        }

        public static FullPackManifest ImportArchive(
            string archivePath,
            bool showCompletionDialog = false)
        {
            if (string.IsNullOrWhiteSpace(archivePath))
            {
                throw new ArgumentException(
                    "An archive path is required.",
                    nameof(archivePath));
            }
            if (!File.Exists(archivePath))
            {
                throw new FileNotFoundException(
                    "The Cozy Interior archive was not found.",
                    archivePath);
            }

            string fullArchivePath = Path.GetFullPath(archivePath);
            string assetRoot = ToAbsoluteProjectPath(FullAssetRoot);
            string previewRoot = ToAbsoluteProjectPath(PreviewRoot);

            ResetDirectory(assetRoot);
            ResetDirectory(previewRoot);

            var pngAssetPaths = new List<string>();
            int pngCount = 0;
            int gifCount = 0;
            int textCount = 0;

            try
            {
                using FileStream stream = File.OpenRead(fullArchivePath);
                using var archive = new ZipArchive(
                    stream,
                    ZipArchiveMode.Read,
                    false);
                PackEntry[] entries = archive.Entries
                    .Where(entry => !string.IsNullOrEmpty(entry.Name))
                    .Select(CreatePackEntry)
                    .Where(entry => entry != null)
                    .ToArray();

                var names = new HashSet<string>(
                    entries.Select(entry => entry.RelativePath),
                    StringComparer.OrdinalIgnoreCase);
                string[] missing = RequiredEntries
                    .Where(required => !names.Contains(required))
                    .ToArray();
                if (missing.Length > 0)
                {
                    throw new InvalidDataException(
                        "The selected ZIP is not the expected Cozy Interior " +
                        "full pack. Missing: " + string.Join(", ", missing));
                }

                AssetDatabase.StartAssetEditing();
                try
                {
                    for (int index = 0; index < entries.Length; index++)
                    {
                        PackEntry entry = entries[index];
                        EditorUtility.DisplayProgressBar(
                            "Importing Cozy Interior full pack",
                            entry.RelativePath,
                            entries.Length == 0
                                ? 1f
                                : (float)index / entries.Length);

                        string root = entry.Kind == EntryKind.Gif
                            ? previewRoot
                            : assetRoot;
                        ExtractEntry(
                            entry.Entry,
                            SafeCombine(root, entry.RelativePath));

                        if (entry.Kind == EntryKind.Png)
                        {
                            pngCount++;
                            pngAssetPaths.Add(
                                FullAssetRoot + "/" + entry.RelativePath);
                        }
                        else if (entry.Kind == EntryKind.Gif)
                        {
                            gifCount++;
                        }
                        else
                        {
                            textCount++;
                        }
                    }
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (pngCount != ExpectedPngCount ||
                gifCount != ExpectedGifCount ||
                textCount != ExpectedTextCount)
            {
                throw new InvalidDataException(
                    "The Cozy Interior archive contents do not match the " +
                    $"reviewed package. PNG {pngCount}/{ExpectedPngCount}; " +
                    $"GIF {gifCount}/{ExpectedGifCount}; " +
                    $"TXT {textCount}/{ExpectedTextCount}.");
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (string assetPath in pngAssetPaths)
            {
                ConfigureTextureImporter(assetPath);
            }

            var manifest = new FullPackManifest
            {
                importSignature = ImportSignature,
                sourceArchiveName = Path.GetFileName(fullArchivePath),
                archiveSha256 = ComputeSha256(fullArchivePath),
                importedUtc = DateTime.UtcNow.ToString("O"),
                pngCount = pngCount,
                gifCount = gifCount,
                textCount = textCount,
                gridSize = 16,
                unityAssetRoot = FullAssetRoot,
                localPreviewRoot = PreviewRoot,
            };
            File.WriteAllText(
                ToAbsoluteProjectPath(ManifestAssetPath),
                JsonUtility.ToJson(manifest, true));
            AssetDatabase.ImportAsset(
                ManifestAssetPath,
                ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();

            Debug.Log(
                $"Imported Cozy Interior full pack locally: {pngCount} PNG, " +
                $"{gifCount} GIF and {textCount} text files.");
            if (showCompletionDialog)
            {
                EditorUtility.DisplayDialog(
                    "Cozy Interior full pack imported",
                    $"PNG sheets available in Unity: {pngCount}\n" +
                    $"GIF previews preserved locally: {gifCount}\n" +
                    $"Text files: {textCount}\n\n" +
                    $"Unity folder: {FullAssetRoot}\n\n" +
                    $"Preview folder: {PreviewRoot}",
                    "OK");
            }

            return manifest;
        }

        public static string NormalizeArchiveEntryPath(string entryPath)
        {
            if (string.IsNullOrWhiteSpace(entryPath))
            {
                return null;
            }

            string normalized = entryPath.Replace('\\', '/').TrimStart('/');
            const string root = "interior full/";
            if (normalized.StartsWith(
                    root,
                    StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(root.Length);
            }
            if (string.IsNullOrWhiteSpace(normalized) ||
                normalized.EndsWith("/", StringComparison.Ordinal))
            {
                return null;
            }

            string[] segments = normalized.Split('/');
            if (segments.Any(segment =>
                    string.IsNullOrWhiteSpace(segment) ||
                    segment == "." ||
                    segment == ".."))
            {
                throw new InvalidDataException(
                    $"Unsafe ZIP entry path: '{entryPath}'.");
            }
            return string.Join("/", segments);
        }

        private static PackEntry CreatePackEntry(ZipArchiveEntry entry)
        {
            string relativePath = NormalizeArchiveEntryPath(entry.FullName);
            if (relativePath == null)
            {
                return null;
            }

            string extension = Path.GetExtension(relativePath);
            EntryKind kind;
            if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase))
            {
                kind = EntryKind.Png;
            }
            else if (extension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
            {
                kind = EntryKind.Gif;
            }
            else if (extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                kind = EntryKind.Text;
            }
            else
            {
                return null;
            }
            return new PackEntry(entry, relativePath, kind);
        }

        private static void ExtractEntry(
            ZipArchiveEntry entry,
            string destinationPath)
        {
            string directory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using Stream input = entry.Open();
            using FileStream output = File.Create(destinationPath);
            input.CopyTo(output);
        }

        private static void ConfigureTextureImporter(string assetPath)
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning(
                    $"Could not configure imported texture '{assetPath}'.");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression =
                TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 8192;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(settings);

            importer.userData = ImportSignature;
            importer.SaveAndReimport();
        }

        private static string SafeCombine(string root, string relativePath)
        {
            string rootPath = Path.GetFullPath(root)
                .TrimEnd(Path.DirectorySeparatorChar) +
                Path.DirectorySeparatorChar;
            string combined = Path.GetFullPath(Path.Combine(
                rootPath,
                relativePath.Replace('/', Path.DirectorySeparatorChar)));
            if (!combined.StartsWith(
                    rootPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    $"ZIP entry escapes the import root: '{relativePath}'.");
            }
            return combined;
        }

        private static void ResetDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }

        private static string ToAbsoluteProjectPath(string relativePath)
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(
                    global::UnityEngine.Application.dataPath,
                    ".."));
            return Path.GetFullPath(Path.Combine(
                projectRoot,
                relativePath.Replace('/', Path.DirectorySeparatorChar)));
        }

        private static int CountFiles(string relativeRoot, string pattern)
        {
            string root = ToAbsoluteProjectPath(relativeRoot);
            return Directory.Exists(root)
                ? Directory.GetFiles(
                    root,
                    pattern,
                    SearchOption.AllDirectories).Length
                : 0;
        }

        private static string ComputeSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using SHA256 sha = SHA256.Create();
            return string.Concat(
                sha.ComputeHash(stream)
                    .Select(value => value.ToString("x2")));
        }

        private static FullPackManifest LoadManifest()
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                ManifestAssetPath);
            return asset == null
                ? null
                : JsonUtility.FromJson<FullPackManifest>(asset.text);
        }

        private enum EntryKind
        {
            Png,
            Gif,
            Text,
        }

        private sealed class PackEntry
        {
            public PackEntry(
                ZipArchiveEntry entry,
                string relativePath,
                EntryKind kind)
            {
                Entry = entry;
                RelativePath = relativePath;
                Kind = kind;
            }

            public ZipArchiveEntry Entry { get; }
            public string RelativePath { get; }
            public EntryKind Kind { get; }
        }

        [Serializable]
        public sealed class FullPackManifest
        {
            public string importSignature;
            public string sourceArchiveName;
            public string archiveSha256;
            public string importedUtc;
            public int pngCount;
            public int gifCount;
            public int textCount;
            public int gridSize;
            public string unityAssetRoot;
            public string localPreviewRoot;
        }
    }
}
