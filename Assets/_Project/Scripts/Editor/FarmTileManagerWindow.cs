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
            EditorGUILayout.LabelField(
                "Farm Tile Manager",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The manager creates and activates a real Unity Tile Palette. " +
                "Open Farm, then choose a layer to paint directly on its Tilemap.",
                MessageType.Info);

            EditorGUILayout.Space();
            if (GUILayout.Button("Rebuild Cozy Tile Catalog + Palette"))
            {
                GameObject palette = CozyFarmTileCatalog.Rebuild();
                Selection.activeObject = palette;
                EditorGUIUtility.PingObject(palette);
            }

            if (GUILayout.Button("Open Palette On Ground Layer"))
            {
                ActivateLayer(layers => layers.Ground);
            }

            if (GUILayout.Button("Ping Generated Palette Asset"))
            {
                PingPalette();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Paint target",
                EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                LayerButton("Ground", layers => layers.Ground);
                LayerButton("Paths", layers => layers.Paths);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                LayerButton("Farming", layers => layers.Farming);
                LayerButton("Decoration", layers => layers.Decoration);
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "No manual prefab conversion is required. Each layer button " +
                "opens Tile Palette, loads 'Cozy Farm Starter Palette' and " +
                "sets the selected Farm Tilemap as the active paint target.",
                MessageType.None);
        }

        private static void LayerButton(
            string label,
            System.Func<FarmTilemapLayers, Tilemap> selector)
        {
            if (GUILayout.Button(label))
            {
                ActivateLayer(selector);
            }
        }

        private static void ActivateLayer(
            System.Func<FarmTilemapLayers, Tilemap> selector)
        {
            FarmTilemapLayers layers = FindLayersInActiveScene();
            Tilemap tilemap = layers == null ? null : selector(layers);
            if (tilemap == null)
            {
                EditorUtility.DisplayDialog(
                    "Farm Tile Manager",
                    "Open the generated Farm scene before selecting a layer.",
                    "OK");
                return;
            }

            GameObject palette = CozyFarmTileCatalog.LoadPalette();
            if (!UnityTilePaletteBridge.OpenAndActivate(
                    palette,
                    tilemap))
            {
                EditorUtility.DisplayDialog(
                    "Farm Tile Manager",
                    "Unity could not activate the Tile Palette. Confirm that " +
                    "2D Tilemap Editor is installed, then rebuild the catalog.",
                    "OK");
            }
        }

        private static FarmTilemapLayers FindLayersInActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            return Resources.FindObjectsOfTypeAll<FarmTilemapLayers>()
                .FirstOrDefault(candidate =>
                    candidate != null &&
                    candidate.gameObject.scene == activeScene);
        }

        private static void PingPalette()
        {
            GameObject palette = CozyFarmTileCatalog.LoadPalette();
            Selection.activeObject = palette;
            EditorGUIUtility.PingObject(palette);
        }
    }
}
