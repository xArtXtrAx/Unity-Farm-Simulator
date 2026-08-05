using System.Linq;
using FarmSimulator.Application.Player;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Player;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class PlayerPrefabAssetPipelineTests
    {
        [Test]
        public void RequiredTopDownSortingLayersExist()
        {
            string[] sortingLayers = SortingLayer.layers
                .Select(layer => layer.name)
                .ToArray();

            foreach (string requiredLayer in
                     TopDownSortingLayers.RequiredNames)
            {
                Assert.That(
                    sortingLayers,
                    Does.Contain(requiredLayer));
            }
        }

        [Test]
        public void PlayerPrefabContainsValidatedRuntimeComposition()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                PlayerPrefabAssetCatalog.PrefabAssetPath);
            Assert.That(prefab, Is.Not.Null);

            Assert.That(
                prefab.GetComponent<PlayerPrefabIdentity>(),
                Is.Not.Null);
            Assert.That(
                prefab.GetComponent<TopDownPlayerMotor>(),
                Is.Not.Null);
            Assert.That(
                prefab.GetComponent<UnifiedMovementInput>(),
                Is.Not.Null);
            Assert.That(
                prefab.GetComponent<PlayerSpriteAnimator>(),
                Is.Not.Null);
            Assert.That(
                prefab.GetComponent<PlayerProxyFacingView>(),
                Is.Not.Null);

            Rigidbody2D body = prefab.GetComponent<Rigidbody2D>();
            Assert.That(body, Is.Not.Null);
            Assert.That(body.bodyType, Is.EqualTo(RigidbodyType2D.Dynamic));
            Assert.That(body.gravityScale, Is.Zero);
            Assert.That(
                body.constraints & RigidbodyConstraints2D.FreezeRotation,
                Is.EqualTo(RigidbodyConstraints2D.FreezeRotation));

            CapsuleCollider2D collider =
                prefab.GetComponent<CapsuleCollider2D>();
            Assert.That(collider, Is.Not.Null);
            Assert.That(
                collider.size.x,
                Is.EqualTo(LabSpatialCalibration.PlayerColliderWidth)
                    .Within(0.001f));
            Assert.That(
                collider.size.y,
                Is.EqualTo(LabSpatialCalibration.PlayerColliderHeight)
                    .Within(0.001f));
            Assert.That(
                collider.offset.y,
                Is.EqualTo(LabSpatialCalibration.PlayerColliderOffsetY)
                    .Within(0.001f));

            Transform spriteTransform = prefab.transform.Find(
                PlayerSpriteAssetCatalog.SpriteVisualObjectName);
            Assert.That(spriteTransform, Is.Not.Null);

            SpriteRenderer renderer =
                spriteTransform.GetComponent<SpriteRenderer>();
            Animator animator = spriteTransform.GetComponent<Animator>();
            Assert.That(renderer, Is.Not.Null);
            Assert.That(renderer.sprite, Is.Not.Null);
            Assert.That(
                renderer.sortingLayerName,
                Is.EqualTo(TopDownSortingLayers.Actors));
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.runtimeAnimatorController, Is.Not.Null);

            PlayerSpriteAnimator animationDriver =
                prefab.GetComponent<PlayerSpriteAnimator>();
            Assert.That(animationDriver.Animator, Is.EqualTo(animator));

            TopDownSpriteSorting sorting =
                prefab.GetComponent<TopDownSpriteSorting>();
            Assert.That(sorting, Is.Not.Null);
            Assert.That(sorting.TargetRenderer, Is.EqualTo(renderer));
            Assert.That(sorting.Feet, Is.EqualTo(prefab.transform));

            TopDownPlayerMotor motor =
                prefab.GetComponent<TopDownPlayerMotor>();
            Assert.That(
                motor.Speed,
                Is.EqualTo(PlayerMovementModel.DefaultSpeedUnitsPerSecond)
                    .Within(0.001f));
        }

        [Test]
        public void PlayerPrefabUsesCurrentGenerationSignature()
        {
            AssetImporter importer = AssetImporter.GetAtPath(
                PlayerPrefabAssetCatalog.PrefabAssetPath);
            Assert.That(importer, Is.Not.Null);
            Assert.That(
                importer.userData,
                Is.EqualTo(PlayerPrefabAssetCatalog.ImportSignature));
        }
    }
}
