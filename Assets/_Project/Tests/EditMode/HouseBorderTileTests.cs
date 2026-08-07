using FarmSimulator.Presentation.Art;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

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

            BoundsInt bounds = new BoundsInt(-2, -2, 0, 5, 5, 1);

            Assert.That(
                HouseBorderTile.ResolveRotationDegrees(
                    new Vector3Int(0, 2, 0), bounds),
                Is.EqualTo(0f));
            Assert.That(
                HouseBorderTile.ResolveRotationDegrees(
                    new Vector3Int(0, -2, 0), bounds),
                Is.EqualTo(180f));
            Assert.That(
                HouseBorderTile.ResolveRotationDegrees(
                    new Vector3Int(-2, 0, 0), bounds),
                Is.EqualTo(90f));
            Assert.That(
                HouseBorderTile.ResolveRotationDegrees(
                    new Vector3Int(2, 0, 0), bounds),
                Is.EqualTo(-90f));
        }
    }
}
