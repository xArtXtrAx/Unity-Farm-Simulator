using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Aligns existing crop-stage sprites to a shared lower baseline without
    /// changing their artwork, orientation, GUIDs, dimensions, or import paths.
    /// </summary>
    public static class FreeCropBaselineAligner
    {
        private const string OutputRoot =
            "Assets/_Project/Art/Placeholder/Crops";
        private const int Size = 32;
        private const int BottomPadding = 0;
        private const float PixelsPerUnit = 32f;

        [MenuItem(
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Art/" +
            "Align Crop Growth Sprites To Baseline")]
        public static void AlignAll()
        {
            if (!AssetDatabase.IsValidFolder(OutputRoot))
            {
                EditorUtility.DisplayDialog(
                    "Crop baseline alignment",
                    "The crop sprite folder does not exist.",
                    "OK");
                return;
            }

            string[] guids = AssetDatabase.FindAssets(
                "t:Texture2D",
                new[] { OutputRoot });
            int aligned = 0;

            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!IsCropStage(path))
                    {
                        continue;
                    }

                    AlignFile(path);
                    aligned++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (IsCropStage(path))
                {
                    ConfigureSprite(path);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log(
                $"Aligned {aligned} crop growth sprites to a shared bottom baseline: " +
                "32x32 px, centered in X, bottom-aligned in Y, 32 PPU.");
        }

        private static void AlignFile(string assetPath)
        {
            byte[] bytes = File.ReadAllBytes(assetPath);
            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);

            try
            {
                if (!texture.LoadImage(bytes, markNonReadable: false) ||
                    texture.width != Size || texture.height != Size)
                {
                    throw new InvalidDataException(
                        $"'{assetPath}' is not a readable {Size}x{Size} crop sprite.");
                }

                Color32[] source = texture.GetPixels32();
                if (!TryFindOpaqueBounds(source, out RectInt bounds))
                {
                    Debug.LogWarning(
                        $"[Crop Baseline] '{assetPath}' is empty and was not changed.");
                    return;
                }

                Color32[] aligned = ComposeBottomAligned(source, bounds);
                texture.SetPixels32(aligned);
                texture.Apply(false, false);
                File.WriteAllBytes(assetPath, texture.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static bool TryFindOpaqueBounds(
            Color32[] pixels,
            out RectInt bounds)
        {
            int minX = Size;
            int minY = Size;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (pixels[(y * Size) + x].a == 0)
                    {
                        continue;
                    }

                    minX = Mathf.Min(minX, x);
                    minY = Mathf.Min(minY, y);
                    maxX = Mathf.Max(maxX, x);
                    maxY = Mathf.Max(maxY, y);
                }
            }

            if (maxX < minX || maxY < minY)
            {
                bounds = default;
                return false;
            }

            bounds = new RectInt(
                minX,
                minY,
                maxX - minX + 1,
                maxY - minY + 1);
            return true;
        }

        private static Color32[] ComposeBottomAligned(
            Color32[] source,
            RectInt bounds)
        {
            var output = new Color32[Size * Size];
            int targetX = (Size - bounds.width) / 2;
            int targetY = BottomPadding;

            for (int y = 0; y < bounds.height; y++)
            {
                for (int x = 0; x < bounds.width; x++)
                {
                    int sourceX = bounds.x + x;
                    int sourceY = bounds.y + y;
                    int destinationX = targetX + x;
                    int destinationY = targetY + y;

                    if (destinationX < 0 || destinationX >= Size ||
                        destinationY < 0 || destinationY >= Size)
                    {
                        continue;
                    }

                    output[(destinationY * Size) + destinationX] =
                        source[(sourceY * Size) + sourceX];
                }
            }

            return output;
        }

        private static bool IsCropStage(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            return path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                   (name.StartsWith("turnip_stage_", StringComparison.Ordinal) ||
                    name.StartsWith("potato_stage_", StringComparison.Ordinal) ||
                    name.StartsWith("radish_stage_", StringComparison.Ordinal));
        }

        private static void ConfigureSprite(string path)
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return;
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
            importer.textureCompression =
                TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}
