using System;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class CropSpriteImportSettings
    {
        private const string CropRoot =
            "Assets/_Project/Art/Placeholder/Crops";

        private static readonly (string crop, int stages)[] Crops =
        {
            ("turnip", 5),
            ("potato", 6),
            ("radish", 5),
        };

        static CropSpriteImportSettings()
        {
            EditorApplication.delayCall += EnsureAll;
        }

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Normalize Crop Sprite Imports")]
        public static void EnsureAll()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode ||
                EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                return;
            }

            foreach ((string crop, int stages) in Crops)
            {
                for (int stage = 0; stage < stages; stage++)
                {
                    EnsureOne($"{CropRoot}/{crop}_stage_{stage}.png");
                }
            }
        }

        private static void EnsureOne(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException(
                    $"Missing first-party crop sprite importer: '{path}'.");
            }

            bool changed = false;
            changed |= SetIfDifferent(
                importer.spritePixelsPerUnit,
                16f,
                value => importer.spritePixelsPerUnit = value);

            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (importer.filterMode != FilterMode.Point)
            {
                importer.filterMode = FilterMode.Point;
                changed = true;
            }

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (importer.textureCompression != TextureImporterCompression.Uncompressed)
            {
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (!changed)
            {
                return;
            }

            importer.SaveAndReimport();
        }

        private static bool SetIfDifferent(
            float current,
            float expected,
            Action<float> setter)
        {
            if (Mathf.Approximately(current, expected))
            {
                return false;
            }

            setter(expected);
            return true;
        }
    }
}
