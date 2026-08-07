using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyInteriorHouseArtTests
    {
        private static readonly string[] FirstPartySpritePaths =
        {
            "Assets/_Project/Art/Placeholder/Source/house_floor.png",
            "Assets/_Project/Art/Placeholder/Source/house_wall.png",
            "Assets/_Project/Art/Placeholder/Source/bed_single.png",
        };

        [Test]
        public void RecoveryProfileUsesFirstPartyInteriorAssets()
        {
            SceneRecoveryArtProfile.PrepareFirstPartyArtProfile();
            SceneRecoveryArtProfile profile = SceneRecoveryArtProfile.LoadOrCreate();

            Assert.That(profile.houseFloorTile, Is.Not.Null);
            Assert.That(profile.houseWallTile, Is.Not.Null);
            Assert.That(profile.bedSprite, Is.Not.Null);

            Assert.That(
                AssetDatabase.GetAssetPath(profile.houseFloorTile),
                Does.StartWith("Assets/_Project/Art/Placeholder/"));
            Assert.That(
                AssetDatabase.GetAssetPath(profile.houseWallTile),
                Does.StartWith("Assets/_Project/Art/Placeholder/"));
            Assert.That(
                AssetDatabase.GetAssetPath(profile.bedSprite),
                Does.StartWith("Assets/_Project/Art/Placeholder/"));
        }

        [Test]
        public void FirstPartyInteriorSpritesUsePixelArtSettings()
        {
            foreach (string path in FirstPartySpritePaths)
            {
                TextureImporter importer =
                    AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer, Is.Not.Null, path);
                Assert.That(importer.spriteImportMode, Is.EqualTo(SpriteImportMode.Single), path);
                Assert.That(importer.spritePixelsPerUnit, Is.EqualTo(16f), path);
                Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point), path);
                Assert.That(importer.mipmapEnabled, Is.False, path);
            }
        }

        [Test]
        public void RecoveryProfileContainsAllRequiredReferences()
        {
            SceneRecoveryArtProfile.PrepareFirstPartyArtProfile();
            SceneRecoveryArtProfile profile = SceneRecoveryArtProfile.LoadOrCreate();

            Assert.That(profile.farmGroundTile, Is.Not.Null);
            Assert.That(profile.farmPathTile, Is.Not.Null);
            Assert.That(profile.farmHouseSprite, Is.Not.Null);
            Assert.That(profile.houseFloorTile, Is.Not.Null);
            Assert.That(profile.houseWallTile, Is.Not.Null);
            Assert.That(profile.bedSprite, Is.Not.Null);
        }
    }
}
