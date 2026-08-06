using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class CozyFarmHouseArtPipeline
    {
        public const string TileSheetAssetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png";

        public const string ImportSignature =
            "cozy-farm-house-art-v2";

        private static readonly SpriteDefinition[] Definitions =
        {
            new SpriteDefinition(
                "cozy_grass",
                new Rect(16f, 768f, 16f, 16f)),
            new SpriteDefinition(
                "cozy_dirt",
                new Rect(96f, 640f, 16f, 16f)),
            new SpriteDefinition(
                "cozy_water",
                new Rect(192f, 576f, 16f, 16f)),
            new SpriteDefinition(
                "cozy_tilled_soil",
                new Rect(128f, 640f, 16f, 16f)),
            new SpriteDefinition(
                "cozy_wood_panel_light",
                new Rect(9f, 107f, 30f, 28f)),
            new SpriteDefinition(
                "cozy_wood_panel_dark",
                new Rect(9f, 59f, 30f, 28f)),
            new SpriteDefinition(
                "cozy_bench_dark",
                new Rect(610f, 368f, 30f, 16f)),
            new SpriteDefinition(
                "cozy_bench_light",
                new Rect(178f, 352f, 30f, 16f)),
            new SpriteDefinition(
                "cozy_flower_crates",
                new Rect(112f, 320f, 30f, 29f),
                new Vector2(0.5f, 0f)),
            new SpriteDefinition(
                "cozy_crates_dark",
                new Rect(544f, 320f, 30f, 16f)),
            new SpriteDefinition(
                "cozy_crates_light",
                new Rect(0f, 320f, 30f, 16f)),
            new SpriteDefinition(
                "cozy_lamp_green",
                new Rect(33f, 0f, 13f, 45f),
                new Vector2(0.5f, 0f)),
            new SpriteDefinition(
                "cozy_fence_horizontal",
                new Rect(116f, 304f, 24f, 13f)),
            new SpriteDefinition(
                "cozy_bridge_wood",
                new Rect(128f, 0f, 96f, 18f)),
            new SpriteDefinition(
                "cozy_bush_row",
                new Rect(320f, 384f, 64f, 24f),
                new Vector2(0.5f, 0f)),
            new SpriteDefinition(
                "cozy_tree_spring",
                new Rect(2f, 706f, 42f, 44f),
                new Vector2(0.5f, 0f)),
            new SpriteDefinition(
                "cozy_rock_row",
                new Rect(368f, 352f, 64f, 14f),
                new Vector2(0.5f, 0f)),
        };

        static CozyFarmHouseArtPipeline()
        {
            EditorApplication.delayCall += EnsureAssets;
        }

        public static IReadOnlyDictionary<string, Rect> CuratedSpriteRects =>
            Definitions.ToDictionary(
                definition => definition.Name,
                definition => definition.Rect,
                StringComparer.Ordinal);

        [MenuItem("Tools/Farm Simulator/Rebuild Cozy Farm House Art")]
        public static void RebuildAssets()
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(TileSheetAssetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError(
                    $"Could not find Cozy Farm tiles at '{TileSheetAssetPath}'.");
                return;
            }

            importer.userData = string.Empty;
            importer.SaveAndReimport();
            EditorApplication.delayCall += EnsureAssets;
        }

        public static void EnsureAssets()
        {
            if (EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureAssets;
                return;
            }

            TextureImporter importer =
                AssetImporter.GetAtPath(TileSheetAssetPath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning(
                    "Cozy Farm house art is waiting for tiles.png.");
                return;
            }

            string[] importedNames =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(
                        TileSheetAssetPath)
                    .OfType<Sprite>()
                    .Select(sprite => sprite.name)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToArray();

            string[] expectedNames = Definitions
                .Select(definition => definition.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();

            bool alreadyConfigured =
                importer.userData == ImportSignature &&
                importer.textureType == TextureImporterType.Sprite &&
                importer.spriteImportMode == SpriteImportMode.Multiple &&
                Mathf.Approximately(importer.spritePixelsPerUnit, 16f) &&
                importer.filterMode == FilterMode.Point &&
                !importer.mipmapEnabled &&
                importer.wrapMode == TextureWrapMode.Clamp &&
                importer.textureCompression ==
                    TextureImporterCompression.Uncompressed &&
                importedNames.SequenceEqual(expectedNames);

            if (alreadyConfigured)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression =
                TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.userData = ImportSignature;

#pragma warning disable 0618
            importer.spritesheet = Definitions
                .Select(definition => definition.ToMetadata())
                .ToArray();
#pragma warning restore 0618

            importer.SaveAndReimport();
            EditorApplication.delayCall +=
                HouseAndSleepScenePipeline.EnsureScenes;

            Debug.Log(
                $"Curated {Definitions.Length} Cozy Farm terrain and cabin sprites.");
        }

        private readonly struct SpriteDefinition
        {
            public SpriteDefinition(
                string name,
                Rect rect,
                Vector2? pivot = null)
            {
                Name = name;
                Rect = rect;
                Pivot = pivot ?? new Vector2(0.5f, 0.5f);
            }

            public string Name { get; }

            public Rect Rect { get; }

            public Vector2 Pivot { get; }

            public SpriteMetaData ToMetadata()
            {
                return new SpriteMetaData
                {
                    name = Name,
                    rect = Rect,
                    alignment = (int)SpriteAlignment.Custom,
                    pivot = Pivot,
                };
            }
        }
    }
}
