using System.Collections;
using FarmSimulator.Application.Player;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FarmSimulator.Tests.PlayMode
{
    public sealed class PlayerSpriteAnimationTests
    {
        [UnityTest]
        public IEnumerator LabInstallsAndDrivesAnimatedFarmerSprite()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Lab,
                LoadSceneMode.Single);
            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;
            yield return null;
            yield return null;

            GameObject player = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerObjectName);
            Assert.That(player, Is.Not.Null);

            Transform spriteTransform = player.transform.Find(
                PlayerSpriteAssetCatalog.SpriteVisualObjectName);
            Assert.That(spriteTransform, Is.Not.Null);

            SpriteRenderer spriteRenderer =
                spriteTransform.GetComponent<SpriteRenderer>();
            Animator animator = spriteTransform.GetComponent<Animator>();
            PlayerSpriteAnimator animationDriver =
                player.GetComponent<PlayerSpriteAnimator>();
            TopDownPlayerMotor motor =
                player.GetComponent<TopDownPlayerMotor>();

            Assert.That(spriteRenderer, Is.Not.Null);
            Assert.That(spriteRenderer.sprite, Is.Not.Null);
            Assert.That(animator, Is.Not.Null);
            Assert.That(animator.runtimeAnimatorController, Is.Not.Null);
            Assert.That(animationDriver, Is.Not.Null);
            Assert.That(motor, Is.Not.Null);

            Renderer proxyRenderer = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerVisualObjectName)
                ?.GetComponent<Renderer>();
            Assert.That(proxyRenderer, Is.Not.Null);
            Assert.That(proxyRenderer.enabled, Is.False);

            Assert.That(
                animationDriver.CurrentState,
                Is.EqualTo(PlayerAnimationState.IdleDown));

            motor.SetDesiredInput(Vector2.right);
            animationDriver.Refresh();
            Assert.That(
                animationDriver.CurrentState,
                Is.EqualTo(PlayerAnimationState.WalkRight));

            motor.Stop();
            animationDriver.Refresh();
            Assert.That(
                animationDriver.CurrentState,
                Is.EqualTo(PlayerAnimationState.IdleRight));
        }
    }
}
