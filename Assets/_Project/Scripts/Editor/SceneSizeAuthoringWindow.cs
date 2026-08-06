using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public sealed class SceneSizeAuthoringWindow : EditorWindow
    {
        private const string FarmScenePath = "Assets/_Project/Scenes/Farm.unity";
        private const string InteriorScenePath = "Assets/_Project/Scenes/HouseInterior.unity";
        private const string BoundsObjectName = "Scene Authoring Bounds";

        private enum SceneKind
        {
            Farm,
            HouseInterior,
        }

        private SceneKind sceneKind;
        private int width = 24;
        private int height = 18;
        private Vector2Int centreCell = Vector2Int.zero;
        private bool fillMissingGround = true;
        private bool clearTilesOutsideBounds;

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Scene Size")]
        public static void Open()
        {
            var window = GetWindow<SceneSizeAuthoringWindow>();
            window.titleContent = new GUIContent("Scene Size");
            window.minSize = new Vector2(430f, 330f);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "Farm Development Kit — Scene Size",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Define the logical width and height of Farm or House Interior in grid cells. " +
                "The tool updates the shared authoring bounds and, for Farm, can extend the Ground Tilemap without rebuilding the scene.",
                MessageType.Info);

            SceneKind nextKind = (SceneKind)GUILayout.Toolbar(
                (int)sceneKind,
                new[] { "Farm", "Interior de la casa" });
            if (nextKind != sceneKind)
            {
                sceneKind = nextKind;
                LoadCurrentSize();
            }

            EditorGUILayout.Space();
            width = Mathf.Clamp(EditorGUILayout.IntField("Width (cells)", width), 4, 512);
            height = Mathf.Clamp(EditorGUILayout.IntField("Height (cells)", height), 4, 512);
            centreCell = EditorGUILayout.Vector2IntField("Centre cell", centreCell);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tile handling", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(sceneKind != SceneKind.Farm))
            {
                fillMissingGround = EditorGUILayout.ToggleLeft(
                    "Fill missing Farm ground cells with grass",
                    fillMissingGround);
                clearTilesOutsideBounds = EditorGUILayout.ToggleLeft(
                    "Clear Farm tiles outside the new bounds",
                    clearTilesOutsideBounds);
            }

            if (clearTilesOutsideBounds)
            {
                EditorGUILayout.HelpBox(
                    "Clearing outside bounds is destructive for painted Tilemaps. Leave it disabled while expanding or testing a size.",
                    MessageType.Warning);
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Read current size", GUILayout.Height(30f)))
                {
                    LoadCurrentSize();
                }

                if (GUILayout.Button("Apply + save scene", GUILayout.Height(30f)))
                {
                    ApplySize();
                }
            }

            EditorGUILayout.Space();
            RectInt cells = CalculateCellBounds();
            EditorGUILayout.LabelField(
                $"Cells: X {cells.xMin}…{cells.xMax - 1}, Y {cells.yMin}…{cells.yMax - 1}",
                EditorStyles.miniLabel);
            EditorGUILayout.LabelField(
                $"Total cells: {cells.width * cells.height:N0}",
                EditorStyles.miniLabel);
        }

        private void LoadCurrentSize()
        {
            string path = ScenePath;
            Scene scene = SceneManager.GetSceneByPath(path);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }

            try
            {
                BoxCollider2D bounds = FindBounds(scene);
                Grid grid = FindGrid(scene);
                if (bounds == null || grid == null)
                {
                    return;
                }

                Vector3 minWorld = bounds.bounds.min;
                Vector3 maxWorld = bounds.bounds.max;
                Vector3Int min = grid.WorldToCell(minWorld + new Vector3(0.001f, 0.001f));
                Vector3Int max = grid.WorldToCell(maxWorld - new Vector3(0.001f, 0.001f));
                width = Mathf.Max(4, max.x - min.x + 1);
                height = Mathf.Max(4, max.y - min.y + 1);
                centreCell = new Vector2Int(
                    Mathf.FloorToInt((min.x + max.x) * 0.5f),
                    Mathf.FloorToInt((min.y + max.y) * 0.5f));
                Repaint();
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private void ApplySize()
        {
            string path = ScenePath;
            Scene scene = SceneManager.GetSceneByPath(path);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }

            try
            {
                Grid grid = FindGrid(scene);
                if (grid == null)
                {
                    throw new InvalidOperationException(
                        $"Scene '{scene.name}' does not contain a Grid component.");
                }

                RectInt cellBounds = CalculateCellBounds();
                BoxCollider2D bounds = GetOrCreateBounds(scene, grid);
                ApplyBoundsCollider(bounds, grid, cellBounds);

                if (sceneKind == SceneKind.Farm)
                {
                    ResizeFarmTilemaps(scene, cellBounds);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene, path))
                {
                    throw new InvalidOperationException($"Could not save scene '{path}'.");
                }

                Selection.activeGameObject = bounds.gameObject;
                EditorGUIUtility.PingObject(bounds.gameObject);
                SceneView.RepaintAll();
                Debug.Log(
                    $"Applied {sceneKind} scene size {width} x {height} cells at centre {centreCell}.");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Scene Size", exception.Message, "OK");
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private void ResizeFarmTilemaps(Scene scene, RectInt bounds)
        {
            Tilemap[] tilemaps = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Tilemap>(true))
                .ToArray();

            if (clearTilesOutsideBounds)
            {
                foreach (Tilemap tilemap in tilemaps)
                {
                    BoundsInt occupied = tilemap.cellBounds;
                    foreach (Vector3Int position in occupied.allPositionsWithin)
                    {
                        if (tilemap.HasTile(position) && !bounds.Contains(new Vector2Int(position.x, position.y)))
                        {
                            tilemap.SetTile(position, null);
                        }
                    }
                    tilemap.CompressBounds();
                    EditorUtility.SetDirty(tilemap);
                }
            }

            if (!fillMissingGround)
            {
                return;
            }

            Tilemap ground = tilemaps.FirstOrDefault(tilemap =>
                string.Equals(tilemap.name, "Ground", StringComparison.OrdinalIgnoreCase));
            TileBase grass = AssetDatabase.LoadAssetAtPath<TileBase>(CozyFarmTileCatalog.GrassTilePath);
            if (ground == null || grass == null)
            {
                throw new InvalidOperationException(
                    "Farm ground could not be extended because Ground or the grass Tile asset is missing.");
            }

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    if (!ground.HasTile(cell))
                    {
                        ground.SetTile(cell, grass);
                    }
                }
            }

            ground.CompressBounds();
            EditorUtility.SetDirty(ground);
        }

        private static void ApplyBoundsCollider(BoxCollider2D collider, Grid grid, RectInt cells)
        {
            Vector3 minimum = grid.CellToWorld(new Vector3Int(cells.xMin, cells.yMin, 0));
            Vector3 maximum = grid.CellToWorld(new Vector3Int(cells.xMax, cells.yMax, 0));
            Vector3 centre = (minimum + maximum) * 0.5f;
            Vector3 size = maximum - minimum;

            collider.transform.position = new Vector3(centre.x, centre.y, collider.transform.position.z);
            collider.offset = Vector2.zero;
            collider.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
            collider.isTrigger = true;
            EditorUtility.SetDirty(collider);
        }

        private static BoxCollider2D GetOrCreateBounds(Scene scene, Grid grid)
        {
            BoxCollider2D existing = FindBounds(scene);
            if (existing != null)
            {
                return existing;
            }

            var gameObject = new GameObject(BoundsObjectName, typeof(BoxCollider2D));
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            gameObject.transform.SetParent(grid.transform, true);
            return gameObject.GetComponent<BoxCollider2D>();
        }

        private static BoxCollider2D FindBounds(Scene scene)
        {
            BoxCollider2D[] colliders = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<BoxCollider2D>(true))
                .ToArray();

            return colliders.FirstOrDefault(collider => collider.name == BoundsObjectName)
                ?? colliders.FirstOrDefault(collider =>
                    collider.name.IndexOf("Movement Boundary", StringComparison.OrdinalIgnoreCase) >= 0)
                ?? colliders.FirstOrDefault(collider =>
                    collider.name.IndexOf("Bounds", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static Grid FindGrid(Scene scene)
        {
            return scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Grid>(true))
                .FirstOrDefault();
        }

        private RectInt CalculateCellBounds()
        {
            int minX = centreCell.x - width / 2;
            int minY = centreCell.y - height / 2;
            return new RectInt(minX, minY, width, height);
        }

        private string ScenePath =>
            sceneKind == SceneKind.Farm ? FarmScenePath : InteriorScenePath;
    }
}
