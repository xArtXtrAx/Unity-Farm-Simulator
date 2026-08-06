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
                "Use the generated Grid/Tilemap layers as painting targets. " +
                "The starter palette contains ground, farming and crop " +
                "sprites already normalized to the one-unit game grid.",
                MessageType.Info);

            EditorGUILayout.Space();
            if (GUILayout.Button("Rebuild Cozy Tile Catalog"))
            {
                CozyFarmTileCatalog.Rebuild();
                PingPalette();
            }

            if (GUILayout.Button("Open Unity Tile Palette"))
            {
                bool opened = EditorApplication.ExecuteMenuItem(
                    "Window/2D/Tile Palette");
                if (!opened)
                {
                    EditorUtility.DisplayDialog(
                        "Tile Palette",
                        "Unity could not open the Tile Palette window. " +
                        "Install or enable the 2D Tilemap Editor package " +
                        "from Package Manager.",
                        "OK");
                }
            }

            if (GUILayout.Button("Ping Starter Palette Prefab"))
            {
                PingPalette();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Active Farm layer",
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
                "One-time palette setup: open Window > 2D > Tile Palette, " +
                "then drag 'Cozy Farm Starter Palette.prefab' into the " +
                "palette toolbar. Unity converts the prepared Grid prefab " +
                "into a paintable Tile Palette.",
                MessageType.None);
        }

        private static void LayerButton(
            string label,
            System.Func<FarmTilemapLayers, Tilemap> selector)
        {
            if (!GUILayout.Button(label))
            {
                return;
            }

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

            Selection.activeGameObject = tilemap.gameObject;
            EditorGUIUtility.PingObject(tilemap.gameObject);
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
            GameObject palette = AssetDatabase.LoadAssetAtPath<GameObject>(
                CozyFarmTileCatalog.PalettePrefabPath);
            if (palette == null)
            {
                CozyFarmTileCatalog.Rebuild();
                palette = AssetDatabase.LoadAssetAtPath<GameObject>(
                    CozyFarmTileCatalog.PalettePrefabPath);
            }

            Selection.activeObject = palette;
            EditorGUIUtility.PingObject(palette);
        }
    }
}
