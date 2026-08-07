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
        private const string CropFolder = "Assets/_Project/Art/Placeholder/Crops";

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
                return;

            Sprite[] turnipStages = LoadStages("turnip", stageCount: 5);
            Sprite[] potatoStages = LoadStages("potato", stageCount: 6);
            Sprite[] radishStages = LoadStages("radish", stageCount: 5);
            if (!Complete(turnipStages, 5) || !Complete(potatoStages, 6) || !Complete(radishStages, 5))
            {
                Debug.LogWarning("[Farming] Placeholder crop sprites are incomplete; plot binding was skipped.");
                return;
            }

            Scene scene = SceneManager.GetSceneByPath(ProjectSceneNames.FarmPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
                scene = EditorSceneManager.OpenScene(ProjectSceneNames.FarmPath, OpenSceneMode.Additive);

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
                    plotChanged |= AssignArray(serialized.FindProperty("turnipStages"), turnipStages);
                    plotChanged |= AssignArray(serialized.FindProperty("potatoStages"), potatoStages);
                    plotChanged |= AssignArray(serialized.FindProperty("radishStages"), radishStages);

                    if (plotChanged && serialized.ApplyModifiedPropertiesWithoutUndo())
                    {
                        changed = true;
                        EditorUtility.SetDirty(plot);
                    }
                }

                if ((changed || saveEvenWhenUnchanged) && plots.Length > 0)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                        throw new InvalidOperationException("Could not save Farm after binding placeholder crop sprites.");
                }

                if (plots.Length > 0 && (changed || saveEvenWhenUnchanged))
                    Debug.Log($"[Farming] Bound turnip, potato and radish sprites to {plots.Length} plots.");
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                    EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static Sprite[] LoadStages(string cropName, int stageCount)
        {
            var stages = new Sprite[stageCount];
            for (int index = 0; index < stageCount; index++)
            {
                string path = $"{CropFolder}/{cropName}_stage_{index}.png";
                stages[index] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            return stages;
        }

        private static bool AssignArray(SerializedProperty property, Sprite[] sprites)
        {
            if (property == null || !property.isArray)
                throw new InvalidOperationException("FarmPlotBehaviour crop-stage serialization contract was not found.");

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

        private static bool Complete(Sprite[] sprites, int expectedCount) =>
            sprites != null && sprites.Length == expectedCount && sprites.All(sprite => sprite != null);
    }

    internal sealed class PlaceholderCropSpritePostprocessor : AssetPostprocessor
    {
        private const string CropFolderPrefix = "Assets/_Project/Art/Placeholder/Crops/";

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
    }
}
