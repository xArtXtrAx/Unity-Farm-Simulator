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
        public void FarmContainsNinePlotsAndTerrainOnlyTilemaps()
        {
            HouseAndSleepScenePipeline.EnsureScenes();
            Assert.That(
                AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.FarmPath),
                Is.Not.Null);

            FarmSceneFarmingUpgrader.ApplyFromMenu();
            Scene scene = SceneManager.GetSceneByPath(ProjectSceneNames.FarmPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(
                    ProjectSceneNames.FarmPath,
                    OpenSceneMode.Additive);
            }

            try
            {
                FarmPlotBehaviour[] plots = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<FarmPlotBehaviour>(true))
                    .ToArray();
                Assert.That(
                    plots.Length,
                    Is.EqualTo(FarmSceneFarmingUpgrader.Columns * FarmSceneFarmingUpgrader.Rows));

                string[] identifiers = plots.Select(plot => plot.PlotId)
                    .OrderBy(value => value)
                    .ToArray();
                Assert.That(
                    new HashSet<string>(identifiers).Count,
                    Is.EqualTo(identifiers.Length));
                Assert.That(
                    plots.All(plot =>
                        plot.SoilRenderer != null &&
                        plot.CropRenderer != null &&
                        plot.CropRenderer.GetComponent<Tilemap>() == null),
                    Is.True,
                    "Each crop must be a plot-owned SpriteRenderer entity.");
                Assert.That(
                    plots.All(plot => plot.SoilRenderer.enabled),
                    Is.True,
                    "Untilled plots should expose a subtle visible guide.");
                Assert.That(
                    plots.All(plot => plot.SoilRenderer.color.a > 0f &&
                                      plot.SoilRenderer.color.a < 1f),
                    Is.True,
                    "Untilled guides should be translucent rather than worked soil.");

                FarmTilemapLayers layers = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<FarmTilemapLayers>(true))
                    .Single();
                Assert.That(layers.Ground, Is.Not.Null);
                Assert.That(layers.Paths, Is.Not.Null);
                Assert.That(layers.Soil, Is.Not.Null);
                Assert.That(layers.Decoration, Is.Not.Null);
                Assert.That(CountOccupiedCells(layers.Ground), Is.EqualTo(15 * 9));
                Assert.That(CountOccupiedCells(layers.Paths), Is.EqualTo(3 * 6));
                Assert.That(CountOccupiedCells(layers.Soil), Is.Zero);

                string[] tilemapNames = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<Tilemap>(true))
                    .Select(tilemap => tilemap.name)
                    .ToArray();
                Assert.That(tilemapNames, Does.Not.Contain("Crops"));
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
        public void CozyCatalogContainsWorldPalettesButNoCropPalette()
        {
            CozyFarmTileCatalog.Rebuild();
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(CozyFarmTileCatalog.GrassTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(CozyFarmTileCatalog.DirtTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(CozyFarmTileCatalog.WaterTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(CozyFarmTileCatalog.TilledSoilTilePath),
                Is.Not.Null);

            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Ground), 2);
            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Paths), 2);
            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Soil), 1);
            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Decoration), 4);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    CozyFarmTileCatalog.PaletteRoot + "/Cozy Farm - Crops.prefab"),
                Is.Null);
        }

        [Test]
        public void GeneratedCropSpritesAreRuntimeReady()
        {
            CozyFarmTileCatalog.Rebuild();
            string[] guids = AssetDatabase.FindAssets(
                "t:Sprite",
                new[] { CozyFarmTileCatalog.GeneratedCropRoot });
            Assert.That(guids.Length, Is.EqualTo(18));

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(sprite, Is.Not.Null, path);
                Assert.That(importer, Is.Not.Null, path);
                Assert.That(importer.alphaIsTransparency, Is.True, path);
                Assert.That(importer.spritePixelsPerUnit, Is.EqualTo(16f), path);
                Assert.That(importer.spritePivot.x, Is.EqualTo(0.5f).Within(0.001f), path);
                Assert.That(importer.spritePivot.y, Is.EqualTo(0.5f).Within(0.001f), path);
            }
        }

        private static void AssertPalette(string palettePath, int minimumOccupiedCells)
        {
            GameObject palette = AssetDatabase.LoadAssetAtPath<GameObject>(palettePath);
            Assert.That(palette, Is.Not.Null, palettePath);
            Assert.That(palette.GetComponent<Grid>(), Is.Not.Null);
            Tilemap tilemap = palette.GetComponentInChildren<Tilemap>(true);
            Assert.That(tilemap, Is.Not.Null);
            Assert.That(
                CountOccupiedCells(tilemap),
                Is.GreaterThanOrEqualTo(minimumOccupiedCells));
        }

        private static int CountOccupiedCells(Tilemap tilemap)
        {
            int count = 0;
            foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(position)) count++;
            }
            return count;
        }
    }
}
