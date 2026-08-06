using System.Linq;
using FarmSimulator.Presentation.Farming;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public sealed class FarmTileManagerWindow : EditorWindow
    {
        [MenuItem("Tools/Farm Simulator/Tile Manager")]
        public static void Open()
        {
            GetWindow<FarmTileManagerWindow>("Farm Tile Manager");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Farm Tile Manager", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Use Tilemaps to author terrain and decoration. Crops are runtime entities owned by FarmPlotBehaviour, so they are intentionally not paintable from a Tile Palette.",
                MessageType.Info);

            EditorGUILayout.Space();
            if (GUILayout.Button("Rebuild Cozy Tile Catalog + Palettes"))
            {
                GameObject palette = CozyFarmTileCatalog.Rebuild();
                Selection.activeObject = palette;
                EditorGUIUtility.PingObject(palette);
            }

            if (GUILayout.Button("Open Ground Palette"))
            {
                ActivateLayer(
                    CozyPaletteCategory.Ground,
                    layers => layers.Ground);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Paint category", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                LayerButton(
                    "Ground",
                    CozyPaletteCategory.Ground,
                    layers => layers.Ground);
                LayerButton(
                    "Paths",
                    CozyPaletteCategory.Paths,
                    layers => layers.Paths);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                LayerButton(
                    "Soil",
                    CozyPaletteCategory.Soil,
                    layers => layers.Soil);
                LayerButton(
                    "Decoration",
                    CozyPaletteCategory.Decoration,
                    layers => layers.Decoration);
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Ground: grass/water. Paths: dirt/bridge. Soil: authored tilled ground. Decoration: lamp, bench, rocks and fence. Seeds and growing plants appear only through gameplay.",
                MessageType.None);
        }

        private static void LayerButton(
            string label,
            CozyPaletteCategory category,
            System.Func<FarmTilemapLayers, Tilemap> selector)
        {
            if (GUILayout.Button(label))
            {
                ActivateLayer(category, selector);
            }
        }

        private static void ActivateLayer(
            CozyPaletteCategory category,
            System.Func<FarmTilemapLayers, Tilemap> selector)
        {
            FarmTilemapLayers layers = FindLayersInActiveScene();
            Tilemap tilemap = layers == null ? null : selector(layers);
            if (tilemap == null)
            {
                EditorUtility.DisplayDialog(
                    "Farm Tile Manager",
                    "Open the generated Farm scene before selecting a category.",
                    "OK");
                return;
            }

            GameObject palette = CozyFarmTileCatalog.LoadPalette(category);
            if (!UnityTilePaletteBridge.OpenAndActivate(palette, tilemap))
            {
                EditorUtility.DisplayDialog(
                    "Farm Tile Manager",
                    "Unity could not activate the categorized Tile Palette. Confirm that 2D Tilemap Editor is installed, then rebuild the catalog.",
                    "OK");
            }
        }

        private static FarmTilemapLayers FindLayersInActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            return Resources.FindObjectsOfTypeAll<FarmTilemapLayers>()
                .FirstOrDefault(candidate =>
                    candidate != null && candidate.gameObject.scene == activeScene);
        }
    }
}
