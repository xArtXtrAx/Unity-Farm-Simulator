using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Extends the generated Decoration palette with outdoor vegetation from
    /// the curated Cozy Farm pilot sheet. The extension is idempotent and is
    /// reapplied after the base catalog recreates its palette prefab.
    /// </summary>
    [InitializeOnLoad]
    public static class CozyFarmVegetationPaletteExtension
    {
        public const string SpringTreeTilePath =
            CozyFarmTileCatalog.DecorationTileRoot + "/Spring Tree.asset";
        public const string BushRowTilePath =
            CozyFarmTileCatalog.DecorationTileRoot + "/Bush Row.asset";
        public const string FlowerCratesTilePath =
            CozyFarmTileCatalog.DecorationTileRoot + "/Flower Crates.asset";

        private const string TileSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png";

        private static bool isApplying;
        private static bool refreshQueued;

        static CozyFarmVegetationPaletteExtension()
        {
            QueueEnsure();
            EditorApplication.projectChanged += QueueEnsure;
        }

        [MenuItem("Tools/Farm Simulator/Refresh Cozy Outdoor Vegetation Palette")]
        public static void RefreshFromMenu()
        {
            EnsureApplied(force: true);
            EditorUtility.DisplayDialog(
                "Cozy outdoor vegetation",
                "Spring tree, bush row and flower crates are available in the Decoration palette.",
                "OK");
        }

        public static void EnsureApplied(bool force = false)
        {
            if (isApplying || EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                QueueEnsure();
                return;
            }

            GameObject palette = AssetDatabase.LoadAssetAtPath<GameObject>(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Decoration));
            if (palette == null)
            {
                return;
            }

            isApplying = true;
            try
            {
                Dictionary<string, Sprite> sprites =
                    AssetDatabase.LoadAllAssetRepresentationsAtPath(TileSheetPath)
                        .OfType<Sprite>()
                        .ToDictionary(sprite => sprite.name, StringComparer.Ordinal);

                Tile tree = CreateOrUpdateTile(
                    SpringTreeTilePath,
                    Required(sprites, "cozy_tree_spring"));
                Tile bushes = CreateOrUpdateTile(
                    BushRowTilePath,
                    Required(sprites, "cozy_bush_row"));
                Tile flowers = CreateOrUpdateTile(
                    FlowerCratesTilePath,
                    Required(sprites, "cozy_flower_crates"));

                string palettePath = CozyFarmTileCatalog.GetPalettePath(
                    CozyPaletteCategory.Decoration);
                GameObject contents = PrefabUtility.LoadPrefabContents(palettePath);
                try
                {
                    Tilemap tilemap = contents.GetComponentInChildren<Tilemap>(true);
                    if (tilemap == null)
                    {
                        throw new InvalidOperationException(
                            "The Decoration palette does not contain a Tilemap.");
                    }

                    bool changed = force;
                    changed |= SetTileIfDifferent(tilemap, new Vector3Int(0, -1, 0), tree);
                    changed |= SetTileIfDifferent(tilemap, new Vector3Int(1, -1, 0), bushes);
                    changed |= SetTileIfDifferent(tilemap, new Vector3Int(2, -1, 0), flowers);

                    if (changed)
                    {
                        PrefabUtility.SaveAsPrefabAsset(contents, palettePath);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(contents);
                }

                AssetDatabase.SaveAssets();
            }
            finally
            {
                isApplying = false;
            }
        }

        private static void QueueEnsure()
        {
            if (refreshQueued)
            {
                return;
            }

            refreshQueued = true;
            EditorApplication.delayCall += () =>
            {
                refreshQueued = false;
                EnsureApplied();
            };
        }

        private static bool SetTileIfDifferent(
            Tilemap tilemap,
            Vector3Int position,
            TileBase tile)
        {
            if (tilemap.GetTile(position) == tile)
            {
                return false;
            }

            tilemap.SetTile(position, tile);
            return true;
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

        private static Sprite Required(
            IReadOnlyDictionary<string, Sprite> sprites,
            string spriteName)
        {
            if (sprites.TryGetValue(spriteName, out Sprite sprite) &&
                sprite != null)
            {
                return sprite;
            }

            throw new InvalidOperationException(
                $"The Cozy Farm tile sheet is missing vegetation sprite '{spriteName}'.");
        }
    }
}
