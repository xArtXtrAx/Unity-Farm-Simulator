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
    /// Enforces one visual Tilemap authority in Farm while remaining strictly
    /// editor-only. Automatic normalization must never open or save scenes during
    /// Play Mode transitions.
    /// </summary>
    [InitializeOnLoad]
    public static class FarmSceneAuthoringGridNormalizer
    {
        private const string FarmWorldName = "Farm World";
        private const string GridName = "Farm Authoring Grid";
        private const string FieldRootName = "Farm Plot Field";
        private const string PlotPrefix = "Plot ";

        private static bool isRunning;
        private static bool callbackQueued;
        private static bool playTransitionActive;

        static FarmSceneAuthoringGridNormalizer()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            QueueEnsureClean();
        }

        [MenuItem(
            "Tools/Farm Simulator/Farm Development Kit/Scene Cleanup/" +
            "Normalize Farm Authoring Grid")]
        public static void NormalizeFromMenu()
        {
            if (!CanAuthorScenes())
            {
                Debug.LogWarning("[Farm Scene Cleanup] Scene authoring is unavailable during Play Mode.");
                return;
            }

            bool changed = NormalizeFarmScene();
            EditorUtility.DisplayDialog(
                "Farm scene cleanup",
                changed
                    ? "Farm was normalized. Duplicate terrain grids were removed and " +
                      "plot entities remain visible in the hierarchy."
                    : "Farm is already clean and uses one canonical authoring grid.",
                "OK");
        }

        public static void EnsureFarmSceneIsClean()
        {
            callbackQueued = false;

            if (!CanAuthorScenes())
            {
                return;
            }

            if (isRunning || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                if (!isRunning)
                {
                    QueueEnsureClean();
                }

                return;
            }

            NormalizeFarmScene();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.ExitingPlayMode:
                    playTransitionActive = true;
                    CancelQueuedCallback();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    playTransitionActive = false;
                    QueueEnsureClean();
                    break;
            }
        }

        private static bool CanAuthorScenes() =>
            !playTransitionActive &&
            !EditorApplication.isPlaying &&
            !EditorApplication.isPlayingOrWillChangePlaymode &&
            !Application.isPlaying;

        private static void QueueEnsureClean()
        {
            if (callbackQueued || !CanAuthorScenes())
            {
                return;
            }

            callbackQueued = true;
            EditorApplication.delayCall += EnsureFarmSceneIsClean;
        }

        private static void CancelQueuedCallback()
        {
            EditorApplication.delayCall -= EnsureFarmSceneIsClean;
            callbackQueued = false;
        }

        public static bool NormalizeFarmScene()
        {
            if (!CanAuthorScenes() ||
                isRunning ||
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
                    if (!CanAuthorScenes())
                    {
                        return false;
                    }

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

                bool changed = RemoveDuplicateGrids(farmWorld.transform, canonical);
                changed |= RestoreVisiblePlots(farmWorld.transform);

                if (!changed)
                {
                    return false;
                }

                if (!CanAuthorScenes())
                {
                    return false;
                }

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                {
                    throw new InvalidOperationException(
                        "Unity could not save Farm after normalizing its hierarchy.");
                }

                Debug.Log(
                    "[Farm Scene Cleanup] Farm normalized: one terrain grid and visible plot entities.");
                return true;
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded && !Application.isPlaying)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }

                isRunning = false;
            }
        }

        private static bool RemoveDuplicateGrids(
            Transform farmWorld,
            Transform canonical)
        {
            Transform[] duplicates = farmWorld
                .GetComponentsInChildren<Transform>(true)
                .Where(candidate =>
                    candidate != canonical &&
                    candidate.name == GridName)
                .ToArray();

            foreach (Transform duplicate in duplicates)
            {
                Debug.Log(
                    "[Farm Scene Cleanup] Removing duplicate terrain grid: " +
                    GetHierarchyPath(duplicate));
                UnityEngine.Object.DestroyImmediate(duplicate.gameObject);
            }

            return duplicates.Length > 0;
        }

        private static bool RestoreVisiblePlots(Transform farmWorld)
        {
            Transform fieldRoot = farmWorld
                .GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(candidate => candidate.name == FieldRootName);
            if (fieldRoot == null)
            {
                return false;
            }

            bool changed = false;
            for (int index = 0; index < fieldRoot.childCount; index++)
            {
                GameObject plot = fieldRoot.GetChild(index).gameObject;
                if (!plot.name.StartsWith(PlotPrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                HideFlags desired = plot.hideFlags & ~HideFlags.HideInHierarchy;
                if (plot.hideFlags == desired)
                {
                    continue;
                }

                plot.hideFlags = desired;
                EditorUtility.SetDirty(plot);
                changed = true;
            }

            return changed;
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

        internal static bool CanScheduleAutomaticAuthoring => CanAuthorScenes();
    }

    internal sealed class FarmSceneAuthoringGridCleanupPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!FarmSceneAuthoringGridNormalizer.CanScheduleAutomaticAuthoring)
            {
                return;
            }

            if (importedAssets.Any(path => string.Equals(
                    path,
                    ProjectSceneNames.FarmPath,
                    StringComparison.Ordinal)))
            {
                FarmSceneAuthoringGridNormalizer.EnsureFarmSceneIsClean();
            }
        }
    }
}
