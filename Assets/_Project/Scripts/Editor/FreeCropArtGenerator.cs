using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Materializes the approved free crop artwork as grid-aligned Sprite assets.
    /// Crops remain runtime entities rendered by SpriteRenderer; they are not terrain tiles.
    /// </summary>
    public static class FreeCropArtGenerator
    {
        public const string OutputRoot = "Assets/_Project/Art/Placeholder/Crops";
        private const float PixelsPerUnit = 32f;

        private static readonly CropSheet[] Crops =
        {
__DATA__
        };

        [MenuItem(
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Art/" +
            "Generate Crop Growth Sprites")]
        public static void Generate()
        {
            EnsureFolder("Assets/_Project/Art/Placeholder");
            EnsureFolder(OutputRoot);

            int written = 0;
            foreach (CropSheet crop in Crops)
            {
                for (int stage = 0; stage < crop.StagePngBase64.Length; stage++)
                {
                    string path = $"{OutputRoot}/{crop.Id}_stage_{stage}.png";
                    File.WriteAllBytes(path, Convert.FromBase64String(crop.StagePngBase64[stage]));
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                    ConfigureSprite(path);
                    written++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                $"Generated {written} free crop growth sprites at {OutputRoot}. " +
                "Each sprite is 32x32 px, 32 PPU, bottom-center aligned, and occupies one 1x1 grid cell.");
        }

        private static void ConfigureSprite(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException($"Could not load TextureImporter for '{path}'.");
            }

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.textureType = TextureImporterType.Sprite;
            settings.spriteMode = (int)SpriteImportMode.Single;
            settings.spritePixelsPerUnit = PixelsPerUnit;
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(0.5f, 0f);
            settings.filterMode = FilterMode.Point;
            settings.mipmapEnabled = false;
            settings.wrapMode = TextureWrapMode.Clamp;
            settings.alphaIsTransparency = true;
            importer.SetTextureSettings(settings);
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }
                current = next;
            }
        }

        private sealed class CropSheet
        {
            public CropSheet(string id, string[] stagePngBase64)
            {
                Id = id;
                StagePngBase64 = stagePngBase64;
            }

            public string Id { get; }
            public string[] StagePngBase64 { get; }
        }
    }
}
