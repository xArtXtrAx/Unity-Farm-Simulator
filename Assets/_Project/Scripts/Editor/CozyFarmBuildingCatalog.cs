using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Generates reusable transparent building sprites from the purchased
    /// Cozy Farm full-version atlas. Source rectangles are defined once here
    /// so future house styles can be added without changing scene logic.
    /// </summary>
    public static class CozyFarmBuildingCatalog
    {
        public const string SourceAtlasPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Full/Buildings/buildings.png";
        public const string GeneratedRoot =
            "Assets/_Project/Art/Generated/CozyFarm/Buildings";
        public const string StarterHousePath =
            GeneratedRoot + "/starter-green-gable-house.png";

        // Coordinates use Texture2D's bottom-left origin. This preset selects
        // one complete green-roof cottage from the full 1503x1072 atlas.
        public static readonly RectInt StarterHouseSource =
            new RectInt(768, 609, 76, 73);

        [MenuItem("Tools/Farm Simulator/Generate Cozy Full-Pack Building Sprites")]
        public static void GenerateFromMenu()
        {
            EnsureAssets();
            EditorUtility.DisplayDialog(
                "Cozy Farm buildings",
                "Generated the starter house sprite from the full buildings atlas.",
                "OK");
        }

        public static Sprite EnsureStarterHouse()
        {
            EnsureAssets();
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(StarterHousePath);
            if (sprite == null)
            {
                throw new InvalidOperationException(
                    "The generated Cozy starter house sprite could not be loaded.");
            }

            return sprite;
        }

        public static void EnsureAssets()
        {
            if (!File.Exists(ToAbsolutePath(SourceAtlasPath)))
            {
                throw new FileNotFoundException(
                    "The Cozy Farm full buildings atlas is missing. Import the full pack first.",
                    SourceAtlasPath);
            }

            Directory.CreateDirectory(ToAbsolutePath(GeneratedRoot));
            GenerateSprite(SourceAtlasPath, StarterHousePath, StarterHouseSource);
            AssetDatabase.SaveAssets();
        }

        private static void GenerateSprite(
            string sourcePath,
            string destinationPath,
            RectInt sourceRect)
        {
            byte[] sourceBytes = File.ReadAllBytes(ToAbsolutePath(sourcePath));
            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            try
            {
                if (!source.LoadImage(sourceBytes, false))
                {
                    throw new InvalidDataException(
                        $"Could not decode Cozy Farm atlas '{sourcePath}'.");
                }

                if (sourceRect.xMin < 0 || sourceRect.yMin < 0 ||
                    sourceRect.xMax > source.width || sourceRect.yMax > source.height)
                {
                    throw new InvalidDataException(
                        $"Building source rectangle {sourceRect} is outside atlas " +
                        $"{source.width}x{source.height}.");
                }

                Color32[] pixels = source.GetPixels32();
                RectInt visible = FindVisibleBounds(
                    pixels,
                    source.width,
                    source.height,
                    sourceRect);
                WriteTrimmedPng(source, visible, destinationPath, padding: 2);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(source);
            }

            AssetDatabase.ImportAsset(
                destinationPath,
                ImportAssetOptions.ForceSynchronousImport);
            ConfigureGeneratedSprite(destinationPath);
        }

        private static RectInt FindVisibleBounds(
            Color32[] pixels,
            int width,
            int height,
            RectInt region)
        {
            int minX = region.xMax;
            int minY = region.yMax;
            int maxX = region.xMin - 1;
            int maxY = region.yMin - 1;

            for (int y = region.yMin; y < region.yMax; y++)
            {
                for (int x = region.xMin; x < region.xMax; x++)
                {
                    Color32 pixel = pixels[y * width + x];
                    if (pixel.a <= 8)
                    {
                        continue;
                    }

                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }

            if (maxX < minX || maxY < minY)
            {
                throw new InvalidDataException(
                    $"No visible pixels were found in building region {region}.");
            }

            return new RectInt(
                minX,
                minY,
                maxX - minX + 1,
                maxY - minY + 1);
        }

        private static void WriteTrimmedPng(
            Texture2D source,
            RectInt visible,
            string destinationPath,
            int padding)
        {
            int outputWidth = visible.width + padding * 2;
            int outputHeight = visible.height + padding * 2;
            var output = new Texture2D(
                outputWidth,
                outputHeight,
                TextureFormat.RGBA32,
                false);
            try
            {
                var clear = new Color32[outputWidth * outputHeight];
                output.SetPixels32(clear);
                output.SetPixels(
                    padding,
                    padding,
                    visible.width,
                    visible.height,
                    source.GetPixels(
                        visible.x,
                        visible.y,
                        visible.width,
                        visible.height));
                output.Apply(false, false);
                File.WriteAllBytes(
                    ToAbsolutePath(destinationPath),
                    output.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(output);
            }
        }

        private static void ConfigureGeneratedSprite(string assetPath)
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException(
                    $"Could not configure generated building sprite '{assetPath}'.");
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
            settings.spritePivot = new Vector2(0.5f, 0f);
            settings.spriteGenerateFallbackPhysicsShape = false;
            importer.SetTextureSettings(settings);
            importer.userData = "cozy-farm-generated-building-v1";
            importer.SaveAndReimport();
        }

        private static string ToAbsolutePath(string projectRelativePath)
        {
            string projectRoot = Path.GetFullPath(
                Path.Combine(global::UnityEngine.Application.dataPath, ".."));
            return Path.GetFullPath(Path.Combine(
                projectRoot,
                projectRelativePath.Replace('/', Path.DirectorySeparatorChar)));
        }
    }
}
