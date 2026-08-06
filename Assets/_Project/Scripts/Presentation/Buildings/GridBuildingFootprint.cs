using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarmSimulator.Presentation.Buildings
{
    [DisallowMultipleComponent]
    public sealed class GridBuildingFootprint : MonoBehaviour
    {
        [SerializeField] private string buildingId;
        [SerializeField] private Vector2Int gridSize = Vector2Int.one;
        [SerializeField] private Vector2Int anchorCell;

        public string BuildingId => buildingId;
        public Vector2Int GridSize => gridSize;
        public Vector2Int AnchorCell => anchorCell;

        public void Configure(string id, Vector2Int size)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Building ID cannot be empty.", nameof(id));
            }

            buildingId = id;
            gridSize = new Vector2Int(Mathf.Max(1, size.x), Mathf.Max(1, size.y));
        }

        public void SetAnchorCell(Vector2Int cell)
        {
            anchorCell = cell;
        }

        public IEnumerable<Vector2Int> GetOccupiedCells()
        {
            int startX = anchorCell.x - ((gridSize.x - 1) / 2);
            int startY = anchorCell.y - ((gridSize.y - 1) / 2);
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    yield return new Vector2Int(startX + x, startY + y);
                }
            }
        }

        public bool Overlaps(GridBuildingFootprint other)
        {
            if (other == null || ReferenceEquals(this, other)) return false;
            var occupied = new HashSet<Vector2Int>(GetOccupiedCells());
            foreach (Vector2Int cell in other.GetOccupiedCells())
            {
                if (occupied.Contains(cell)) return true;
            }

            return false;
        }
    }
}
