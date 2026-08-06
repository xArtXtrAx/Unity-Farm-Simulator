using System;
using System.Collections.Generic;
using System.IO;
using FarmSimulator.Presentation.Art;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Generates the project's redistributable placeholder art deterministically.
    /// One logical grid cell is 16 pixels and one Unity world unit.
    /// </summary>
    public static class FreePlaceholderArtGenerator
    {
        public const int PixelsPerCell = 16;

        private const string MenuRoot =
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Art/";
        private const string Root = "Assets/_Project/Art/Placeholder";
        private const string SourceRoot = Root + "/Source";
        private const string TileRoot = Root + "/Tiles";
        private const string PrefabRoot = Root + "/Prefabs";

        private static readonly Color32 Transparent = new Color32(0, 0, 0, 0);
        private static readonly Color32 Outline = new Color32(55, 45, 39, 255);
        private static readonly Color32 Grass = new Color32(91, 171, 71, 255);
        private static readonly Color32 GrassDark = new Color32(57, 126, 55, 255);
        private static readonly Color32 Dirt = new Color32(191, 133, 72, 255);
        private static readonly Color32 DirtLight = new Color32(222, 166, 91, 255);
        private static readonly Color32 Soil = new Color32(113, 69, 45, 255);
        private static readonly Color32 SoilWet = new Color32(72, 62, 55, 255);
        private static readonly Color32 Water = new Color32(64, 164, 211, 255);
        private static readonly Color32 WaterLight = new Color32(117, 211, 230, 255);
        private static readonly Color32 Wood = new Color32(126, 76, 43, 255);
        private static readonly Color32 WoodLight = new Color32(194, 128, 62, 255);
        private static readonly Color32 Roof = new Color32(183, 72, 48, 255);
        private static readonly Color32 Wall = new Color32(229, 181, 91, 255);
        private static readonly Color32 Window = new Color32(91, 188, 215, 255);
        private static readonly Color32 Stone = new Color32(126, 126, 132, 255);
        private static readonly Color32 Leaf = new Color32(65, 145, 65, 255);
        private static readonly Color32 LeafLight = new Color32(105, 185, 77, 255);

        [MenuItem(MenuRoot + "Generate Missing Assets")]
        public static void GenerateMissingAssets()
        {
            Generate(false);
        }

        [MenuItem(MenuRoot + "Rebuild All Assets")]
        public static void RebuildAllAssets()
        {
            if (!EditorUtility.DisplayDialog(
                    "Rebuild free placeholder art",
                    "This replaces every generated placeholder PNG, Tile and prefab. " +
                    "Scene references remain stable because existing asset paths are reused.",
                    "Rebuild",
                    "Cancel"))
            {
                return;
            }

            Generate(true);
        }

        private static void Generate(bool replace)
        {
            EnsureFolders();

            var generated = new List<string>();
            var skipped = new List<string>();

            GenerateTile("ground_grass", "tile.ground.grass", DrawGrass, replace, generated, skipped);
            GenerateTile("path_dirt", "tile.path.dirt", DrawDirt, replace, generated, skipped);
            GenerateTile("soil_tilled", "tile.soil.tilled", DrawTilledSoil, replace, generated, skipped);
            GenerateTile("soil_wet", "tile.soil.wet", DrawWetSoil, replace, generated, skipped);
            GenerateTile("water_basic", "tile.water.basic", DrawWater, replace, generated, skipped);

            GeneratePrefab(
                "house_small_4x5",
                "building.house.small.4x5",
                4,
                5,
                DrawHouse,
                new Vector2(0.5f, 0f),
                new Vector2(3.8f, 1.8f),
                new Vector2(0f, 0.9f),
                replace,
                generated,
                skipped);

            GeneratePrefab(
                "bed_single",
                "furniture.bed.single",
                1,
                2,
                DrawBed,
                new Vector2(0.5f, 0f),
                new Vector2(0.9f, 1.8f),
                new Vector2(0f, 0.9f),
                replace,
                generated,
                skipped);

            GeneratePrefab(
                "tree_small",
                "decoration.tree.small",
                2,
                3,
                DrawTree,
                new Vector2(0.5f, 0f),
                new Vector2(1.1f, 0.7f),
                new Vector2(0f, 0.35f),
                replace,
                generated,
                skipped);

            GeneratePrefab(
                "fence_wood",
                "decoration.fence.wood",
                1,
                1,
                DrawFence,
                new Vector2(0.5f, 0f),
                new Vector2(1f, 0.65f),
                new Vector2(0f, 0.33f),
                replace,
                generated,
                skipped);

            GeneratePrefab(
                "rock_small",
                "decoration.rock.small",
                1,
                1,
                DrawRock,
                new Vector2(0.5f, 0f),
                new Vector2(0.85f, 0.55f),
                new Vector2(0f, 0.28f),
                replace,
                generated,
                skipped);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string message =
                $"Generated or rebuilt: {generated.Count}\n" +
                $"Preserved existing: {skipped.Count}\n\n" +
                $"Root: {Root}\n" +
                $"Scale: {PixelsPerCell} pixels = 1 grid cell = 1 Unity unit.";
            Debug.Log("[Free Placeholder Art] " + message.Replace("\n", " | "));
            EditorUtility.DisplayDialog("Free placeholder art", message, "OK");
        }

        private static void GenerateTile(
            string fileName,
            string semanticKey,
            Action<Color32[], int, int> painter,
            bool replace,
            ICollection<string> generated,
            ICollection<string> skipped)
        {
            string pngPath = $"{SourceRoot}/{fileName}.png";
            string tilePath = $"{TileRoot}/{fileName}.asset";

            if (!replace && File.Exists(pngPath) && AssetDatabase.LoadAssetAtPath<Tile>(tilePath) != null)
            {
                skipped.Add(tilePath);
                return;
            }

            WriteTexture(pngPath, PixelsPerCell, PixelsPerCell, painter, new Vector2(0.5f, 0.5f));
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
            if (sprite == null)
            {
                throw new InvalidOperationException($"Could not import sprite '{pngPath}'.");
            }

            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tilePath);
            }

            tile.name = semanticKey;
            tile.sprite = sprite;
            tile.color = Color.white;
            tile.colliderType = Tile.ColliderType.None;
            EditorUtility.SetDirty(tile);
            generated.Add(tilePath);
        }

        private static void GeneratePrefab(
            string fileName,
            string semanticKey,
            int widthCells,
            int heightCells,
            Action<Color32[], int, int> painter,
            Vector2 pivot,
            Vector2 colliderSize,
            Vector2 colliderOffset,
            bool replace,
            ICollection<string> generated,
            ICollection<string> skipped)
        {
            string pngPath = $"{SourceRoot}/{fileName}.png";
            string prefabPath = $"{PrefabRoot}/{fileName}.prefab";

            if (!replace && File.Exists(pngPath) && AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                skipped.Add(prefabPath);
                return;
            }

            WriteTexture(
                pngPath,
                widthCells * PixelsPerCell,
                heightCells * PixelsPerCell,
                painter,
                pivot);

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
            if (sprite == null)
            {
                throw new InvalidOperationException($"Could not import sprite '{pngPath}'.");
            }

            GameObject root = new GameObject(fileName);
            try
            {
                PlaceholderAssetIdentity identity = root.AddComponent<PlaceholderAssetIdentity>();
                identity.Configure(semanticKey, new Vector2Int(widthCells, heightCells));

                SpriteRenderer renderer = root.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = 20;

                BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
                collider.size = colliderSize;
                collider.offset = colliderOffset;

                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            generated.Add(prefabPath);
        }

        private static void WriteTexture(
            string assetPath,
            int width,
            int height,
            Action<Color32[], int, int> painter,
            Vector2 pivot)
        {
            var pixels = new Color32[width * height];
            for (int index = 0; index < pixels.Length; index++)
            {
                pixels[index] = Transparent;
            }

            painter(pixels, width, height);

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels32(pixels);
            texture.Apply(false, false);

            string absolute = Path.GetFullPath(assetPath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolute) ?? Root);
            File.WriteAllBytes(absolute, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException($"Texture importer unavailable for '{assetPath}'.");
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerCell;
            importer.spriteAlignment = (int)SpriteAlignment.Custom;
            importer.spritePivot = pivot;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_Project/Art", "Placeholder");
            EnsureFolder(Root, "Source");
            EnsureFolder(Root, "Tiles");
            EnsureFolder(Root, "Prefabs");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void DrawGrass(Color32[] p, int w, int h)
        {
            Fill(p, w, 0, 0, w, h, Grass);
            Border(p, w, h, GrassDark);
            Set(p, w, 3, 4, GrassDark); Set(p, w, 11, 3, GrassDark);
            Set(p, w, 7, 11, new Color32(130, 202, 82, 255));
            Set(p, w, 13, 9, GrassDark); Set(p, w, 4, 13, GrassDark);
        }

        private static void DrawDirt(Color32[] p, int w, int h)
        {
            Fill(p, w, 0, 0, w, h, Dirt);
            Border(p, w, h, Wood);
            Fill(p, w, 3, 4, 3, 2, DirtLight);
            Fill(p, w, 10, 10, 2, 2, DirtLight);
            Set(p, w, 8, 3, Wood);
        }

        private static void DrawTilledSoil(Color32[] p, int w, int h)
        {
            Fill(p, w, 0, 0, w, h, Dirt);
            Border(p, w, h, Wood);
            for (int y = 3; y < h - 2; y += 4)
            {
                Fill(p, w, 2, y, w - 4, 2, Soil);
            }
        }

        private static void DrawWetSoil(Color32[] p, int w, int h)
        {
            Fill(p, w, 0, 0, w, h, Soil);
            Border(p, w, h, SoilWet);
            for (int y = 3; y < h - 2; y += 4)
            {
                Fill(p, w, 2, y, w - 4, 2, SoilWet);
                Set(p, w, 4, y + 1, WaterLight);
                Set(p, w, 11, y + 1, WaterLight);
            }
        }

        private static void DrawWater(Color32[] p, int w, int h)
        {
            Fill(p, w, 0, 0, w, h, Water);
            Border(p, w, h, new Color32(38, 111, 170, 255));
            Fill(p, w, 2, 4, 6, 2, WaterLight);
            Fill(p, w, 9, 10, 5, 2, WaterLight);
        }

        private static void DrawHouse(Color32[] p, int w, int h)
        {
            Fill(p, w, 4, 1, w - 8, 5, Stone);
            Fill(p, w, 6, 6, w - 12, 38, Wall);
            Fill(p, w, 2, 39, w - 4, 5, Outline);
            for (int row = 0; row < 25; row++)
            {
                int inset = row;
                Fill(p, w, Mathf.Clamp(inset, 2, w / 2 - 1), 44 + row,
                    Mathf.Max(1, w - Mathf.Clamp(inset, 2, w / 2 - 1) * 2), 1, Roof);
            }
            Fill(p, w, 25, 6, 14, 22, Wood);
            Fill(p, w, 29, 9, 6, 15, WoodLight);
            Fill(p, w, 10, 18, 10, 10, Outline);
            Fill(p, w, 12, 20, 6, 6, Window);
            Fill(p, w, 44, 18, 10, 10, Outline);
            Fill(p, w, 46, 20, 6, 6, Window);
            Fill(p, w, 27, 38, 10, 8, Outline);
            Fill(p, w, 29, 40, 6, 4, Window);
            Fill(p, w, 46, 61, 7, 13, Stone);
            Fill(p, w, 47, 63, 5, 10, new Color32(145, 151, 158, 255));
        }

        private static void DrawBed(Color32[] p, int w, int h)
        {
            Fill(p, w, 2, 1, 12, 30, Outline);
            Fill(p, w, 4, 3, 8, 26, WoodLight);
            Fill(p, w, 4, 12, 8, 14, new Color32(78, 170, 209, 255));
            Fill(p, w, 5, 24, 6, 4, new Color32(235, 235, 222, 255));
            Fill(p, w, 3, 1, 2, 5, Wood);
            Fill(p, w, 11, 1, 2, 5, Wood);
        }

        private static void DrawTree(Color32[] p, int w, int h)
        {
            Fill(p, w, 13, 1, 6, 18, Wood);
            Fill(p, w, 5, 16, 22, 22, Leaf);
            Fill(p, w, 9, 27, 15, 17, LeafLight);
            Fill(p, w, 2, 22, 10, 12, Leaf);
            Fill(p, w, 20, 21, 10, 13, Leaf);
            Set(p, w, 10, 36, new Color32(149, 210, 87, 255));
        }

        private static void DrawFence(Color32[] p, int w, int h)
        {
            Fill(p, w, 1, 1, 3, 14, Wood);
            Fill(p, w, 12, 1, 3, 14, Wood);
            Fill(p, w, 2, 5, 12, 3, WoodLight);
            Fill(p, w, 2, 10, 12, 3, WoodLight);
            Border(p, w, h, Transparent);
        }

        private static void DrawRock(Color32[] p, int w, int h)
        {
            Fill(p, w, 3, 2, 10, 10, Stone);
            Fill(p, w, 5, 10, 6, 4, new Color32(158, 158, 166, 255));
            Fill(p, w, 2, 4, 12, 5, Stone);
            Set(p, w, 5, 11, new Color32(196, 196, 202, 255));
        }

        private static void Fill(Color32[] p, int w, int x, int y, int width, int height, Color32 color)
        {
            int textureHeight = p.Length / w;
            int minX = Mathf.Clamp(x, 0, w);
            int maxX = Mathf.Clamp(x + width, 0, w);
            int minY = Mathf.Clamp(y, 0, textureHeight);
            int maxY = Mathf.Clamp(y + height, 0, textureHeight);
            for (int py = minY; py < maxY; py++)
            {
                for (int px = minX; px < maxX; px++)
                {
                    p[py * w + px] = color;
                }
            }
        }

        private static void Set(Color32[] p, int w, int x, int y, Color32 color)
        {
            int h = p.Length / w;
            if (x >= 0 && x < w && y >= 0 && y < h)
            {
                p[y * w + x] = color;
            }
        }

        private static void Border(Color32[] p, int w, int h, Color32 color)
        {
            Fill(p, w, 0, 0, w, 1, color);
            Fill(p, w, 0, h - 1, w, 1, color);
            Fill(p, w, 0, 0, 1, h, color);
            Fill(p, w, w - 1, 0, 1, h, color);
        }
    }
}
