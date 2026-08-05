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
            GameObject provisionalPlayer = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerObjectName);
            if (provisionalPlayer == null)
            {
                return false;
            }

            if (provisionalPlayer.GetComponent<PlayerPrefabIdentity>() != null)
            {
                return true;
            }

            GameObject prefab = Resources.Load<GameObject>(
                PlayerPrefabAssetCatalog.ResourcePath);
            if (prefab == null)
            {
                Debug.LogError(
                    "The Player prefab is not ready. Wait for the Editor " +
                    "pipeline or run Tools > Farm Simulator > " +
                    "Rebuild Player Prefab.");
                return false;
            }

            Transform oldTransform = provisionalPlayer.transform;
            Transform parent = oldTransform.parent;
            Vector3 position = oldTransform.position;
            Quaternion rotation = oldTransform.rotation;
            Vector3 scale = oldTransform.localScale;

            Transform proxyVisual = oldTransform.Find(
                LabSpatialCalibration.PlayablePlayerVisualObjectName);
            Transform facingMarker = oldTransform.Find(
                LabSpatialCalibration.PlayerFacingMarkerObjectName);

            provisionalPlayer.SetActive(false);

            GameObject player = Object.Instantiate(
                prefab,
                position,
                rotation,
                parent);
            player.name = LabSpatialCalibration.PlayablePlayerObjectName;
            player.transform.localScale = scale;

            TransferTechnicalChild(proxyVisual, player.transform);
            TransferTechnicalChild(facingMarker, player.transform);

            Renderer proxyRenderer = proxyVisual?.GetComponent<Renderer>();
            if (proxyRenderer != null)
            {
                proxyRenderer.enabled = false;
            }

            PlayerProxyFacingView facingView =
                player.GetComponent<PlayerProxyFacingView>();
            if (facingView != null && facingMarker != null)
            {
                facingView.Initialize(facingMarker);
            }

            Animator animator = player.GetComponentInChildren<Animator>(
                includeInactive: true);
            player.GetComponent<PlayerSpriteAnimator>()?.Initialize(animator);

            TopDownSpriteSorting sorting =
                player.GetComponent<TopDownSpriteSorting>();
            SpriteRenderer spriteRenderer =
                player.GetComponentInChildren<SpriteRenderer>(
                    includeInactive: true);
            sorting?.Initialize(spriteRenderer, player.transform);

            Object.Destroy(provisionalPlayer);
            return true;
        }

        private static void TransferTechnicalChild(
            Transform child,
            Transform newParent)
        {
            if (child == null)
            {
                return;
            }

            Vector3 localPosition = child.localPosition;
            Quaternion localRotation = child.localRotation;
            Vector3 localScale = child.localScale;

            child.SetParent(newParent, worldPositionStays: false);
            child.localPosition = localPosition;
            child.localRotation = localRotation;
            child.localScale = localScale;
            child.gameObject.SetActive(true);
        }
    }
}
