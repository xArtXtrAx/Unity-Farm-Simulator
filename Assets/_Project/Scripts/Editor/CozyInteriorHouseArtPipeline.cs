using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class CozyInteriorHouseArtPipeline
    {
        public const string Root =
            "Assets/_Project/Art/ThirdParty/CozyInterior/Full";

        public const string WallpapersPath = Root + "/basics/wallpapers.png";
        public const string DoorsPath = Root + "/basics/doors.png";
        public const string BedsPath = Root + "/furniture/beds.png";
        public const string RugsPath = Root + "/basics/rugs.png";

        public const string ImportSignature =
            "cozy-interior-house-art-v1";

        private static readonly SheetDefinition[] Sheets =
        {
            new SheetDefinition(
                WallpapersPath,
                new[]
                {
                    new SpriteDefinition(
                        "cozy_interior_wall_cream",
                        new Rect(112f, 784f, 16f, 16f)),
                    new SpriteDefinition(
                        "cozy_interior_floor_wood",
                        new Rect(112f, 240f, 16f, 16f)),
                }),
            new SheetDefinition(
                DoorsPath,
                new[]
                {
                    new SpriteDefinition(
                        "cozy_interior_door_cream",
                        new Rect(0f, 256f, 48f, 64f),
                        new Vector2(0.5f, 0f)),
                }),
            new SheetDefinition(
                BedsPath,
                new[]
                {
                    new SpriteDefinition(
                        "cozy_interior_bed_cream",
                        new Rect(0f, 720f, 48f, 64f),
                        new Vector2(0.5f, 0.15f)),
                }),
            new SheetDefinition(
                RugsPath,
                new[]
                {
                    new SpriteDefinition(
                        "cozy_interior_rug_warm",
                        new Rect(0f, 240f, 64f, 48f)),
                }),
        };

        static CozyInteriorHouseArtPipeline()
        {
            EditorApplication.delayCall += EnsureAssets;
        }

        [MenuItem("Tools/Farm Simulator/Rebuild Cozy Interior House Art")]
        public static void RebuildAssets()
        {
            foreach (SheetDefinition sheet in Sheets)
            {
                TextureImporter importer =
                    AssetImporter.GetAtPath(sheet.Path) as TextureImporter;
                if (importer != null)
                {
                    importer.userData = string.Empty;
                    importer.SaveAndReimport();
                }
            }

            EditorApplication.delayCall += EnsureAssets;
        }

        public static void EnsureAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureAssets;
                return;
            }

            foreach (SheetDefinition sheet in Sheets)
            {
                if (!Configure(sheet))
                {
                    return;
                }
            }

            EditorApplication.delayCall +=
                CozyInteriorHouseSceneUpgrader.EnsureUpgradedScene;
        }

        public static IReadOnlyDictionary<string, Sprite> LoadSprites()
        {
            var result = new Dictionary<string, Sprite>(
                StringComparer.Ordinal);

            foreach (SheetDefinition sheet in Sheets)
            {
                foreach (Sprite sprite in
                         AssetDatabase.LoadAllAssetRepresentationsAtPath(
                                 sheet.Path)
                             .OfType<Sprite>())
                {
                    result[sprite.name] = sprite;
                }
            }

            return result;
        }

        private static bool Configure(SheetDefinition sheet)
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(sheet.Path) as TextureImporter;
            if (importer == null)
            {
                Debug.LogWarning(
                    $"Cozy Interior house art is waiting for '{sheet.Path}'.");
                return false;
            }

            string sheetSignature =
                ImportSignature + ":" + sheet.Path;
            string[] expected = sheet.Sprites
                .Select(sprite => sprite.Name)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
            string[] imported =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(sheet.Path)
                    .OfType<Sprite>()
                    .Select(sprite => sprite.name)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToArray();

            bool current =
                importer.userData == sheetSignature &&
                importer.textureType == TextureImporterType.Sprite &&
                importer.spriteImportMode == SpriteImportMode.Multiple &&
                Mathf.Approximately(importer.spritePixelsPerUnit, 16f) &&
                importer.filterMode == FilterMode.Point &&
                !importer.mipmapEnabled &&
                imported.SequenceEqual(expected);

            if (current)
            {
                return true;
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
            importer.maxTextureSize = 8192;
            importer.userData = sheetSignature;

#pragma warning disable 0618
            importer.spritesheet = sheet.Sprites
                .Select(sprite => sprite.ToMetadata())
                .ToArray();
#pragma warning restore 0618

            importer.SaveAndReimport();
            return false;
        }

        private sealed class SheetDefinition
        {
            public SheetDefinition(
                string path,
                SpriteDefinition[] sprites)
            {
                Path = path;
                Sprites = sprites;
            }

            public string Path { get; }
            public SpriteDefinition[] Sprites { get; }
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
