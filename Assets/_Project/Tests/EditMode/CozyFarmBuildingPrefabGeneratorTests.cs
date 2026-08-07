using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmBuildingPrefabGeneratorTests
    {
        private const string HousePrefabPath =
            "Assets/_Project/Art/Placeholder/Prefabs/house_small_4x5.prefab";

        [Test]
        public void FirstPartyHousePrefabExistsAndUsesProjectSprite()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HousePrefabPath);
            Assert.That(prefab, Is.Not.Null);

            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            Assert.That(renderer, Is.Not.Null);
            Assert.That(renderer.sprite, Is.Not.Null);
            Assert.That(
                AssetDatabase.GetAssetPath(renderer.sprite),
                Does.StartWith("Assets/_Project/Art/Placeholder/Source/"));
        }

        [Test]
        public void FirstPartyHousePrefabContainsAuthoritativeFootprintMetadata()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HousePrefabPath);
            Assert.That(prefab, Is.Not.Null);

            Component identity = prefab.GetComponent("PlaceholderAssetIdentity");
            Assert.That(identity, Is.Not.Null);

            var serialized = new SerializedObject(identity);
            Assert.That(
                serialized.FindProperty("assetKey").stringValue,
                Is.EqualTo("building.house.small.4x5"));
            Assert.That(
                serialized.FindProperty("footprintCells").vector2IntValue,
                Is.EqualTo(new Vector2Int(4, 5)));
        }

        [Test]
        public void FirstPartyHousePrefabKeepsFunctionalColliderAtBase()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HousePrefabPath);
            Assert.That(prefab, Is.Not.Null);

            BoxCollider2D collider = prefab.GetComponent<BoxCollider2D>();
            Assert.That(collider, Is.Not.Null);
            Assert.That(collider.isTrigger, Is.False);
            Assert.That(collider.size.x, Is.GreaterThan(0f));
            Assert.That(collider.size.y, Is.GreaterThan(0f));

            SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            Assert.That(renderer, Is.Not.Null);
            Assert.That(renderer.transform.localPosition, Is.EqualTo(Vector3.zero));
        }
    }
}
