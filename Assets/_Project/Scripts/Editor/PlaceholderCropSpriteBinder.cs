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
    [InitializeOnLoad]
    public static class PlaceholderCropSpriteBinder
    {
        private const string CropFolder =
            "Assets/_Project/Art/Placeholder/Crops";

        static PlaceholderCropSpriteBinder()
        {
            EditorApplication.delayCall += EnsureApplied;
        }

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Farming/Bind Placeholder Crop Sprites")]
        public static void ApplyFromMenu() => Apply(saveEvenWhenUnchanged: true);

        public static void EnsureApplied() => Apply(saveEvenWhenUnchanged: false);

        private static void Apply(bool saveEvenWhenUnchanged)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureApplied;
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.FarmPath) == null)
            {
                return;
            }

            Sprite[] turnipStages = LoadSixStages("turnip", sourceStageCount: 5);
            Sprite[] potatoStages = LoadSixStages("potato", sourceStageCount: 6);
            Sprite[] radishStages = LoadSixStages("radish", sourceStageCount: 5);
            if (!Complete(turnipStages) || !Complete(potatoStages) || !Complete(radishStages))
            {
                Debug.LogWarning(
                    "[Farming] Placeholder crop sprites are incomplete; plot binding was skipped.");
                return;
            }

            Scene scene = SceneManager.GetSceneByPath(ProjectSceneNames.FarmPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(
                    ProjectSceneNames.FarmPath,
                    OpenSceneMode.Additive);
            }

            bool changed = false;
            try
            {
                FarmPlotBehaviour[] plots = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<FarmPlotBehaviour>(true))
                    .ToArray();

                foreach (FarmPlotBehaviour plot in plots)
                {
                    var serialized = new SerializedObject(plot);
                    bool plotChanged = false;
                    plotChanged |= AssignArray(
                        serialized.FindProperty("turnipStages"),
                        turnipStages);

                    // The current domain still exposes the second and third crop slots as
                    // carrot/cabbage. Keep those serialized contracts stable while using the
                    // approved potato and radish art until the domain-name migration lands.
                    plotChanged |= AssignArray(
                        serialized.FindProperty("carrotStages"),
                        potatoStages);
                    plotChanged |= AssignArray(
                        serialized.FindProperty("cabbageStages"),
                        radishStages);

                    if (plotChanged)
                    {
                        serialized.ApplyModifiedPropertiesWithoutUndo();
                        EditorUtility.SetDirty(plot);
                        changed = true;
                    }
                }

                if ((changed || saveEvenWhenUnchanged) && plots.Length > 0)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                    {
                        throw new InvalidOperationException(
                            "Could not save Farm after binding placeholder crop sprites.");
                    }
                }

                if (plots.Length > 0 && (changed || saveEvenWhenUnchanged))
                {
                    Debug.Log(
                        $"[Farming] Bound placeholder crop sprites to {plots.Length} plots " +
                        "(turnip, potato compatibility slot, radish compatibility slot).");
                }
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static Sprite[] LoadSixStages(string cropName, int sourceStageCount)
        {
            var stages = new Sprite[6];
            for (int index = 0; index < sourceStageCount; index++)
            {
                string path = $"{CropFolder}/{cropName}_stage_{index}.png";
                stages[index] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            for (int index = sourceStageCount; index < stages.Length; index++)
            {
                stages[index] = stages[sourceStageCount - 1];
            }
            return stages;
        }

        private static bool AssignArray(SerializedProperty property, Sprite[] sprites)
        {
            if (property == null || !property.isArray)
            {
                throw new InvalidOperationException(
                    "FarmPlotBehaviour crop-stage serialization contract was not found.");
            }

            bool changed = property.arraySize != sprites.Length;
            property.arraySize = sprites.Length;
            for (int index = 0; index < sprites.Length; index++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                if (element.objectReferenceValue == sprites[index]) continue;
                element.objectReferenceValue = sprites[index];
                changed = true;
            }
            return changed;
        }

        private static bool Complete(Sprite[] sprites) =>
            sprites != null &&
            sprites.Length == 6 &&
            sprites.All(sprite => sprite != null);
    }

    internal sealed class PlaceholderCropSpritePostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Any(path =>
                    path.StartsWith(CropFolderPrefix, StringComparison.Ordinal) ||
                    string.Equals(path, ProjectSceneNames.FarmPath, StringComparison.Ordinal)))
            {
                EditorApplication.delayCall += PlaceholderCropSpriteBinder.EnsureApplied;
            }
        }

        private const string CropFolderPrefix =
            "Assets/_Project/Art/Placeholder/Crops/";
    }
}
