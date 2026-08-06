using System.Linq;
using FarmSimulator.Presentation.Buildings;
using NUnit.Framework;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class GridBuildingFootprintTests
    {
        [Test]
        public void OccupiedCellsMatchConfiguredGridSize()
        {
            var go = new GameObject("Footprint");
            try
            {
                GridBuildingFootprint footprint = go.AddComponent<GridBuildingFootprint>();
                footprint.Configure("test-building", new Vector2Int(3, 2));
                footprint.SetAnchorCell(new Vector2Int(4, 5));

                Vector2Int[] cells = footprint.GetOccupiedCells().ToArray();
                Assert.That(cells, Has.Length.EqualTo(6));
                Assert.That(cells, Does.Contain(new Vector2Int(3, 5)));
                Assert.That(cells, Does.Contain(new Vector2Int(5, 6)));
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void OverlapDetectsSharedCellsOnly()
        {
            var firstObject = new GameObject("First");
            var secondObject = new GameObject("Second");
            try
            {
                GridBuildingFootprint first = firstObject.AddComponent<GridBuildingFootprint>();
                GridBuildingFootprint second = secondObject.AddComponent<GridBuildingFootprint>();
                first.Configure("first", new Vector2Int(2, 2));
                second.Configure("second", new Vector2Int(2, 2));
                first.SetAnchorCell(Vector2Int.zero);
                second.SetAnchorCell(new Vector2Int(1, 0));
                Assert.That(first.Overlaps(second), Is.True);

                second.SetAnchorCell(new Vector2Int(4, 0));
                Assert.That(first.Overlaps(second), Is.False);
            }
            finally
            {
                Object.DestroyImmediate(firstObject);
                Object.DestroyImmediate(secondObject);
            }
        }
    }
}
