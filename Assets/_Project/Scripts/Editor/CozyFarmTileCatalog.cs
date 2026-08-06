using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public static class CozyFarmTileCatalog
    {
        public const string CatalogRoot =
            "Assets/_Project/Tiles";
        public const string TileAssetRoot =
            CatalogRoot + "/CozyFarm";
        public const string GroundTileRoot =
            TileAssetRoot + "/Ground";
        public const string CropTileRoot =
            TileAssetRoot + "/Crops";
        public const string PaletteRoot =
            CatalogRoot + "/Palettes";
        public const string PalettePrefabPath =
            PaletteRoot + "/Cozy Farm Starter Palette.prefab";

        public const string GrassTilePath =
            GroundTileRoot + "/Grass.asset";
        public const string DirtTilePath =
            GroundTileRoot + "/Dirt.asset";
        public const string WaterTilePath =
            GroundTileRoot + "/Water.asset";
        public const string TilledSoilTilePath =
            GroundTileRoot + "/Tilled Soil.asset";

        private const string TileSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/" +
            "Pilot/Source/tiles.png";
        private const string CropSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/" +
            "Pilot/Source/crops.png";

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

        [MenuItem(
            "Tools/Farm Simulator/Rebuild Cozy Tile Catalog")]
        public static void RebuildFromMenu()
        {
            Rebuild();
            EditorUtility.DisplayDialog(
                "Cozy Tile Catalog",
                "The starter tile catalog and palette prefab are ready.",
                "OK");
        }

        public static void EnsureAssets()
        {
            if (AssetDatabase.LoadAssetAtPath<Tile>(GrassTilePath) != null &&
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    PalettePrefabPath) != null)
            {
                return;
            }

            Rebuild();
        }

        public static void Rebuild()
        {
            EnsureFolder(GroundTileRoot);
            EnsureFolder(CropTileRoot);
            EnsureFolder(PaletteRoot);

            Dictionary<string, Sprite> tiles =
                LoadRepresentations(TileSheetPath);
            Dictionary<string, Sprite> crops =
                LoadRepresentations(CropSheetPath);

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

            var cropRows = new List<Tile[]>();
            for (int cropIndex = 0;
                 cropIndex < CropPrefixes.Length;
                 cropIndex++)
            {
                string cropFolder =
                    CropTileRoot + "/" + CropFolderNames[cropIndex];
                EnsureFolder(cropFolder);

                var stages = new Tile[6];
                for (int stage = 0; stage < stages.Length; stage++)
                {
                    string spriteName =
                        CropPrefixes[cropIndex] + stage;
                    string tilePath =
                        cropFolder + $"/Stage {stage}.asset";
                    stages[stage] = CreateOrUpdateTile(
                        tilePath,
                        Required(crops, spriteName));
                }

                cropRows.Add(stages);
            }

            BuildPalettePrefab(
                new[] { grass, dirt, water, tilled },
                cropRows);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void BuildPalettePrefab(
            IReadOnlyList<Tile> groundTiles,
            IReadOnlyList<Tile[]> cropRows)
        {
            var root = new GameObject(
                "Cozy Farm Starter Palette",
                typeof(Grid));
            Grid grid = root.GetComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSize = Vector3.one;
            grid.cellGap = Vector3.zero;

            var layerObject = new GameObject(
                "Catalog",
                typeof(Tilemap),
                typeof(TilemapRenderer));
            layerObject.transform.SetParent(root.transform, false);
            Tilemap tilemap = layerObject.GetComponent<Tilemap>();
            TilemapRenderer renderer =
                layerObject.GetComponent<TilemapRenderer>();
            renderer.mode = TilemapRenderer.Mode.Chunk;

            for (int index = 0; index < groundTiles.Count; index++)
            {
                tilemap.SetTile(
                    new Vector3Int(index, 0, 0),
                    groundTiles[index]);
            }

            for (int row = 0; row < cropRows.Count; row++)
            {
                Tile[] stages = cropRows[row];
                for (int stage = 0; stage < stages.Length; stage++)
                {
                    tilemap.SetTile(
                        new Vector3Int(stage, -(row + 2), 0),
                        stages[stage]);
                }
            }

            try
            {
                PrefabUtility.SaveAsPrefabAsset(
                    root,
                    PalettePrefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static Tile CreateOrUpdateTile(
            string assetPath,
            Sprite sprite)
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

        private static Dictionary<string, Sprite> LoadRepresentations(
            string path)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                .OfType<Sprite>()
                .ToDictionary(
                    sprite => sprite.name,
                    StringComparer.Ordinal);
        }

        private static Sprite Required(
            IReadOnlyDictionary<string, Sprite> sprites,
            string name)
        {
            if (sprites.TryGetValue(name, out Sprite sprite) &&
                sprite != null)
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
