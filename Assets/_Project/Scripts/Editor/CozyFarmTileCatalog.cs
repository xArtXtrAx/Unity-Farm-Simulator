using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public enum CozyPaletteCategory
    {
        Ground,
        Paths,
        Soil,
        Crops,
        Decoration,
    }

    public static class CozyFarmTileCatalog
    {
        public const string CatalogRoot = "Assets/_Project/Tiles";
        public const string TileAssetRoot = CatalogRoot + "/CozyFarm";
        public const string GroundTileRoot = TileAssetRoot + "/Ground";
        public const string CropTileRoot = TileAssetRoot + "/Crops";
        public const string DecorationTileRoot = TileAssetRoot + "/Decoration";
        public const string GeneratedCropRoot = CatalogRoot + "/Generated/Crops";
        public const string PaletteRoot = CatalogRoot + "/Palettes";

        public const string GrassTilePath = GroundTileRoot + "/Grass.asset";
        public const string DirtTilePath = GroundTileRoot + "/Dirt.asset";
        public const string WaterTilePath = GroundTileRoot + "/Water.asset";
        public const string TilledSoilTilePath = GroundTileRoot + "/Tilled Soil.asset";

        private const string TileSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png";
        private const string CropSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/crops.png";

        private static readonly string[] CropPrefixes =
        {
            "cozy_turnip_stage_",
            "cozy_carrot_stage_",
            "cozy_cabbage_stage_",
        };

        private static readonly string[] CropFolderNames =
        {
            "Turnip",
            "Carrot",
            "Cabbage",
        };

        [MenuItem("Tools/Farm Simulator/Rebuild Cozy Tile Catalog")]
        public static void RebuildFromMenu()
        {
            Rebuild();
            EditorUtility.DisplayDialog(
                "Cozy Tile Catalog",
                "Five categorized Tile Palettes are ready: Ground, Paths, Soil, Crops and Decoration.",
                "OK");
        }

        public static void EnsureAssets()
        {
            bool ready =
                AssetDatabase.LoadAssetAtPath<Tile>(GrassTilePath) != null &&
                Enum.GetValues(typeof(CozyPaletteCategory))
                    .Cast<CozyPaletteCategory>()
                    .All(category =>
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            GetPalettePath(category)) != null);

            if (!ready)
            {
                Rebuild();
            }
        }

        public static GameObject Rebuild()
        {
            EnsureFolder(GroundTileRoot);
            EnsureFolder(CropTileRoot);
            EnsureFolder(DecorationTileRoot);
            EnsureFolder(GeneratedCropRoot);
            EnsureFolder(PaletteRoot);

            Dictionary<string, Sprite> tiles = LoadRepresentations(TileSheetPath);
            Dictionary<string, Sprite> sourceCrops = LoadRepresentations(CropSheetPath);

            Tile grass = CreateOrUpdateTile(
                GrassTilePath,
                Required(tiles, "cozy_grass"));
            Tile dirt = CreateOrUpdateTile(
                DirtTilePath,
                Required(tiles, "cozy_dirt"));
            Tile water = CreateOrUpdateTile(
                WaterTilePath,
                Required(tiles, "cozy_water"));
            Tile tilled = CreateOrUpdateTile(
                TilledSoilTilePath,
                Required(tiles, "cozy_tilled_soil"));

            Tile bridge = CreateOrUpdateTile(
                GroundTileRoot + "/Wood Bridge.asset",
                Required(tiles, "cozy_bridge_wood"));
            Tile lamp = CreateOrUpdateTile(
                DecorationTileRoot + "/Green Lamp.asset",
                Required(tiles, "cozy_lamp_green"));
            Tile bench = CreateOrUpdateTile(
                DecorationTileRoot + "/Light Bench.asset",
                Required(tiles, "cozy_bench_light"));
            Tile rocks = CreateOrUpdateTile(
                DecorationTileRoot + "/Rock Row.asset",
                Required(tiles, "cozy_rock_row"));
            Tile fence = CreateOrUpdateTile(
                DecorationTileRoot + "/Horizontal Fence.asset",
                Required(tiles, "cozy_fence_horizontal"));

            Dictionary<string, Sprite> transparentCrops =
                GenerateTransparentCropSprites(sourceCrops);

            var cropRows = new List<Tile[]>();
            for (int cropIndex = 0; cropIndex < CropPrefixes.Length; cropIndex++)
            {
                string cropFolder = CropTileRoot + "/" + CropFolderNames[cropIndex];
                EnsureFolder(cropFolder);

                var stages = new Tile[6];
                for (int stage = 0; stage < stages.Length; stage++)
                {
                    string spriteName = CropPrefixes[cropIndex] + stage;
                    stages[stage] = CreateOrUpdateTile(
                        cropFolder + $"/Stage {stage}.asset",
                        Required(transparentCrops, spriteName));
                }

                cropRows.Add(stages);
            }

            AssetDatabase.SaveAssets();

            GameObject groundPalette = CreatePalette(
                CozyPaletteCategory.Ground,
                tilemap => PopulateLinear(tilemap, new[] { grass, water }));
            CreatePalette(
                CozyPaletteCategory.Paths,
                tilemap => PopulateLinear(tilemap, new[] { dirt, bridge }));
            CreatePalette(
                CozyPaletteCategory.Soil,
                tilemap => PopulateLinear(tilemap, new[] { tilled }));
            CreatePalette(
                CozyPaletteCategory.Crops,
                tilemap => PopulateCrops(tilemap, cropRows));
            CreatePalette(
                CozyPaletteCategory.Decoration,
                tilemap => PopulateLinear(tilemap, new[] { lamp, bench, rocks, fence }));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return groundPalette;
        }

        public static GameObject LoadPalette(CozyPaletteCategory category)
        {
            EnsureAssets();
            return AssetDatabase.LoadAssetAtPath<GameObject>(GetPalettePath(category));
        }

        public static string GetPaletteName(CozyPaletteCategory category)
        {
            return $"Cozy Farm - {category}";
        }

        public static string GetPalettePath(CozyPaletteCategory category)
        {
            return PaletteRoot + "/" + GetPaletteName(category) + ".prefab";
        }

        private static Dictionary<string, Sprite> GenerateTransparentCropSprites(
            IReadOnlyDictionary<string, Sprite> sourceSprites)
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(CropSheetPath) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException(
                    "Could not access the Cozy Farm crop texture importer.");
            }

            bool wasReadable = importer.isReadable;
            if (!wasReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            try
            {
                Texture2D source = AssetDatabase.LoadAssetAtPath<Texture2D>(CropSheetPath);
                if (source == null)
                {
                    throw new InvalidOperationException(
                        "Could not load the Cozy Farm crop texture.");
                }

                foreach (string prefix in CropPrefixes)
                {
                    for (int stage = 0; stage < 6; stage++)
                    {
                        string name = prefix + stage;
                        Sprite sprite = Required(sourceSprites, name);
                        Rect rect = sprite.rect;
                        int width = Mathf.RoundToInt(rect.width);
                        int height = Mathf.RoundToInt(rect.height);
                        Color[] pixels = source.GetPixels(
                            Mathf.RoundToInt(rect.x),
                            Mathf.RoundToInt(rect.y),
                            width,
                            height);

                        Color key = pixels[0];
                        bool useColorKey = key.a > 0.99f;
                        if (useColorKey)
                        {
                            for (int index = 0; index < pixels.Length; index++)
                            {
                                if (SameRgb(pixels[index], key))
                                {
                                    pixels[index].a = 0f;
                                }
                            }
                        }

                        pixels = NormalizeToTileCell(pixels, width, height);

                        var generated = new Texture2D(
                            width,
                            height,
                            TextureFormat.RGBA32,
                            false);
                        generated.name = name;
                        generated.filterMode = FilterMode.Point;
                        generated.SetPixels(pixels);
                        generated.Apply(false, false);

                        string path = GeneratedCropRoot + "/" + name + ".png";
                        File.WriteAllBytes(path, generated.EncodeToPNG());
                        UnityEngine.Object.DestroyImmediate(generated);
                    }
                }
            }
            finally
            {
                if (!wasReadable)
                {
                    importer.isReadable = false;
                    importer.SaveAndReimport();
                }
            }

            AssetDatabase.Refresh();

            var result = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            foreach (string prefix in CropPrefixes)
            {
                for (int stage = 0; stage < 6; stage++)
                {
                    string name = prefix + stage;
                    string path = GeneratedCropRoot + "/" + name + ".png";
                    ConfigureGeneratedSprite(path);
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (sprite == null)
                    {
                        throw new InvalidOperationException(
                            $"Could not import generated crop sprite '{name}'.");
                    }

                    result[name] = sprite;
                }
            }

            return result;
        }

        private static Color[] NormalizeToTileCell(
            IReadOnlyList<Color> source,
            int width,
            int height)
        {
            int minX = width;
            int minY = height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (source[(y * width) + x].a <= 0.01f)
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
                return source.ToArray();
            }

            int contentWidth = maxX - minX + 1;
            int contentHeight = maxY - minY + 1;
            int destinationX = (width - contentWidth) / 2;
            int destinationY = contentHeight < height ? 1 : 0;
            destinationY = Mathf.Min(destinationY, height - contentHeight);

            var result = new Color[width * height];
            for (int y = 0; y < contentHeight; y++)
            {
                for (int x = 0; x < contentWidth; x++)
                {
                    int sourceIndex = ((minY + y) * width) + minX + x;
                    int destinationIndex =
                        ((destinationY + y) * width) + destinationX + x;
                    result[destinationIndex] = source[sourceIndex];
                }
            }

            return result;
        }

        private static void ConfigureGeneratedSprite(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16f;

            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spritePivot = new Vector2(0.5f, 0.5f);
            importer.SetTextureSettings(settings);

            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
        }

        private static bool SameRgb(Color left, Color right)
        {
            const float epsilon = 0.002f;
            return Mathf.Abs(left.r - right.r) <= epsilon &&
                Mathf.Abs(left.g - right.g) <= epsilon &&
                Mathf.Abs(left.b - right.b) <= epsilon;
        }

        private static GameObject CreatePalette(
            CozyPaletteCategory category,
            Action<Tilemap> populate)
        {
            return UnityTilePaletteBridge.CreateOrReplacePalette(
                PaletteRoot,
                GetPaletteName(category),
                populate);
        }

        private static void PopulateLinear(Tilemap tilemap, IReadOnlyList<Tile> items)
        {
            for (int index = 0; index < items.Count; index++)
            {
                tilemap.SetTile(new Vector3Int(index, 0, 0), items[index]);
            }
        }

        private static void PopulateCrops(
            Tilemap tilemap,
            IReadOnlyList<Tile[]> cropRows)
        {
            for (int row = 0; row < cropRows.Count; row++)
            {
                Tile[] stages = cropRows[row];
                for (int stage = 0; stage < stages.Length; stage++)
                {
                    tilemap.SetTile(
                        new Vector3Int(stage, -row, 0),
                        stages[stage]);
                }
            }
        }

        private static Tile CreateOrUpdateTile(string assetPath, Sprite sprite)
        {
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                tile.name = Path.GetFileNameWithoutExtension(assetPath);
                AssetDatabase.CreateAsset(tile, assetPath);
            }

            tile.sprite = sprite;
            tile.color = Color.white;
            tile.transform = Matrix4x4.identity;
            tile.flags = TileFlags.LockColor | TileFlags.LockTransform;
            tile.colliderType = Tile.ColliderType.None;
            EditorUtility.SetDirty(tile);
            return tile;
        }

        private static Dictionary<string, Sprite> LoadRepresentations(string path)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                .OfType<Sprite>()
                .ToDictionary(sprite => sprite.name, StringComparer.Ordinal);
        }

        private static Sprite Required(
            IReadOnlyDictionary<string, Sprite> sprites,
            string name)
        {
            if (sprites.TryGetValue(name, out Sprite sprite) && sprite != null)
            {
                return sprite;
            }

            throw new InvalidOperationException(
                $"The Cozy Farm catalog is missing sprite '{name}'.");
        }

        private static void EnsureFolder(string assetPath)
        {
            string normalized = assetPath.Replace('\\', '/');
            string[] parts = normalized.Split('/');
            if (parts.Length == 0 || parts[0] != "Assets")
            {
                throw new ArgumentException(
                    "Asset folders must be inside Assets.",
                    nameof(assetPath));
            }

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
    }
}
