using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Repairs the first generated crop sprite batch without creating new artwork.
    /// It flips the decoded images vertically and removes pixels leaked from
    /// neighbouring source-sheet cells while preserving the 32x32 grid contract.
    /// </summary>
    public static class FreeCropArtRepair
    {
        private const string OutputRoot =
            "Assets/_Project/Art/Placeholder/Crops";
        private const int Size = 32;
        private const int HorizontalSafetyMargin = 5;
        private const int BottomSafetyMargin = 3;
        private const float PixelsPerUnit = 32f;

        [MenuItem(
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Art/" +
            "Repair Crop Growth Sprites")]
        public static void Repair()
        {
            if (!AssetDatabase.IsValidFolder(OutputRoot))
            {
                EditorUtility.DisplayDialog(
                    "Crop sprite repair",
                    "The crop sprite folder does not exist yet. Run Generate Crop Growth Sprites first.",
                    "OK");
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { OutputRoot });
            int repaired = 0;

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

                    RepairFile(path);
                    repaired++;
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
                $"Repaired {repaired} crop growth sprites: upright orientation, " +
                "neighbour bleed removed, 32x32 px, 32 PPU, bottom-center pivot.");
        }

        private static void RepairFile(string assetPath)
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
                Color32[] upright = FlipVertically(source);
                ClearSafetyMargins(upright);
                KeepLargestConnectedComponent(upright);

                texture.SetPixels32(upright);
                texture.Apply(false, false);
                File.WriteAllBytes(assetPath, texture.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static Color32[] FlipVertically(Color32[] source)
        {
            var result = new Color32[source.Length];
            for (int y = 0; y < Size; y++)
            {
                int sourceRow = y * Size;
                int targetRow = (Size - 1 - y) * Size;
                Array.Copy(source, sourceRow, result, targetRow, Size);
            }

            return result;
        }

        private static void ClearSafetyMargins(Color32[] pixels)
        {
            for (int y = 0; y < Size; y++)
            {
                for (int x = 0; x < Size; x++)
                {
                    if (x < HorizontalSafetyMargin ||
                        x >= Size - HorizontalSafetyMargin ||
                        y < BottomSafetyMargin)
                    {
                        pixels[(y * Size) + x] = new Color32(0, 0, 0, 0);
                    }
                }
            }
        }

        private static void KeepLargestConnectedComponent(Color32[] pixels)
        {
            bool[] visited = new bool[pixels.Length];
            List<int> largest = null;

            for (int index = 0; index < pixels.Length; index++)
            {
                if (visited[index] || pixels[index].a == 0)
                {
                    continue;
                }

                List<int> component = CollectComponent(index, pixels, visited);
                if (largest == null || component.Count > largest.Count)
                {
                    largest = component;
                }
            }

            if (largest == null)
            {
                return;
            }

            bool[] keep = new bool[pixels.Length];
            foreach (int index in largest)
            {
                keep[index] = true;
            }

            for (int index = 0; index < pixels.Length; index++)
            {
                if (!keep[index])
                {
                    pixels[index] = new Color32(0, 0, 0, 0);
                }
            }
        }

        private static List<int> CollectComponent(
            int start,
            Color32[] pixels,
            bool[] visited)
        {
            var result = new List<int>();
            var pending = new Stack<int>();
            pending.Push(start);
            visited[start] = true;

            while (pending.Count > 0)
            {
                int current = pending.Pop();
                result.Add(current);
                int x = current % Size;
                int y = current / Size;

                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    for (int offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        if (offsetX == 0 && offsetY == 0)
                        {
                            continue;
                        }

                        int nextX = x + offsetX;
                        int nextY = y + offsetY;
                        if (nextX < 0 || nextX >= Size ||
                            nextY < 0 || nextY >= Size)
                        {
                            continue;
                        }

                        int next = (nextY * Size) + nextX;
                        if (visited[next] || pixels[next].a == 0)
                        {
                            continue;
                        }

                        visited[next] = true;
                        pending.Push(next);
                    }
                }
            }

            return result;
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
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
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
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}
