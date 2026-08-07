using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Rebuilds redistributable placeholder surface tiles without dark cell borders.
    /// Existing paths and meta files are preserved, so scene and profile references remain stable.
    /// </summary>
    public static class HomogeneousPlaceholderTileRebuilder
    {
        private const string MenuPath =
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Art/Rebuild Homogeneous Surface Tiles";
        private const string Root = "Assets/_Project/Art/Placeholder";
        private const string SourceRoot = Root + "/Source";
        private const string TileRoot = Root + "/Tiles";
        private const int Size = FreePlaceholderArtGenerator.PixelsPerCell;

        [MenuItem(MenuPath)]
        public static void RebuildWithConfirmation()
        {
            if (!EditorUtility.DisplayDialog(
                    "Rebuild homogeneous surface tiles",
                    "This replaces only the seven free placeholder surface PNGs. " +
                    "Existing asset paths, meta files, GUIDs and scene references are preserved.",
                    "Rebuild surfaces",
                    "Cancel"))
            {
                return;
            }

            try
            {
                Rebuild();
                EditorUtility.DisplayDialog(
                    "Homogeneous surface tiles",
                    "Rebuilt grass, dirt path, tilled soil, wet soil, water, wood floor and cream wall.\n\n" +
                    "Open Farm and HouseInterior to review the result. The scenes do not need to be regenerated.",
                    "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "Surface rebuild failed",
                    exception.Message + "\n\nSee Console for the complete stack trace.",
                    "OK");
            }
        }

        public static void Rebuild()
        {
            EnsureFolder("Assets/_Project/Art", "Placeholder");
            EnsureFolder(Root, "Source");
            EnsureFolder(Root, "Tiles");

            RebuildTile("ground_grass", "tile.ground.grass", DrawGrass);
            RebuildTile("path_dirt", "tile.path.dirt", DrawDirt);
            RebuildTile("soil_tilled", "tile.soil.tilled", DrawTilledSoil);
            RebuildTile("soil_wet", "tile.soil.wet", DrawWetSoil);
            RebuildTile("water_basic", "tile.water.basic", DrawWater);
            RebuildTile("floor_wood", "tile.house.floor_wood", DrawWoodFloor);
            RebuildTile("wall_cream", "tile.house.wall_cream", DrawCreamWall);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SceneView.RepaintAll();
            Debug.Log(
                "[Free Placeholder Art] Rebuilt seven homogeneous surface tiles without cell borders.");
        }

        private static void RebuildTile(
            string fileName,
            string semanticKey,
            Action<Color32[], int, int> painter)
        {
            string pngPath = SourceRoot + "/" + fileName + ".png";
            string tilePath = TileRoot + "/" + fileName + ".asset";

            var pixels = new Color32[Size * Size];
            painter(pixels, Size, Size);

            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            try
            {
                texture.SetPixels32(pixels);
                texture.Apply(false, false);
                string absolutePath = Path.GetFullPath(pngPath);
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath) ?? SourceRoot);
                File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }

            AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceSynchronousImport);
            ConfigureImporter(pngPath);

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
            if (sprite == null)
            {
                throw new InvalidOperationException("Could not import placeholder sprite: " + pngPath);
            }

            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tilePath);
            }

            tile.name = fileName;
            tile.sprite = sprite;
            tile.color = Color.white;
            tile.colliderType = Tile.ColliderType.None;
            EditorUtility.SetDirty(tile);
            AssetDatabase.SetLabels(tile, new[] { "placeholder", semanticKey });
        }

        private static void ConfigureImporter(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException("Texture importer unavailable for: " + path);
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = Size;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(0.5f, 0.5f);
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }

        private static void DrawGrass(Color32[] pixels, int width, int height)
        {
            Color32 baseColor = new Color32(94, 169, 75, 255);
            Color32 shadow = new Color32(81, 155, 68, 255);
            Color32 light = new Color32(111, 184, 82, 255);
            Fill(pixels, baseColor);

            Set(pixels, width, height, 3, 4, shadow);
            Set(pixels, width, height, 11, 3, light);
            Set(pixels, width, height, 7, 11, light);
            Set(pixels, width, height, 13, 9, shadow);
            Set(pixels, width, height, 4, 13, shadow);
        }

        private static void DrawDirt(Color32[] pixels, int width, int height)
        {
            Color32 baseColor = new Color32(190, 132, 76, 255);
            Color32 shadow = new Color32(174, 117, 68, 255);
            Color32 light = new Color32(207, 149, 86, 255);
            Fill(pixels, baseColor);

            Set(pixels, width, height, 3, 5, light);
            Set(pixels, width, height, 4, 5, light);
            Set(pixels, width, height, 10, 11, light);
            Set(pixels, width, height, 12, 7, shadow);
            Set(pixels, width, height, 7, 3, shadow);
        }

        private static void DrawTilledSoil(Color32[] pixels, int width, int height)
        {
            Color32 baseColor = new Color32(145, 91, 57, 255);
            Color32 furrow = new Color32(125, 76, 51, 255);
            Color32 highlight = new Color32(158, 101, 63, 255);
            Fill(pixels, baseColor);

            for (int y = 2; y < height; y += 4)
            {
                FillRow(pixels, width, height, y, furrow);
                if (y + 1 < height)
                {
                    Set(pixels, width, height, 4, y + 1, highlight);
                    Set(pixels, width, height, 11, y + 1, highlight);
                }
            }
        }

        private static void DrawWetSoil(Color32[] pixels, int width, int height)
        {
            Color32 baseColor = new Color32(91, 69, 55, 255);
            Color32 furrow = new Color32(73, 57, 48, 255);
            Color32 reflection = new Color32(116, 96, 76, 255);
            Fill(pixels, baseColor);

            for (int y = 2; y < height; y += 4)
            {
                FillRow(pixels, width, height, y, furrow);
            }

            Set(pixels, width, height, 4, 4, reflection);
            Set(pixels, width, height, 10, 8, reflection);
            Set(pixels, width, height, 6, 13, reflection);
        }

        private static void DrawWater(Color32[] pixels, int width, int height)
        {
            Color32 baseColor = new Color32(67, 163, 207, 255);
            Color32 light = new Color32(92, 185, 218, 255);
            Color32 shadow = new Color32(55, 148, 195, 255);
            Fill(pixels, baseColor);

            for (int x = 2; x < 8; x++)
            {
                Set(pixels, width, height, x, 5, light);
            }
            for (int x = 9; x < 14; x++)
            {
                Set(pixels, width, height, x, 11, light);
            }
            Set(pixels, width, height, 12, 3, shadow);
            Set(pixels, width, height, 4, 13, shadow);
        }

        private static void DrawWoodFloor(Color32[] pixels, int width, int height)
        {
            Color32 baseColor = new Color32(177, 113, 65, 255);
            Color32 seam = new Color32(159, 96, 58, 255);
            Color32 grain = new Color32(194, 130, 73, 255);
            Fill(pixels, baseColor);

            for (int y = 3; y < height; y += 4)
            {
                FillRow(pixels, width, height, y, seam);
            }

            Set(pixels, width, height, 3, 1, grain);
            Set(pixels, width, height, 11, 5, grain);
            Set(pixels, width, height, 6, 9, grain);
            Set(pixels, width, height, 13, 13, grain);
        }

        private static void DrawCreamWall(Color32[] pixels, int width, int height)
        {
            Color32 baseColor = new Color32(230, 207, 158, 255);
            Color32 shadow = new Color32(219, 194, 145, 255);
            Color32 light = new Color32(241, 220, 174, 255);
            Fill(pixels, baseColor);

            Set(pixels, width, height, 4, 10, light);
            Set(pixels, width, height, 10, 5, light);
            Set(pixels, width, height, 6, 3, shadow);
            Set(pixels, width, height, 13, 12, shadow);
        }

        private static void Fill(Color32[] pixels, Color32 color)
        {
            for (int index = 0; index < pixels.Length; index++)
            {
                pixels[index] = color;
            }
        }

        private static void FillRow(
            Color32[] pixels,
            int width,
            int height,
            int y,
            Color32 color)
        {
            if (y < 0 || y >= height)
            {
                return;
            }

            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = color;
            }
        }

        private static void Set(
            Color32[] pixels,
            int width,
            int height,
            int x,
            int y,
            Color32 color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                pixels[y * width + x] = color;
            }
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
