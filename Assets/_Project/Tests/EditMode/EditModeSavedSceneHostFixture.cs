using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Tests.EditMode
{
    [SetUpFixture]
    public sealed class EditModeSavedSceneHostFixture
    {
        private const string TempFolder =
            "Assets/_Project/Tests/Temp";
        private const string TempScenePath =
            TempFolder + "/EditMode Test Host.unity";

        private bool createdTemporaryHost;

        [OneTimeSetUp]
        public void EnsureSavedHostScene()
        {
            Scene active = SceneManager.GetActiveScene();
            if (!active.IsValid() || !string.IsNullOrEmpty(active.path))
            {
                return;
            }

            EnsureFolder(TempFolder);
            if (!EditorSceneManager.SaveScene(active, TempScenePath))
            {
                throw new System.InvalidOperationException(
                    $"Could not save the EditMode host scene to '{TempScenePath}'.");
            }

            createdTemporaryHost = true;
        }

        [OneTimeTearDown]
        public void RemoveTemporaryHostScene()
        {
            if (!createdTemporaryHost)
            {
                return;
            }

            Scene active = SceneManager.GetActiveScene();
            if (active.IsValid() && active.path == TempScenePath)
            {
                EditorSceneManager.NewScene(
                    NewSceneSetup.EmptyScene,
                    NewSceneMode.Single);
            }

            AssetDatabase.DeleteAsset(TempScenePath);
            AssetDatabase.DeleteAsset(TempFolder);
            AssetDatabase.Refresh();
        }

        private static void EnsureFolder(string assetPath)
        {
            string normalized = assetPath.Replace('\\', '/');
            string[] parts = normalized.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }
    }
}
