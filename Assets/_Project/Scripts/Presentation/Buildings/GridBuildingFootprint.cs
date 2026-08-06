using System;
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
    }
}
