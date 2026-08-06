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
    public static class CozyFarmFullPackImporter
    {
        public const string FullAssetRoot =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Full";

        public const string LocalPackageRoot =
            "LocalAssets/CozyFarm";

        public const string PreviewRoot =
            LocalPackageRoot + "/Previews";

        public const string ManifestAssetPath =
            FullAssetRoot + "/cozy-farm-full-pack-manifest.json";

        public const string ImportSignature =
            "cozy-farm-full-pack-v1";

        private static readonly string[] RequiredEntries =
        {
            "global.png",
            "tiles/tiles.png",
            "Buildings/buildings.png",
            "farming/crops.png",
            "ui/items.png",
            "read me.txt",
        };

        [MenuItem("Tools/Farm Simulator/Import Full Cozy Farm Pack...")]
        public static void ImportFromMenu()
        {
            string archivePath = EditorUtility.OpenFilePanel(
                "Select the purchased Cozy Farm full-version ZIP",
                string.Empty,
                "zip");

            if (string.IsNullOrWhiteSpace(archivePath))
            {
                return;
            }

            ImportArchive(archivePath, showCompletionDialog: true);
        }

        [MenuItem("Tools/Farm Simulator/Validate Full Cozy Farm Pack Import")]
        public static void ValidateImportedPack()
        {
            FullPackManifest manifest = LoadManifest();
            if (manifest == null)
            {
                EditorUtility.DisplayDialog(
                    "Cozy Farm full pack",
                    "The full pack has not been imported locally yet.",
                    "OK");
                return;
            }

            string absoluteAssetRoot = ToAbsoluteProjectPath(FullAssetRoot);
            int importedPngCount = Directory.Exists(absoluteAssetRoot)
                ? Directory.GetFiles(
                    absoluteAssetRoot,
                    "*.png",
                    SearchOption.AllDirectories).Length
                : 0;

            string absolutePreviewRoot = ToAbsoluteProjectPath(PreviewRoot);
            int previewGifCount = Directory.Exists(absolutePreviewRoot)
                ? Directory.GetFiles(
                    absolutePreviewRoot,
                    "*.gif",
                    SearchOption.AllDirectories).Length
                : 0;

            bool valid =
                importedPngCount == manifest.pngCount &&
                previewGifCount == manifest.gifCount &&
                manifest.pngCount > 0;

            EditorUtility.DisplayDialog(
                "Cozy Farm full pack",
                valid
                    ? $"Import validated.\n\n" +
                      $"PNG sheets in Unity: {importedPngCount}\n" +
                      $"GIF previews outside Assets: {previewGifCount}\n" +
                      $"Archive SHA-256: {manifest.archiveSha256}"
                    : $"The import is incomplete.\n\n" +
                      $"Expected PNG: {manifest.pngCount}; found: {importedPngCount}\n" +
                      $"Expected GIF: {manifest.gifCount}; found: {previewGifCount}",
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
                    "The Cozy Farm archive was not found.",
                    archivePath);
            }

            string fullArchivePath = Path.GetFullPath(archivePath);
            string assetRootAbsolute = ToAbsoluteProjectPath(FullAssetRoot);
            string previewRootAbsolute = ToAbsoluteProjectPath(PreviewRoot);

            Directory.CreateDirectory(assetRootAbsolute);
            Directory.CreateDirectory(previewRootAbsolute);

            var importedAssetPaths = new List<string>();
            var discoveredEntries = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            int pngCount = 0;
            int gifCount = 0;
            int textCount = 0;

            try
            {
                using FileStream stream = File.OpenRead(fullArchivePath);
                using var archive = new ZipArchive(
                    stream,
                    ZipArchiveMode.Read,
                    leaveOpen: false);

                IReadOnlyList<PackEntry> entries = archive.Entries
                    .Where(entry => !string.IsNullOrEmpty(entry.Name))
                    .Select(CreatePackEntry)
                    .Where(entry => entry != null)
                    .ToArray();

                foreach (PackEntry entry in entries)
                {
                    discoveredEntries.Add(entry.RelativePath);
                }

                string[] missing = RequiredEntries
                    .Where(required => !discoveredEntries.Contains(required))
                    .ToArray();

                if (missing.Length > 0)
                {
                    throw new InvalidDataException(
                        "The selected ZIP is not the expected Cozy Farm full pack. " +
                        "Missing: " + string.Join(", ", missing));
                }

                AssetDatabase.StartAssetEditing();
                try
                {
                    for (int index = 0; index < entries.Count; index++)
                    {
                        PackEntry entry = entries[index];
                        EditorUtility.DisplayProgressBar(
                            "Importing Cozy Farm full pack",
                            entry.RelativePath,
                            entries.Count == 0
                                ? 1f
                                : (float)index / entries.Count);

                        switch (entry.Kind)
                        {
                            case PackEntryKind.Png:
                            case PackEntryKind.Text:
                            {
                                string destination = SafeCombine(
                                    assetRootAbsolute,
                                    entry.RelativePath);
                                ExtractEntry(entry.Entry, destination);
                                string assetPath =
                                    FullAssetRoot + "/" +
                                    entry.RelativePath.Replace('\\', '/');
                                importedAssetPaths.Add(assetPath);

                                if (entry.Kind == PackEntryKind.Png)
                                {
                                    pngCount++;
                                }
                                else
                                {
                                    textCount++;
                                }

                                break;
                            }
                            case PackEntryKind.Gif:
                            {
                                string destination = SafeCombine(
                                    previewRootAbsolute,
                                    entry.RelativePath);
                                ExtractEntry(entry.Entry, destination);
                                gifCount++;
                                break;
                            }
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

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            foreach (string assetPath in importedAssetPaths
                         .Where(path => path.EndsWith(
                             ".png",
                             StringComparison.OrdinalIgnoreCase)))
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
                unityAssetRoot = FullAssetRoot,
                localPreviewRoot = PreviewRoot,
            };

            string manifestAbsolute =
                ToAbsoluteProjectPath(ManifestAssetPath);
            Directory.CreateDirectory(
                Path.GetDirectoryName(manifestAbsolute) ?? assetRootAbsolute);
            File.WriteAllText(
                manifestAbsolute,
                JsonUtility.ToJson(manifest, prettyPrint: true));
            AssetDatabase.ImportAsset(
                ManifestAssetPath,
                ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();

            Debug.Log(
                $"Imported Cozy Farm full pack locally: " +
                $"{pngCount} PNG sheets, {gifCount} GIF previews and " +
                $"{textCount} text files. Assets are available under " +
                $"'{FullAssetRoot}'.");

            if (showCompletionDialog)
            {
                EditorUtility.DisplayDialog(
                    "Cozy Farm full pack imported",
                    $"PNG sheets available in Unity: {pngCount}\n" +
                    $"GIF previews preserved locally: {gifCount}\n" +
                    $"Text files: {textCount}\n\n" +
                    $"Unity folder:\n{FullAssetRoot}\n\n" +
                    $"Preview folder:\n{PreviewRoot}",
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
            const string rootPrefix = "full version/";
            if (normalized.StartsWith(
                    rootPrefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(rootPrefix.Length);
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
            PackEntryKind kind;
            if (string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase))
            {
                kind = PackEntryKind.Png;
            }
            else if (string.Equals(extension, ".gif", StringComparison.OrdinalIgnoreCase))
            {
                kind = PackEntryKind.Gif;
            }
            else if (string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase))
            {
                kind = PackEntryKind.Text;
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
            using FileStream output = new FileStream(
                destinationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);
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
            importer.maxTextureSize = 4096;
            importer.spriteGenerateFallbackPhysicsShape = false;
            importer.userData = ImportSignature;
            importer.SaveAndReimport();
        }

        private static string SafeCombine(string root, string relativePath)
        {
            string rootFullPath = Path.GetFullPath(root)
                .TrimEnd(Path.DirectorySeparatorChar) +
                Path.DirectorySeparatorChar;
            string combined = Path.GetFullPath(
                Path.Combine(
                    rootFullPath,
                    relativePath.Replace('/', Path.DirectorySeparatorChar)));

            if (!combined.StartsWith(
                    rootFullPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    $"ZIP entry escapes the import root: '{relativePath}'.");
            }

            return combined;
        }

        private static string ToAbsoluteProjectPath(string projectRelativePath)
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(Application.dataPath, ".."));
            return Path.GetFullPath(
                Path.Combine(
                    projectRoot,
                    projectRelativePath.Replace(
                        '/',
                        Path.DirectorySeparatorChar)));
        }

        private static string ComputeSha256(string path)
        {
            using FileStream stream = File.OpenRead(path);
            using SHA256 sha = SHA256.Create();
            return string.Concat(
                sha.ComputeHash(stream).Select(value => value.ToString("x2")));
        }

        private static FullPackManifest LoadManifest()
        {
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(
                ManifestAssetPath);
            return asset == null
                ? null
                : JsonUtility.FromJson<FullPackManifest>(asset.text);
        }

        private enum PackEntryKind
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
                PackEntryKind kind)
            {
                Entry = entry;
                RelativePath = relativePath;
                Kind = kind;
            }

            public ZipArchiveEntry Entry { get; }

            public string RelativePath { get; }

            public PackEntryKind Kind { get; }
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
            public string unityAssetRoot;
            public string localPreviewRoot;
        }
    }
}
