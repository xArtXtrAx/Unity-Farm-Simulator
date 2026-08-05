using FarmSimulator.Application.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Presentation.Bootstrap
{
    internal static class BootstrapStartup
    {
        private static bool isTransitioning;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            isTransitioning = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name != ProjectSceneNames.Bootstrap || isTransitioning)
            {
                return;
            }

            if (!UnityEngine.Application.CanStreamedLevelBeLoaded(ProjectSceneNames.Lab))
            {
                Debug.LogError(
                    $"Bootstrap could not load the required scene '{ProjectSceneNames.Lab}'. " +
                    "Verify the project build settings.");
                return;
            }

            isTransitioning = true;
            AsyncOperation operation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Lab,
                LoadSceneMode.Single);

            if (operation == null)
            {
                isTransitioning = false;
                Debug.LogError(
                    $"Unity did not create an async load operation for '{ProjectSceneNames.Lab}'.");
            }
        }
    }
}
