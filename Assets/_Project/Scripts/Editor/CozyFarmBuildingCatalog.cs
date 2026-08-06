using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Generates reusable transparent building sprites from the purchased
    /// Cozy Farm full-version atlas. Every house style owns its extraction and
    /// scene-alignment metadata so scene code does not contain atlas knowledge.
    /// </summary>
    public static class CozyFarmBuildingCatalog
    {
        public sealed class HouseVariant
        {
            public HouseVariant(
                string id,
                string displayName,
                RectInt sourceRect,
                Vector2 doorAnchor,
                Vector2 portalOffset,
                Vector2 spawnOffset,
                Vector2 colliderSize,
                Vector2 colliderOffset,
                float maximumWidth,
                float maximumHeight,
                float baseline,
                Vector2 shadowOffset,
                Vector2 shadowScale,
                int sortingOrder)
            {
                Id = id;
                DisplayName = displayName;
                SourceRect = sourceRect;
                DoorAnchor = doorAnchor;
                PortalOffset = portalOffset;
                SpawnOffset = spawnOffset;
                ColliderSize = colliderSize;
                ColliderOffset = colliderOffset;
                MaximumWidth = maximumWidth;
                MaximumHeight = maximumHeight;
                Baseline = baseline;
                ShadowOffset = shadowOffset;
                ShadowScale = shadowScale;
                SortingOrder = sortingOrder;
            }

            public string Id { get; }
            public string DisplayName { get; }
            public RectInt SourceRect { get; }
            public Vector2 DoorAnchor { get; }
            public Vector2 PortalOffset { get; }
            public Vector2 SpawnOffset { get; }
            public Vector2 ColliderSize { get; }
            public Vector2 ColliderOffset { get; }
            public float MaximumWidth { get; }
            public float MaximumHeight { get; }
            public float Baseline { get; }
            public Vector2 ShadowOffset { get; }
            public Vector2 ShadowScale { get; }
            public int SortingOrder { get; }
            public string GeneratedPath => GeneratedRoot + "/" + Id + ".png";
        }

        public const string SourceAtlasPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Full/Buildings/buildings.png";
        public const string GeneratedRoot =
            "Assets/_Project/Art/Generated/CozyFarm/Buildings";
        public const string DefaultHouseId = "starter-green-gable-house";
        public const string StarterHousePath =
            GeneratedRoot + "/starter-green-gable-house.png";

        private static readonly HouseVariant[] HouseVariants =
        {
            CreateGreenVariant(
                DefaultHouseId,
                "Starter Green Gable House",
                new RectInt(681, 548, 68, 86)),
            CreateGreenVariant(
                "green-gable-house-wide-entry",
                "Green Gable House — Wide Entry",
                new RectInt(761, 548, 68, 86)),
            CreateGreenVariant(
                "green-gable-house-clean",
                "Green Gable House — Clean Roof",
                new RectInt(905, 548, 68, 86)),
            CreateGreenVariant(
                "green-gable-house-autumn",
                "Green Gable House — Autumn Roof",
                new RectInt(1033, 548, 68, 86)),
            CreateGreenVariant(
                "green-gable-house-winter",
                "Green Gable House — Winter",
                new RectInt(1161, 548, 68, 86))
        };

        public static IReadOnlyList<HouseVariant> Houses => HouseVariants;

        // Compatibility members retained for existing tests and callers.
        public static RectInt StarterHouseSource => GetHouse(DefaultHouseId).SourceRect;

        [MenuItem("Tools/Farm Simulator/Generate Cozy Full-Pack Building Sprites")]
        public static void GenerateFromMenu()
        {
            EnsureAssets();
            EditorUtility.DisplayDialog(
                "Cozy Farm buildings",
                $"Generated {HouseVariants.Length} reusable house sprites from the full buildings atlas.",
                "OK");
        }

        public static HouseVariant GetHouse(string id)
        {
            for (int index = 0; index < HouseVariants.Length; index++)
            {
                if (string.Equals(HouseVariants[index].Id, id, StringComparison.Ordinal))
                {
                    return HouseVariants[index];
                }
            }

            throw new ArgumentException($"Unknown Cozy Farm house variant '{id}'.", nameof(id));
        }

        public static Sprite EnsureHouse(string id)
        {
            HouseVariant variant = GetHouse(id);
            EnsureAssets();
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(variant.GeneratedPath);
            if (sprite == null)
            {
                throw new InvalidOperationException(
                    $"The generated Cozy house sprite '{variant.GeneratedPath}' could not be loaded.");
            }

            return sprite;
        }

        public static Sprite EnsureStarterHouse()
        {
            return EnsureHouse(DefaultHouseId);
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
            for (int index = 0; index < HouseVariants.Length; index++)
            {
                HouseVariant variant = HouseVariants[index];
                GenerateSprite(SourceAtlasPath, variant.GeneratedPath, variant.SourceRect);
            }

            AssetDatabase.SaveAssets();
        }

        private static HouseVariant CreateGreenVariant(
            string id,
            string displayName,
            RectInt sourceRect)
        {
            return new HouseVariant(
                id,
                displayName,
                sourceRect,
                doorAnchor: new Vector2(0.5f, 0.08f),
                portalOffset: new Vector2(0f, -1.55f),
                spawnOffset: new Vector2(0f, -2.15f),
                colliderSize: new Vector2(5.35f, 2.45f),
                colliderOffset: new Vector2(0f, -0.25f),
                maximumWidth: 5.8f,
                maximumHeight: 4.45f,
                baseline: -1.62f,
                shadowOffset: new Vector2(0f, -1.67f),
                shadowScale: new Vector2(1.25f, 0.28f),
                sortingOrder: 20);
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
                output.SetPixels32(new Color32[outputWidth * outputHeight]);
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
            importer.userData = "cozy-farm-generated-building-v2";
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
