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
        Decoration,
    }

    public static class CozyFarmTileCatalog
    {
        public const string CatalogRoot = "Assets/_Project/Tiles";
        public const string TileAssetRoot = CatalogRoot + "/CozyFarm";
        public const string GroundTileRoot = TileAssetRoot + "/Ground";
        public const string DecorationTileRoot = TileAssetRoot + "/Decoration";
        public const string PaletteRoot = CatalogRoot + "/Palettes";

        public const string GrassTilePath = GroundTileRoot + "/Grass.asset";
        public const string DirtTilePath = GroundTileRoot + "/Dirt.asset";
        public const string WaterTilePath = GroundTileRoot + "/Water.asset";
        public const string TilledSoilTilePath = GroundTileRoot + "/Tilled Soil.asset";

        private const string TileSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png";

        [MenuItem("Tools/Farm Simulator/Rebuild Cozy Tile Catalog")]
        public static void RebuildFromMenu()
        {
            Rebuild();
            EditorUtility.DisplayDialog(
                "Cozy Tile Catalog",
                "Ground, Paths, Soil and Decoration palettes are ready.",
                "OK");
        }

        public static void EnsureAssets()
        {
            bool palettesReady =
                AssetDatabase.LoadAssetAtPath<Tile>(GrassTilePath) != null &&
                Enum.GetValues(typeof(CozyPaletteCategory))
                    .Cast<CozyPaletteCategory>()
                    .All(category =>
                        AssetDatabase.LoadAssetAtPath<GameObject>(
                            GetPalettePath(category)) != null);

            if (!palettesReady)
                Rebuild();
        }

        public static GameObject Rebuild()
        {
            EnsureFolder(GroundTileRoot);
            EnsureFolder(DecorationTileRoot);
            EnsureFolder(PaletteRoot);

            Dictionary<string, Sprite> tiles = LoadRepresentations(TileSheetPath);

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

        public static string GetPaletteName(CozyPaletteCategory category) =>
            $"Cozy Farm - {category}";

        public static string GetPalettePath(CozyPaletteCategory category) =>
            PaletteRoot + "/" + GetPaletteName(category) + ".prefab";

        private static GameObject CreatePalette(
            CozyPaletteCategory category,
            Action<Tilemap> populate) =>
            UnityTilePaletteBridge.CreateOrReplacePalette(
                PaletteRoot,
                GetPaletteName(category),
                populate);

        private static void PopulateLinear(Tilemap tilemap, IReadOnlyList<Tile> items)
        {
            for (int index = 0; index < items.Count; index++)
                tilemap.SetTile(new Vector3Int(index, 0, 0), items[index]);
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

        private static Dictionary<string, Sprite> LoadRepresentations(string path) =>
            AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                .OfType<Sprite>()
                .ToDictionary(sprite => sprite.name, StringComparer.Ordinal);

        private static Sprite Required(
            IReadOnlyDictionary<string, Sprite> sprites,
            string name)
        {
            if (sprites.TryGetValue(name, out Sprite sprite) && sprite != null)
                return sprite;

            throw new InvalidOperationException(
                $"The Cozy Farm catalog is missing sprite '{name}'.");
        }

        private static void EnsureFolder(string assetPath)
        {
            string[] parts = assetPath.Replace('\\', '/').Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[index]);

                current = next;
            }
        }
    }
}
