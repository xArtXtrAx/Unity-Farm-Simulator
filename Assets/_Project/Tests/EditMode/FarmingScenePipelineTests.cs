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
        public void FarmContainsNineConfiguredPlotsOnSeparatedTilemapGrid()
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
                    plots.All(plot => plot.SoilRenderer != null && plot.CropRenderer != null),
                    Is.True);
                Assert.That(
                    plots.All(plot => !plot.SoilRenderer.enabled),
                    Is.True,
                    "Untilled plots should visually merge with grass.");

                FarmTilemapLayers layers = scene.GetRootGameObjects()
                    .SelectMany(root => root.GetComponentsInChildren<FarmTilemapLayers>(true))
                    .Single();

                Assert.That(layers.Ground, Is.Not.Null);
                Assert.That(layers.Paths, Is.Not.Null);
                Assert.That(layers.Soil, Is.Not.Null);
                Assert.That(layers.Crops, Is.Not.Null);
                Assert.That(layers.Decoration, Is.Not.Null);
                Assert.That(layers.Soil, Is.Not.SameAs(layers.Crops));
                Assert.That(layers.Ground.GetUsedTilesCount(), Is.EqualTo(1));
                Assert.That(layers.Paths.GetUsedTilesCount(), Is.EqualTo(1));
                Assert.That(CountOccupiedCells(layers.Ground), Is.EqualTo(15 * 9));
                Assert.That(CountOccupiedCells(layers.Paths), Is.EqualTo(3 * 6));
                Assert.That(CountOccupiedCells(layers.Soil), Is.Zero);
                Assert.That(CountOccupiedCells(layers.Crops), Is.Zero);

                TilemapRenderer soilRenderer = layers.Soil.GetComponent<TilemapRenderer>();
                TilemapRenderer cropRenderer = layers.Crops.GetComponent<TilemapRenderer>();
                Assert.That(
                    cropRenderer.sortingOrder,
                    Is.GreaterThan(soilRenderer.sortingOrder));
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
        public void CozyStarterTileCatalogContainsPaintableAssetsAndSeparatedPalettes()
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
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Ground),
                minimumOccupiedCells: 2);
            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Paths),
                minimumOccupiedCells: 2);
            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Soil),
                minimumOccupiedCells: 1);
            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Crops),
                minimumOccupiedCells: 18);
            AssertPalette(
                CozyFarmTileCatalog.GetPalettePath(CozyPaletteCategory.Decoration),
                minimumOccupiedCells: 4);
        }

        [Test]
        public void GeneratedCropSpritesUseTransparencyAndBottomCenteredPivots()
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
                Assert.That(importer.spritePivot.y, Is.Zero.Within(0.001f), path);
            }
        }

        private static void AssertPalette(
            string palettePath,
            int minimumOccupiedCells)
        {
            GameObject palette = AssetDatabase.LoadAssetAtPath<GameObject>(palettePath);
            Assert.That(
                palette,
                Is.Not.Null,
                $"Expected generated palette at '{palettePath}'.");
            Assert.That(palette.GetComponent<Grid>(), Is.Not.Null);

            Tilemap tilemap = palette.GetComponentInChildren<Tilemap>(true);
            Assert.That(tilemap, Is.Not.Null);
            Assert.That(
                CountOccupiedCells(tilemap),
                Is.GreaterThanOrEqualTo(minimumOccupiedCells),
                $"Palette '{palettePath}' does not contain its expected tiles.");
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
