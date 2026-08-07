using System;
using System.Linq;
using FarmSimulator.Application.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Enforces one visual Tilemap authority in Farm:
    /// Farm World/Farm Authoring Grid.
    /// Farming systems may own plots and runtime logic, but never a second terrain grid.
    /// </summary>
    [InitializeOnLoad]
    public static class FarmSceneAuthoringGridNormalizer
    {
        private const string FarmWorldName = "Farm World";
        private const string GridName = "Farm Authoring Grid";

        private static bool isRunning;

        static FarmSceneAuthoringGridNormalizer()
        {
            EditorApplication.delayCall += EnsureFarmSceneIsClean;
        }

        [MenuItem(
            "Tools/Farm Simulator/Farm Development Kit/Scene Cleanup/" +
            "Normalize Farm Authoring Grid")]
        public static void NormalizeFromMenu()
        {
            bool changed = NormalizeFarmScene();
            EditorUtility.DisplayDialog(
                "Farm scene cleanup",
                changed
                    ? "Duplicate Farm Authoring Grid objects were removed. " +
                      "The canonical grid under Farm World was preserved."
                    : "Farm is already clean and uses one canonical authoring grid.",
                "OK");
        }

        public static void EnsureFarmSceneIsClean()
        {
            if (isRunning || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                if (!isRunning)
                {
                    EditorApplication.delayCall += EnsureFarmSceneIsClean;
                }

                return;
            }

            NormalizeFarmScene();
        }

        public static bool NormalizeFarmScene()
        {
            if (isRunning ||
                AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.FarmPath) == null)
            {
                return false;
            }

            isRunning = true;
            Scene scene = SceneManager.GetSceneByPath(ProjectSceneNames.FarmPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;

            try
            {
                if (openedHere)
                {
                    scene = EditorSceneManager.OpenScene(
                        ProjectSceneNames.FarmPath,
                        OpenSceneMode.Additive);
                }

                GameObject farmWorld = scene.GetRootGameObjects()
                    .FirstOrDefault(root => root.name == FarmWorldName);
                if (farmWorld == null)
                {
                    Debug.LogWarning(
                        "[Farm Scene Cleanup] Farm World was not found; no cleanup was applied.");
                    return false;
                }

                Transform canonical = FindDirectChild(farmWorld.transform, GridName);
                if (canonical == null)
                {
                    Debug.LogWarning(
                        "[Farm Scene Cleanup] The canonical Farm World/Farm Authoring Grid " +
                        "was not found; duplicate grids were preserved to avoid data loss.");
                    return false;
                }

                Transform[] duplicates = farmWorld
                    .GetComponentsInChildren<Transform>(true)
                    .Where(candidate =>
                        candidate != canonical &&
                        candidate.name == GridName)
                    .ToArray();

                if (duplicates.Length == 0)
                {
                    return false;
                }

                foreach (Transform duplicate in duplicates)
                {
                    Debug.Log(
                        "[Farm Scene Cleanup] Removing duplicate terrain grid: " +
                        GetHierarchyPath(duplicate));
                    UnityEngine.Object.DestroyImmediate(duplicate.gameObject);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                {
                    throw new InvalidOperationException(
                        "Unity could not save Farm after removing duplicate authoring grids.");
                }

                Debug.Log(
                    $"[Farm Scene Cleanup] Removed {duplicates.Length} duplicate grid(s). " +
                    "Farm World/Farm Authoring Grid is the only terrain authority.");
                return true;
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }

                isRunning = false;
            }
        }

        private static Transform FindDirectChild(Transform parent, string name)
        {
            for (int index = 0; index < parent.childCount; index++)
            {
                Transform child = parent.GetChild(index);
                if (child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        private static string GetHierarchyPath(Transform target)
        {
            string path = target.name;
            Transform parent = target.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }

    internal sealed class FarmSceneAuthoringGridCleanupPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Any(path => string.Equals(
                    path,
                    ProjectSceneNames.FarmPath,
                    StringComparison.Ordinal)))
            {
                EditorApplication.delayCall +=
                    FarmSceneAuthoringGridNormalizer.EnsureFarmSceneIsClean;
            }
        }
    }
}
