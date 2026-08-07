using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyInteriorHouseArtTests
    {
        private const string ProfilePath =
            "Assets/_Project/Editor/Scene Recovery Art Profile.asset";

        private static readonly string[] FirstPartySpritePaths =
        {
            "Assets/_Project/Art/Placeholder/Source/house_floor.png",
            "Assets/_Project/Art/Placeholder/Source/house_wall.png",
            "Assets/_Project/Art/Placeholder/Source/bed_single.png",
        };

        [Test]
        public void RecoveryProfileUsesFirstPartyInteriorAssets()
        {
            SerializedObject profile = LoadProfile();
            Object floor = Reference(profile, "houseFloorTile");
            Object wall = Reference(profile, "houseWallTile");
            Object bed = Reference(profile, "bedSprite");

            Assert.That(floor, Is.Not.Null);
            Assert.That(wall, Is.Not.Null);
            Assert.That(bed, Is.Not.Null);

            Assert.That(
                AssetDatabase.GetAssetPath(floor),
                Does.StartWith("Assets/_Project/Art/Placeholder/"));
            Assert.That(
                AssetDatabase.GetAssetPath(wall),
                Does.StartWith("Assets/_Project/Art/Placeholder/"));
            Assert.That(
                AssetDatabase.GetAssetPath(bed),
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
            SerializedObject profile = LoadProfile();

            Assert.That(Reference(profile, "farmGroundTile"), Is.Not.Null);
            Assert.That(Reference(profile, "farmPathTile"), Is.Not.Null);
            Assert.That(Reference(profile, "farmHouseSprite"), Is.Not.Null);
            Assert.That(Reference(profile, "houseFloorTile"), Is.Not.Null);
            Assert.That(Reference(profile, "houseWallTile"), Is.Not.Null);
            Assert.That(Reference(profile, "bedSprite"), Is.Not.Null);
        }

        private static SerializedObject LoadProfile()
        {
            Object asset = AssetDatabase.LoadMainAssetAtPath(ProfilePath);
            Assert.That(asset, Is.Not.Null, ProfilePath);
            return new SerializedObject(asset);
        }

        private static Object Reference(SerializedObject serializedObject, string propertyName)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            Assert.That(property, Is.Not.Null, propertyName);
            return property.objectReferenceValue;
        }
    }
}
