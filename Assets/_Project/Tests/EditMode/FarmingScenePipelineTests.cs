using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Editor;
using FarmSimulator.Presentation.Farming;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class FarmingScenePipelineTests
    {
        [Test]
        public void FarmContainsNineConfiguredPlots()
        {
            HouseAndSleepScenePipeline.EnsureScenes();
            Assert.That(
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    ProjectSceneNames.FarmPath),
                Is.Not.Null);

            FarmSceneFarmingUpgrader.ApplyFromMenu();

            Scene scene =
                SceneManager.GetSceneByPath(ProjectSceneNames.FarmPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(
                    ProjectSceneNames.FarmPath,
                    OpenSceneMode.Additive);
            }

            try
            {
                FarmPlotBehaviour[] plots =
                    scene.GetRootGameObjects()
                        .SelectMany(root =>
                            root.GetComponentsInChildren<
                                FarmPlotBehaviour>(true))
                        .ToArray();

                Assert.That(
                    plots.Length,
                    Is.EqualTo(
                        FarmSceneFarmingUpgrader.Columns *
                        FarmSceneFarmingUpgrader.Rows));

                string[] identifiers =
                    plots.Select(plot => plot.PlotId)
                        .OrderBy(value => value)
                        .ToArray();
                Assert.That(
                    new HashSet<string>(identifiers).Count,
                    Is.EqualTo(identifiers.Length));
                Assert.That(
                    plots.All(plot =>
                        plot.SoilRenderer != null &&
                        plot.CropRenderer != null),
                    Is.True);
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
    }
}
