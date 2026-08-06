using System;
using System.Linq;
using FarmSimulator.Presentation.Buildings;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public static class CozyBuildingGridPlacementUtility
    {
        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Snap Selected Building To Grid")]
        public static void SnapSelectedToGrid()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null || selected.GetComponent<GridBuildingFootprint>() == null)
            {
                EditorUtility.DisplayDialog(
                    "Farm Development Kit",
                    "Select a generated building instance first.",
                    "OK");
                return;
            }

            SnapToNearestCell(selected);
        }

        public static GameObject PlacePrefab(GameObject prefab)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null) return null;

            Undo.RegisterCreatedObjectUndo(instance, "Place Cozy building prefab");
            Vector3 target = SceneView.lastActiveSceneView == null
                ? Vector3.zero
                : SceneView.lastActiveSceneView.pivot;
            target.z = 0f;
            instance.transform.position = target;
            SnapToNearestCell(instance);
            Selection.activeGameObject = instance;
            return instance;
        }

        public static void SnapToNearestCell(GameObject instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            Grid grid = UnityEngine.Object.FindObjectsByType<Grid>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .FirstOrDefault();
            if (grid == null)
            {
                instance.transform.position = new Vector3(
                    Mathf.Round(instance.transform.position.x),
                    Mathf.Round(instance.transform.position.y),
                    0f);
                return;
            }

            Undo.RecordObject(instance.transform, "Snap building to grid");
            Vector3Int cell = grid.WorldToCell(instance.transform.position);
            instance.transform.position = grid.GetCellCenterWorld(cell);
            GridBuildingFootprint footprint = instance.GetComponent<GridBuildingFootprint>();
            if (footprint != null)
            {
                footprint.SetAnchorCell(new Vector2Int(cell.x, cell.y));
                EditorUtility.SetDirty(footprint);
            }
        }
    }
}
