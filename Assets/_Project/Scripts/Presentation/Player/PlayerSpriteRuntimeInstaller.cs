using System;
using FarmSimulator.Application.Player;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Calibration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Presentation.Player
{
    public static class PlayerSpriteRuntimeInstaller
    {
        private static bool registered;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            registered = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            if (registered)
            {
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            registered = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstallIntoCurrentScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == ProjectSceneNames.Lab)
            {
                Install();
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == ProjectSceneNames.Lab)
            {
                Install();
            }
        }

        public static bool Install()
        {
            GameObject player = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerObjectName);
            if (player == null)
            {
                return false;
            }

            Transform existingSprite = player.transform.Find(
                PlayerSpriteAssetCatalog.SpriteVisualObjectName);
            if (existingSprite != null)
            {
                return true;
            }

            RuntimeAnimatorController controller =
                Resources.Load<RuntimeAnimatorController>(
                    PlayerSpriteAssetCatalog.AnimatorControllerResourcePath);
            Sprite[] sprites = Resources.LoadAll<Sprite>(
                PlayerSpriteAssetCatalog.SpriteSheetResourcePath);

            Sprite idleDown = Array.Find(
                sprites,
                sprite => sprite.name ==
                    PlayerAnimationModel.SpriteName(
                        PlayerAnimationModel.Frames(
                            PlayerAnimationState.IdleDown)[0]));

            if (controller == null || idleDown == null)
            {
                Debug.LogError(
                    "Farmer sprite assets are not ready. Wait for the Editor " +
                    "pipeline to finish or run Tools > Farm Simulator > " +
                    "Rebuild Farmer Sprite Assets.");
                return false;
            }

            GameObject proxy = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerVisualObjectName);
            Renderer proxyRenderer = proxy?.GetComponent<Renderer>();
            if (proxyRenderer != null)
            {
                proxyRenderer.enabled = false;
            }

            var spriteObject = new GameObject(
                PlayerSpriteAssetCatalog.SpriteVisualObjectName);
            spriteObject.transform.SetParent(player.transform, false);
            spriteObject.transform.localPosition = new Vector3(0f, 0f, -0.01f);

            SpriteRenderer spriteRenderer =
                spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = idleDown;
            spriteRenderer.sortingOrder = 100;

            Animator animator = spriteObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;

            PlayerSpriteAnimator animationDriver =
                player.GetComponent<PlayerSpriteAnimator>();
            if (animationDriver == null)
            {
                animationDriver = player.AddComponent<PlayerSpriteAnimator>();
            }

            animationDriver.Initialize(animator);
            return true;
        }
    }
}
