using System.Collections.Generic;
using FarmSimulator.Presentation.Buildings;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    [CustomEditor(typeof(GridBuildingFootprint))]
    public sealed class GridBuildingFootprintEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var footprint = (GridBuildingFootprint)target;
            Grid grid = Object.FindFirstObjectByType<Grid>(FindObjectsInactive.Include);
            Vector2Int previewAnchor = GetPreviewAnchor(footprint, grid);
            var previewCells = new List<Vector2Int>(footprint.GetOccupiedCells(previewAnchor));
            bool blocked = OverlapsAnother(footprint, previewCells, grid);

            Handles.color = blocked
                ? new Color(1f, 0.2f, 0.2f, 0.65f)
                : new Color(0.2f, 1f, 0.35f, 0.55f);

            foreach (Vector2Int cell in previewCells)
            {
                Vector3 center = grid == null
                    ? new Vector3(cell.x, cell.y, 0f)
                    : grid.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
                Vector3 size = grid == null ? Vector3.one : grid.cellSize;
                Handles.DrawSolidRectangleWithOutline(
                    new[]
                    {
                        center + new Vector3(-size.x * 0.5f, -size.y * 0.5f),
                        center + new Vector3(-size.x * 0.5f, size.y * 0.5f),
                        center + new Vector3(size.x * 0.5f, size.y * 0.5f),
                        center + new Vector3(size.x * 0.5f, -size.y * 0.5f),
                    },
                    Handles.color,
                    new Color(Handles.color.r, Handles.color.g, Handles.color.b, 1f));
            }

            Handles.color = Color.cyan;
            Handles.DrawWireDisc(footprint.AnchorWorldPosition, Vector3.forward, 0.12f);
            Handles.Label(
                footprint.AnchorWorldPosition + Vector3.up * 0.2f,
                blocked ? "Footprint blocked" : $"{previewCells.Count} occupied cells");
            SceneView.RepaintAll();
        }

        private static Vector2Int GetPreviewAnchor(GridBuildingFootprint footprint, Grid grid)
        {
            Vector3 position = footprint.AnchorWorldPosition;
            if (grid == null)
            {
                return new Vector2Int(
                    Mathf.RoundToInt(position.x),
                    Mathf.RoundToInt(position.y));
            }

            Vector3Int cell = grid.WorldToCell(position);
            return new Vector2Int(cell.x, cell.y);
        }

        private static bool OverlapsAnother(
            GridBuildingFootprint footprint,
            IReadOnlyList<Vector2Int> previewCells,
            Grid grid)
        {
            var preview = new HashSet<Vector2Int>(previewCells);
            foreach (GridBuildingFootprint other in
                     Object.FindObjectsByType<GridBuildingFootprint>(
                         FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                if (other == footprint || other.gameObject.scene != footprint.gameObject.scene)
                {
                    continue;
                }

                Vector2Int otherAnchor = GetPreviewAnchor(other, grid);
                foreach (Vector2Int occupied in other.GetOccupiedCells(otherAnchor))
                {
                    if (preview.Contains(occupied)) return true;
                }
            }

            return false;
        }
    }
}
