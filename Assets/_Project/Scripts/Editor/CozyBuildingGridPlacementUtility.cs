using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Presentation.Buildings;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public static class CozyBuildingGridPlacementUtility
    {
        private const int PlacementSearchRadius = 24;

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

            if (!PlaceAtNearestFreeCell(instance, PlacementSearchRadius))
            {
                Undo.DestroyObjectImmediate(instance);
                EditorUtility.DisplayDialog(
                    "Farm Development Kit",
                    "No free grid footprint was found near the Scene view. Move the Scene view to an open area and try again.",
                    "OK");
                return null;
            }

            Selection.activeGameObject = instance;
            SceneView.lastActiveSceneView?.FrameSelected();
            return instance;
        }

        public static bool PlaceAtNearestFreeCell(GameObject instance, int searchRadius)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            GridBuildingFootprint footprint = instance.GetComponent<GridBuildingFootprint>();
            if (footprint == null) return SnapToNearestCell(instance);

            Grid grid = FindGrid();
            Vector3Int origin = WorldToCell(grid, footprint.AnchorWorldPosition);
            int radiusLimit = Mathf.Max(0, searchRadius);
            for (int radius = 0; radius <= radiusLimit; radius++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        if (radius > 0 && Mathf.Abs(x) != radius && Mathf.Abs(y) != radius)
                        {
                            continue;
                        }

                        Vector3Int candidate = new Vector3Int(origin.x + x, origin.y + y, 0);
                        SetCell(instance, footprint, grid, candidate);
                        if (!OverlapsAnother(footprint))
                        {
                            EditorUtility.SetDirty(footprint);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool SnapToNearestCell(GameObject instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            GridBuildingFootprint footprint = instance.GetComponent<GridBuildingFootprint>();
            Vector3 previousPosition = instance.transform.position;
            Vector2Int previousCell = footprint == null ? Vector2Int.zero : footprint.AnchorCell;

            Grid grid = FindGrid();
            Vector3 anchorPosition = footprint == null
                ? instance.transform.position
                : footprint.AnchorWorldPosition;
            Vector3Int cell = WorldToCell(grid, anchorPosition);

            Undo.RecordObject(instance.transform, "Snap building to grid");
            if (footprint != null) Undo.RecordObject(footprint, "Update building footprint");

            SetCell(instance, footprint, grid, cell);
            if (footprint != null && OverlapsAnother(footprint))
            {
                instance.transform.position = previousPosition;
                footprint.SetAnchorCell(previousCell);
                return false;
            }

            if (footprint != null) EditorUtility.SetDirty(footprint);
            return true;
        }

        public static bool OverlapsAnother(GridBuildingFootprint footprint)
        {
            if (footprint == null) return false;
            Grid grid = FindGrid();
            Vector2Int anchor = CurrentAnchor(footprint, grid);
            var occupied = new HashSet<Vector2Int>(footprint.GetOccupiedCells(anchor));

            foreach (GridBuildingFootprint other in
                     UnityEngine.Object.FindObjectsByType<GridBuildingFootprint>(
                         FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                if (other == footprint || other.gameObject.scene != footprint.gameObject.scene)
                {
                    continue;
                }

                Vector2Int otherAnchor = CurrentAnchor(other, grid);
                if (other.GetOccupiedCells(otherAnchor).Any(occupied.Contains)) return true;
            }

            return false;
        }

        private static Grid FindGrid()
        {
            return UnityEngine.Object.FindObjectsByType<Grid>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .FirstOrDefault();
        }

        private static Vector2Int CurrentAnchor(GridBuildingFootprint footprint, Grid grid)
        {
            Vector3Int cell = WorldToCell(grid, footprint.AnchorWorldPosition);
            return new Vector2Int(cell.x, cell.y);
        }

        private static Vector3Int WorldToCell(Grid grid, Vector3 position)
        {
            return grid == null
                ? new Vector3Int(
                    Mathf.RoundToInt(position.x),
                    Mathf.RoundToInt(position.y),
                    0)
                : grid.WorldToCell(position);
        }

        private static void SetCell(
            GameObject instance,
            GridBuildingFootprint footprint,
            Grid grid,
            Vector3Int cell)
        {
            Vector3 targetAnchor = grid == null
                ? new Vector3(cell.x, cell.y, 0f)
                : grid.GetCellCenterWorld(cell);
            Vector3 currentAnchor = footprint == null
                ? instance.transform.position
                : footprint.AnchorWorldPosition;
            instance.transform.position += targetAnchor - currentAnchor;
            footprint?.SetAnchorCell(new Vector2Int(cell.x, cell.y));
        }
    }
}
