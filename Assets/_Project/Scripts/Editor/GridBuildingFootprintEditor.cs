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
            IReadOnlyList<Vector2Int> previewCells = GetOccupiedCells(
                previewAnchor,
                footprint.GridSize);
            bool blocked = OverlapsAnother(footprint, previewCells);

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

            Handles.Label(
                footprint.transform.position + Vector3.up * 0.35f,
                blocked
                    ? "Footprint blocked"
                    : $"{footprint.GridSize.x} × {footprint.GridSize.y} cells");

            SceneView.RepaintAll();
        }

        private static Vector2Int GetPreviewAnchor(
            GridBuildingFootprint footprint,
            Grid grid)
        {
            if (grid == null)
            {
                return new Vector2Int(
                    Mathf.RoundToInt(footprint.transform.position.x),
                    Mathf.RoundToInt(footprint.transform.position.y));
            }

            Vector3Int cell = grid.WorldToCell(footprint.transform.position);
            return new Vector2Int(cell.x, cell.y);
        }

        private static IReadOnlyList<Vector2Int> GetOccupiedCells(
            Vector2Int anchor,
            Vector2Int size)
        {
            var cells = new List<Vector2Int>(size.x * size.y);
            int startX = anchor.x - ((size.x - 1) / 2);
            int startY = anchor.y - ((size.y - 1) / 2);
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    cells.Add(new Vector2Int(startX + x, startY + y));
                }
            }

            return cells;
        }

        private static bool OverlapsAnother(
            GridBuildingFootprint footprint,
            IReadOnlyList<Vector2Int> previewCells)
        {
            var preview = new HashSet<Vector2Int>(previewCells);
            foreach (GridBuildingFootprint other in
                     Object.FindObjectsByType<GridBuildingFootprint>(
                         FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                if (other == footprint ||
                    other.gameObject.scene != footprint.gameObject.scene)
                {
                    continue;
                }

                foreach (Vector2Int occupied in other.GetOccupiedCells())
                {
                    if (preview.Contains(occupied))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
