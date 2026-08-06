using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Removes the opaque preview-cell background from crop sprites generated
    /// by CozyFarmTileCatalog. The source sheet uses several green shades, so
    /// a single color key is not sufficient. Only colors connected to the
    /// texture perimeter are removed; crop pixels enclosed inside the cell are
    /// preserved.
    /// </summary>
    internal sealed class GeneratedCropTransparencyPostprocessor : AssetPostprocessor
    {
        private const string GeneratedRoot =
            "Assets/_Project/Tiles/Generated/Crops/";

        private void OnPreprocessTexture()
        {
            if (!IsGeneratedCrop(assetPath))
            {
                return;
            }

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.wrapMode = TextureWrapMode.Clamp;
        }

        private void OnPostprocessTexture(Texture2D texture)
        {
            if (!IsGeneratedCrop(assetPath) || texture == null)
            {
                return;
            }

            Color32[] pixels = texture.GetPixels32();
            if (pixels == null || pixels.Length == 0)
            {
                return;
            }

            RemoveConnectedBorderBackground(
                pixels,
                texture.width,
                texture.height);
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
        }

        private static bool IsGeneratedCrop(string path)
        {
            return !string.IsNullOrEmpty(path) &&
                path.StartsWith(GeneratedRoot, StringComparison.Ordinal) &&
                path.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
        }

        private static void RemoveConnectedBorderBackground(
            Color32[] pixels,
            int width,
            int height)
        {
            var borderCounts = new Dictionary<int, int>();
            CountBorderColors(pixels, width, height, borderCounts);

            int borderLength = Math.Max(1, (width + height) * 2 - 4);
            int minimumOccurrences = Math.Max(2, borderLength / 10);
            var backgroundColors = new HashSet<int>();

            foreach (KeyValuePair<int, int> pair in borderCounts)
            {
                if (pair.Value >= minimumOccurrences)
                {
                    backgroundColors.Add(pair.Key);
                }
            }

            // Small sprites may expose the same background in only a few
            // perimeter pixels. Fall back to the most frequent border color.
            if (backgroundColors.Count == 0)
            {
                int dominantColor = 0;
                int dominantCount = -1;
                foreach (KeyValuePair<int, int> pair in borderCounts)
                {
                    if (pair.Value > dominantCount)
                    {
                        dominantColor = pair.Key;
                        dominantCount = pair.Value;
                    }
                }

                if (dominantCount > 0)
                {
                    backgroundColors.Add(dominantColor);
                }
            }

            var visited = new bool[pixels.Length];
            var queue = new Queue<int>();

            for (int x = 0; x < width; x++)
            {
                EnqueueIfBackground(x, 0);
                EnqueueIfBackground(x, height - 1);
            }

            for (int y = 1; y < height - 1; y++)
            {
                EnqueueIfBackground(0, y);
                EnqueueIfBackground(width - 1, y);
            }

            while (queue.Count > 0)
            {
                int index = queue.Dequeue();
                int x = index % width;
                int y = index / width;

                Color32 pixel = pixels[index];
                pixel.a = 0;
                pixels[index] = pixel;

                EnqueueIfBackground(x - 1, y);
                EnqueueIfBackground(x + 1, y);
                EnqueueIfBackground(x, y - 1);
                EnqueueIfBackground(x, y + 1);
            }

            void EnqueueIfBackground(int x, int y)
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    return;
                }

                int index = y * width + x;
                if (visited[index])
                {
                    return;
                }

                visited[index] = true;
                Color32 pixel = pixels[index];
                if (pixel.a == 0 ||
                    backgroundColors.Contains(ToRgbKey(pixel)))
                {
                    queue.Enqueue(index);
                }
            }
        }

        private static void CountBorderColors(
            IReadOnlyList<Color32> pixels,
            int width,
            int height,
            IDictionary<int, int> counts)
        {
            for (int x = 0; x < width; x++)
            {
                Count(x, 0);
                if (height > 1)
                {
                    Count(x, height - 1);
                }
            }

            for (int y = 1; y < height - 1; y++)
            {
                Count(0, y);
                if (width > 1)
                {
                    Count(width - 1, y);
                }
            }

            void Count(int x, int y)
            {
                Color32 pixel = pixels[y * width + x];
                if (pixel.a == 0)
                {
                    return;
                }

                int key = ToRgbKey(pixel);
                counts.TryGetValue(key, out int count);
                counts[key] = count + 1;
            }
        }

        private static int ToRgbKey(Color32 color)
        {
            return color.r | (color.g << 8) | (color.b << 16);
        }
    }
}
