using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Editor;
using FarmSimulator.Presentation.Farming;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class FarmingScenePipelineTests
    {
        [Test]
        public void FarmContainsNineConfiguredPlotsOnTilemapGrid()
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
                Assert.That(
                    plots.All(plot => !plot.SoilRenderer.enabled),
                    Is.True,
                    "Untilled plots should visually merge with grass.");

                FarmTilemapLayers layers =
                    scene.GetRootGameObjects()
                        .SelectMany(root =>
                            root.GetComponentsInChildren<
                                FarmTilemapLayers>(true))
                        .Single();

                Assert.That(layers.Ground, Is.Not.Null);
                Assert.That(layers.Paths, Is.Not.Null);
                Assert.That(layers.Farming, Is.Not.Null);
                Assert.That(layers.Decoration, Is.Not.Null);
                Assert.That(
                    layers.Ground.GetUsedTilesCount(),
                    Is.EqualTo(1));
                Assert.That(
                    layers.Paths.GetUsedTilesCount(),
                    Is.EqualTo(1));
                Assert.That(
                    CountOccupiedCells(layers.Ground),
                    Is.EqualTo(15 * 9));
                Assert.That(
                    CountOccupiedCells(layers.Paths),
                    Is.EqualTo(3 * 6));
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        [Test]
        public void CozyStarterTileCatalogContainsPaintableAssets()
        {
            CozyFarmTileCatalog.Rebuild();

            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(
                    CozyFarmTileCatalog.GrassTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(
                    CozyFarmTileCatalog.DirtTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(
                    CozyFarmTileCatalog.WaterTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(
                    CozyFarmTileCatalog.TilledSoilTilePath),
                Is.Not.Null);

            GameObject palette =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    CozyFarmTileCatalog.PalettePrefabPath);
            Assert.That(palette, Is.Not.Null);
            Assert.That(palette.GetComponent<Grid>(), Is.Not.Null);
            Assert.That(
                palette.GetComponentInChildren<Tilemap>(true),
                Is.Not.Null);
        }

        private static int CountOccupiedCells(Tilemap tilemap)
        {
            int count = 0;
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(position))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
