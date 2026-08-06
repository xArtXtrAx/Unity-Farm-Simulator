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

            if (!SnapToNearestCell(selected))
            {
                EditorUtility.DisplayDialog(
                    "Farm Development Kit",
                    "That footprint overlaps another generated building. The previous position was restored.",
                    "OK");
            }
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
            if (!SnapToNearestCell(instance))
            {
                Undo.DestroyObjectImmediate(instance);
                EditorUtility.DisplayDialog(
                    "Farm Development Kit",
                    "The nearest grid footprint is occupied by another building.",
                    "OK");
                return null;
            }

            Selection.activeGameObject = instance;
            return instance;
        }

        public static bool SnapToNearestCell(GameObject instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            GridBuildingFootprint footprint = instance.GetComponent<GridBuildingFootprint>();
            Vector3 previousPosition = instance.transform.position;
            Vector2Int previousCell = footprint == null
                ? Vector2Int.zero
                : footprint.AnchorCell;

            Grid grid = UnityEngine.Object.FindObjectsByType<Grid>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .FirstOrDefault();
            Vector3Int cell;
            Vector3 snappedPosition;
            if (grid == null)
            {
                cell = new Vector3Int(
                    Mathf.RoundToInt(instance.transform.position.x),
                    Mathf.RoundToInt(instance.transform.position.y),
                    0);
                snappedPosition = new Vector3(cell.x, cell.y, 0f);
            }
            else
            {
                cell = grid.WorldToCell(instance.transform.position);
                snappedPosition = grid.GetCellCenterWorld(cell);
            }

            Undo.RecordObject(instance.transform, "Snap building to grid");
            instance.transform.position = snappedPosition;
            if (footprint != null)
            {
                Undo.RecordObject(footprint, "Update building footprint");
                footprint.SetAnchorCell(new Vector2Int(cell.x, cell.y));
                if (OverlapsAnother(footprint))
                {
                    instance.transform.position = previousPosition;
                    footprint.SetAnchorCell(previousCell);
                    return false;
                }

                EditorUtility.SetDirty(footprint);
            }

            return true;
        }

        public static bool OverlapsAnother(GridBuildingFootprint footprint)
        {
            if (footprint == null) return false;
            return UnityEngine.Object.FindObjectsByType<GridBuildingFootprint>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .Any(other => other != footprint && footprint.Overlaps(other));
        }
    }
}
