using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarmSimulator.Presentation.Buildings
{
    [DisallowMultipleComponent]
    public sealed class GridBuildingFootprint : MonoBehaviour
    {
        [SerializeField] private string buildingId;
        [SerializeField] private Vector2Int gridSize = Vector2Int.one;
        [SerializeField] private Vector2Int anchorCell;
        [SerializeField] private Vector2Int[] occupiedOffsets = { Vector2Int.zero };
        [SerializeField] private Transform footprintAnchor;

        public string BuildingId => buildingId;
        public Vector2Int GridSize => gridSize;
        public Vector2Int AnchorCell => anchorCell;
        public IReadOnlyList<Vector2Int> OccupiedOffsets => occupiedOffsets;
        public Transform FootprintAnchor => footprintAnchor;
        public Vector3 AnchorWorldPosition =>
            footprintAnchor == null ? transform.position : footprintAnchor.position;

        public void Configure(string id, Vector2Int size)
        {
            Configure(id, size, CreateRectangleOffsets(size), null);
        }

        public void Configure(
            string id,
            Vector2Int size,
            IEnumerable<Vector2Int> offsets)
        {
            Configure(id, size, offsets, null);
        }

        public void Configure(
            string id,
            Vector2Int size,
            IEnumerable<Vector2Int> offsets,
            Transform anchor)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Building ID cannot be empty.", nameof(id));
            }

            buildingId = id;
            gridSize = new Vector2Int(Mathf.Max(1, size.x), Mathf.Max(1, size.y));
            occupiedOffsets = NormalizeOffsets(offsets).ToArray();
            if (occupiedOffsets.Length == 0)
            {
                occupiedOffsets = new[] { Vector2Int.zero };
            }
            footprintAnchor = anchor;
        }

        public void SetAnchorCell(Vector2Int cell)
        {
            anchorCell = cell;
        }

        public IEnumerable<Vector2Int> GetOccupiedCells()
        {
            return GetOccupiedCells(anchorCell);
        }

        public IEnumerable<Vector2Int> GetOccupiedCells(Vector2Int anchor)
        {
            for (int index = 0; index < occupiedOffsets.Length; index++)
            {
                yield return anchor + occupiedOffsets[index];
            }
        }

        public bool Overlaps(GridBuildingFootprint other)
        {
            if (other == null || ReferenceEquals(this, other)) return false;
            var occupied = new HashSet<Vector2Int>(GetOccupiedCells());
            return other.GetOccupiedCells().Any(occupied.Contains);
        }

        public static int GetCanvasMinimumX(int width)
        {
            return -(Mathf.Max(1, width) / 2);
        }

        public static IEnumerable<Vector2Int> CreateRectangleOffsets(Vector2Int size)
        {
            size = new Vector2Int(Mathf.Max(1, size.x), Mathf.Max(1, size.y));
            int startX = GetCanvasMinimumX(size.x);
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    yield return new Vector2Int(startX + x, y);
                }
            }
        }

        private static IEnumerable<Vector2Int> NormalizeOffsets(
            IEnumerable<Vector2Int> offsets)
        {
            if (offsets == null) yield break;
            foreach (Vector2Int offset in offsets
                         .Distinct()
                         .OrderBy(value => value.y)
                         .ThenBy(value => value.x))
            {
                yield return offset;
            }
        }
    }
}
