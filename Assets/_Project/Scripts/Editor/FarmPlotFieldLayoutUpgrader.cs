using System;
using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Farming;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Keeps the authored farm field as three rows of five persistent plot entities.
    /// Existing plot visuals and gameplay components are preserved; missing plots are
    /// cloned from the first valid plot and receive stable identifiers.
    /// </summary>
    [InitializeOnLoad]
    public static class FarmPlotFieldLayoutUpgrader
    {
        private const string FarmWorldName = "Farm World";
        private const string FieldRootName = "Farm Plot Field";
        private const string PlotPrefix = "Plot ";
        private const int Columns = 5;
        private const int Rows = 3;
        private static readonly Vector3 FirstPlotPosition = new Vector3(-6f, -3f, 0f);

        private static bool isRunning;

        static FarmPlotFieldLayoutUpgrader()
        {
            EditorApplication.delayCall += EnsureApplied;
        }

        [MenuItem(
            "Tools/Farm Simulator/Farm Development Kit/Farming/" +
            "Arrange 3 x 5 Plot Field")]
        public static void ApplyFromMenu()
        {
            bool changed = Apply();
            EditorUtility.DisplayDialog(
                "Farm plot layout",
                changed
                    ? "Farm now contains three rows of five plots below and left of the hero house."
                    : "The 3 x 5 plot field is already correctly arranged.",
                "OK");
        }

        public static void EnsureApplied()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureApplied;
                return;
            }

            Apply();
        }

        private static bool Apply()
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
                    return false;
                }

                Transform fieldRoot = farmWorld
                    .GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(candidate => candidate.name == FieldRootName);
                if (fieldRoot == null)
                {
                    Debug.LogWarning(
                        "[Farm Plot Layout] Farm Plot Field was not found. Generate the farming field first.");
                    return false;
                }

                GameObject[] existing = fieldRoot.Cast<Transform>()
                    .Where(child => child.name.StartsWith(PlotPrefix, StringComparison.Ordinal))
                    .Select(child => child.gameObject)
                    .OrderBy(plot => plot.name, StringComparer.Ordinal)
                    .ToArray();

                GameObject template = existing.FirstOrDefault(
                    plot => plot.GetComponent<FarmPlotBehaviour>() != null);
                if (template == null)
                {
                    Debug.LogWarning(
                        "[Farm Plot Layout] No configured FarmPlotBehaviour was found to use as a template.");
                    return false;
                }

                bool changed = false;
                int desiredCount = Columns * Rows;

                for (int index = existing.Length; index < desiredCount; index++)
                {
                    GameObject clone = UnityEngine.Object.Instantiate(template, fieldRoot);
                    clone.name = PlotName(index);
                    clone.hideFlags &= ~HideFlags.HideInHierarchy;
                    existing = existing.Append(clone).ToArray();
                    changed = true;
                }

                for (int index = existing.Length - 1; index >= desiredCount; index--)
                {
                    UnityEngine.Object.DestroyImmediate(existing[index]);
                    changed = true;
                }

                GameObject[] plots = fieldRoot.Cast<Transform>()
                    .Where(child => child.name.StartsWith(PlotPrefix, StringComparison.Ordinal))
                    .Select(child => child.gameObject)
                    .OrderBy(plot => plot.name, StringComparer.Ordinal)
                    .Take(desiredCount)
                    .ToArray();

                for (int index = 0; index < desiredCount; index++)
                {
                    int row = index / Columns;
                    int column = index % Columns;
                    GameObject plot = plots[index];
                    string desiredName = $"Plot {column + 1}-{row + 1}";
                    Vector3 desiredPosition = FirstPlotPosition + new Vector3(column, row, 0f);

                    if (plot.name != desiredName)
                    {
                        plot.name = desiredName;
                        changed = true;
                    }

                    if (plot.transform.position != desiredPosition)
                    {
                        plot.transform.position = desiredPosition;
                        changed = true;
                    }

                    if ((plot.hideFlags & HideFlags.HideInHierarchy) != 0)
                    {
                        plot.hideFlags &= ~HideFlags.HideInHierarchy;
                        changed = true;
                    }

                    FarmPlotBehaviour behaviour = plot.GetComponent<FarmPlotBehaviour>();
                    if (behaviour != null)
                    {
                        string desiredId = $"farm-plot-{column}-{row}";
                        SerializedObject serialized = new SerializedObject(behaviour);
                        SerializedProperty plotId = serialized.FindProperty("plotId");
                        if (plotId != null && plotId.stringValue != desiredId)
                        {
                            plotId.stringValue = desiredId;
                            serialized.ApplyModifiedPropertiesWithoutUndo();
                            changed = true;
                        }
                    }
                }

                if (!changed)
                {
                    return false;
                }

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                {
                    throw new InvalidOperationException(
                        "Unity could not save Farm after arranging the 3 x 5 plot field.");
                }

                Debug.Log(
                    "[Farm Plot Layout] Arranged 15 visible plot entities as three rows of five " +
                    "at x=-6..-2 and y=-3..-1.");
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

        private static string PlotName(int index)
        {
            int row = index / Columns;
            int column = index % Columns;
            return $"Plot {column + 1}-{row + 1}";
        }
    }

    internal sealed class FarmPlotFieldLayoutPostprocessor : AssetPostprocessor
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
                EditorApplication.delayCall += FarmPlotFieldLayoutUpgrader.EnsureApplied;
            }
        }
    }
}
