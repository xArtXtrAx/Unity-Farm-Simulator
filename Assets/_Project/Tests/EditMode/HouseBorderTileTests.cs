using FarmSimulator.Presentation.Art;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class HouseBorderTileTests
    {
        private const string TilePath =
            "Assets/_Project/Art/Placeholder/Tiles/house_wall_oriented.asset";

        [Test]
        public void HouseWallTileRotatesTrimTowardRoomInterior()
        {
            HouseBorderTile tile =
                AssetDatabase.LoadAssetAtPath<HouseBorderTile>(TilePath);
            Assert.That(tile, Is.Not.Null);
            Assert.That(tile.Sprite, Is.Not.Null);

            GameObject gridObject = new GameObject("Test Grid", typeof(Grid));
            GameObject tilemapObject = new GameObject(
                "Walls",
                typeof(Tilemap),
                typeof(TilemapRenderer));
            tilemapObject.transform.SetParent(gridObject.transform, false);
            Tilemap tilemap = tilemapObject.GetComponent<Tilemap>();

            try
            {
                BoundsInt bounds = new BoundsInt(-2, -2, 0, 5, 5, 1);
                foreach (Vector3Int position in bounds.allPositionsWithin)
                {
                    bool border =
                        position.x == bounds.xMin ||
                        position.x == bounds.xMax - 1 ||
                        position.y == bounds.yMin ||
                        position.y == bounds.yMax - 1;
                    if (border)
                    {
                        tilemap.SetTile(position, tile);
                    }
                }

                tilemap.RefreshAllTiles();

                AssertDirection(tilemap, new Vector3Int(0, 2, 0), Vector3.down);
                AssertDirection(tilemap, new Vector3Int(0, -2, 0), Vector3.up);
                AssertDirection(tilemap, new Vector3Int(-2, 0, 0), Vector3.right);
                AssertDirection(tilemap, new Vector3Int(2, 0, 0), Vector3.left);
            }
            finally
            {
                Object.DestroyImmediate(gridObject);
            }
        }

        private static void AssertDirection(
            Tilemap tilemap,
            Vector3Int position,
            Vector3 expected)
        {
            Vector3 actual = tilemap
                .GetTransformMatrix(position)
                .MultiplyVector(Vector3.down)
                .normalized;

            Assert.That(Vector3.Dot(actual, expected), Is.GreaterThan(0.999f));
        }
    }
}
