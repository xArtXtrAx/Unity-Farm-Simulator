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
            bool blocked = CozyBuildingGridPlacementUtility.OverlapsAnother(footprint);
            Handles.color = blocked
                ? new Color(1f, 0.2f, 0.2f, 0.65f)
                : new Color(0.2f, 1f, 0.35f, 0.55f);

            foreach (Vector2Int cell in footprint.GetOccupiedCells())
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
                blocked ? "Footprint blocked" : $"{footprint.GridSize.x} × {footprint.GridSize.y} cells");
        }
    }
}
