using FarmSimulator.Application.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Presentation.Calibration
{
    public static class LabSpatialCalibrationInstaller
    {
        private const string InstallerObjectName = "Lab Spatial Calibration";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterSceneCallback()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureCurrentScene()
        {
            EnsureCalibration(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            EnsureCalibration(scene);
        }

        private static void EnsureCalibration(Scene scene)
        {
            if (!scene.IsValid() || scene.name != ProjectSceneNames.Lab)
            {
                return;
            }

            LabSpatialCalibration calibration =
                Object.FindFirstObjectByType<LabSpatialCalibration>();
            if (calibration == null)
            {
                var installerObject = new GameObject(InstallerObjectName);
                SceneManager.MoveGameObjectToScene(installerObject, scene);
                calibration =
                    installerObject.AddComponent<LabSpatialCalibration>();
            }

            LabDepthSortingReference depthReference =
                calibration.GetComponent<LabDepthSortingReference>();
            if (depthReference == null)
            {
                depthReference =
                    calibration.gameObject.AddComponent<LabDepthSortingReference>();
            }

            depthReference.EnsureCreated();
        }
    }
}
