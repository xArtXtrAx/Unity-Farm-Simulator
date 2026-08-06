using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Presentation.Inventory
{
    internal static class InventoryHotbarInstaller
    {
        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(
            Scene scene,
            LoadSceneMode loadMode)
        {
            bool supportsHotbar =
                scene.name == ProjectSceneNames.Lab ||
                scene.name == ProjectSceneNames.Farm;

            if (!supportsHotbar ||
                Object.FindFirstObjectByType<InventoryHotbarView>() != null)
            {
                return;
            }

            GameObject prefab = Resources.Load<GameObject>(
                InventoryHotbarAssetCatalog.ResourcePath);
            if (prefab == null)
            {
                Debug.LogError(
                    $"Could not load inventory hotbar prefab at " +
                    $"Resources/{InventoryHotbarAssetCatalog.ResourcePath}.");
                return;
            }

            GameObject instance = Object.Instantiate(prefab);
            instance.name = InventoryHotbarAssetCatalog.RootObjectName;
            SceneManager.MoveGameObjectToScene(instance, scene);

            InventoryHotbarView view =
                instance.GetComponent<InventoryHotbarView>();
            if (view == null)
            {
                Debug.LogError(
                    "Inventory hotbar prefab does not contain " +
                    "InventoryHotbarView.");
                Object.Destroy(instance);
                return;
            }

            view.Initialize(GameSessionRuntime.Instance.Inventory);
        }
    }
}
