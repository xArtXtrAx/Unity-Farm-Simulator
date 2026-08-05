using System;
using System.Linq;
using FarmSimulator.Application.Spatial;
using FarmSimulator.Editor;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Player;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmShowcaseSceneTests
    {
        [OneTimeSetUp]
        public void EnsureShowcaseExists()
        {
            CozyFarmShowcaseScenePipeline.EnsureScene();
        }

        [Test]
        public void ShowcaseSceneIsGeneratedAndSigned()
        {
            SceneAsset sceneAsset =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    CozyFarmShowcaseScenePipeline.SceneAssetPath);
            AssetImporter importer = AssetImporter.GetAtPath(
                CozyFarmShowcaseScenePipeline.SceneAssetPath);

            Assert.That(sceneAsset, Is.Not.Null);
            Assert.That(importer, Is.Not.Null);
            Assert.That(
                importer.userData,
                Is.EqualTo(
                    CozyFarmShowcaseScenePipeline.ImportSignature));
        }

        [Test]
        public void ShowcaseContainsCuratedSpriteGroups()
        {
            WithShowcaseScene(scene =>
            {
                GameObject root = FindRoot(
                    scene,
                    CozyFarmShowcaseScenePipeline.RootObjectName);
                Assert.That(root, Is.Not.Null);

                Transform terrain = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.TerrainGroupName);
                Transform catalog = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.CatalogGroupName);
                Transform crops = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.CropGroupName);

                Assert.That(terrain, Is.Not.Null);
                Assert.That(catalog, Is.Not.Null);
                Assert.That(crops, Is.Not.Null);

                string[] terrainNames = Enumerable
                    .Range(0, terrain.childCount)
                    .Select(index => terrain.GetChild(index).name)
                    .ToArray();
                CollectionAssert.AreEquivalent(
                    new[]
                    {
                        "cozy_grass",
                        "cozy_dirt",
                        "cozy_water",
                        "cozy_tilled_soil"
                    },
                    terrainNames);

                string[] catalogSprites = catalog
                    .GetComponentsInChildren<SpriteRenderer>(true)
                    .Select(renderer => renderer.gameObject.name)
                    .Where(name => name.StartsWith(
                        "cozy_",
                        StringComparison.Ordinal))
                    .ToArray();
                CollectionAssert.AreEquivalent(
                    new[]
                    {
                        "cozy_turnip",
                        "cozy_carrot",
                        "cozy_cabbage",
                        "cozy_turnip_seeds",
                        "cozy_carrot_seeds",
                        "cozy_cabbage_seeds"
                    },
                    catalogSprites);

                string[] cropSprites = crops
                    .GetComponentsInChildren<SpriteRenderer>(true)
                    .Select(renderer => renderer.gameObject.name)
                    .Where(name => name.StartsWith(
                        "cozy_",
                        StringComparison.Ordinal))
                    .ToArray();

                Assert.That(cropSprites, Has.Length.EqualTo(18));
                foreach (string cropName in
                         new[] { "turnip", "carrot", "cabbage" })
                {
                    for (int stage = 0; stage < 6; stage++)
                    {
                        Assert.That(
                            cropSprites,
                            Does.Contain(
                                $"cozy_{cropName}_stage_{stage}"));
                    }
                }
            });
        }

        [Test]
        public void ShowcaseSeparatesUiScaleIconsFromWorldScaleSprites()
        {
            WithShowcaseScene(scene =>
            {
                GameObject root = FindRoot(
                    scene,
                    CozyFarmShowcaseScenePipeline.RootObjectName);
                Assert.That(root, Is.Not.Null);

                Transform catalog = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.CatalogGroupName);
                Transform crops = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.CropGroupName);
                Assert.That(catalog, Is.Not.Null);
                Assert.That(crops, Is.Not.Null);
                Assert.That(
                    CozyFarmShowcaseScenePipeline.CatalogIconScale,
                    Is.EqualTo(0.75f).Within(0.001f));

                Transform panel = catalog.Find(
                    CozyFarmShowcaseScenePipeline.CatalogPanelObjectName);
                Assert.That(panel, Is.Not.Null);
                Assert.That(
                    panel.GetComponentsInChildren<SpriteRenderer>(true),
                    Has.Length.EqualTo(6));

                SpriteRenderer[] catalogIcons = catalog
                    .GetComponentsInChildren<SpriteRenderer>(true)
                    .Where(renderer => renderer.gameObject.name.StartsWith(
                        "cozy_",
                        StringComparison.Ordinal))
                    .ToArray();
                Assert.That(catalogIcons, Has.Length.EqualTo(6));
                foreach (SpriteRenderer icon in catalogIcons)
                {
                    Assert.That(
                        icon.transform.localScale.x,
                        Is.EqualTo(
                            CozyFarmShowcaseScenePipeline.CatalogIconScale)
                            .Within(0.001f));
                    Assert.That(
                        icon.transform.localScale.y,
                        Is.EqualTo(
                            CozyFarmShowcaseScenePipeline.CatalogIconScale)
                            .Within(0.001f));
                }

                SpriteRenderer[] cropStages = crops
                    .GetComponentsInChildren<SpriteRenderer>(true)
                    .Where(renderer => renderer.gameObject.name.StartsWith(
                        "cozy_",
                        StringComparison.Ordinal))
                    .ToArray();
                Assert.That(cropStages, Has.Length.EqualTo(18));
                foreach (SpriteRenderer stage in cropStages)
                {
                    Assert.That(
                        stage.transform.localScale,
                        Is.EqualTo(Vector3.one));
                }
            });
        }

        [Test]
        public void ShowcaseCentersPlantedSeedStagesWithinTheirSoilRows()
        {
            WithShowcaseScene(scene =>
            {
                GameObject root = FindRoot(
                    scene,
                    CozyFarmShowcaseScenePipeline.RootObjectName);
                Assert.That(root, Is.Not.Null);

                Transform crops = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.CropGroupName);
                Assert.That(crops, Is.Not.Null);
                Assert.That(
                    CozyFarmShowcaseScenePipeline
                        .PlantedSeedStageYOffset,
                    Is.EqualTo(-0.3f).Within(0.001f));

                string[] cropNames =
                    { "turnip", "carrot", "cabbage" };
                float[] rowY = { 0.05f, -0.95f, -1.95f };

                for (int cropIndex = 0;
                     cropIndex < cropNames.Length;
                     cropIndex++)
                {
                    string cropName = cropNames[cropIndex];
                    Transform row = crops.Find(
                        $"{cropName} stages");
                    Assert.That(row, Is.Not.Null);

                    Transform plantedSeeds = row.Find(
                        $"cozy_{cropName}_stage_0");
                    Transform firstSprout = row.Find(
                        $"cozy_{cropName}_stage_1");
                    Assert.That(plantedSeeds, Is.Not.Null);
                    Assert.That(firstSprout, Is.Not.Null);

                    Assert.That(
                        plantedSeeds.localPosition.y,
                        Is.EqualTo(
                            rowY[cropIndex] +
                            CozyFarmShowcaseScenePipeline
                                .PlantedSeedStageYOffset)
                            .Within(0.001f));
                    Assert.That(
                        firstSprout.localPosition.y,
                        Is.EqualTo(rowY[cropIndex])
                            .Within(0.001f));
                    Assert.That(
                        plantedSeeds.localPosition.x,
                        Is.EqualTo(-5.35f).Within(0.001f));
                }
            });
        }

        [Test]
        public void ShowcaseUsesCompactSharedBedsAndSingleTileSamples()
        {
            WithShowcaseScene(scene =>
            {
                GameObject root = FindRoot(
                    scene,
                    CozyFarmShowcaseScenePipeline.RootObjectName);
                Assert.That(root, Is.Not.Null);

                Transform terrain = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.TerrainGroupName);
                Transform crops = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.CropGroupName);
                Transform heroReference = root.transform.Find(
                    CozyFarmShowcaseScenePipeline
                        .HeroScaleReferenceObjectName);

                Assert.That(terrain, Is.Not.Null);
                Assert.That(crops, Is.Not.Null);
                Assert.That(heroReference, Is.Not.Null);

                foreach (Transform sample in terrain)
                {
                    Assert.That(sample.childCount, Is.Zero);
                    Assert.That(
                        sample.GetComponent<SpriteRenderer>(),
                        Is.Not.Null);
                }

                Transform cropBed = crops.Find(
                    CozyFarmShowcaseScenePipeline.CropBedObjectName);
                Assert.That(cropBed, Is.Not.Null);
                Assert.That(
                    cropBed.GetComponentsInChildren<SpriteRenderer>(true),
                    Has.Length.EqualTo(18));
                Assert.That(
                    crops.GetComponentsInChildren<Transform>(true)
                        .Any(transform => transform.name.StartsWith(
                            "soil_for_",
                            StringComparison.Ordinal)),
                    Is.False);

                Assert.That(
                    heroReference.GetComponentsInChildren<SpriteRenderer>(
                        true),
                    Has.Length.EqualTo(4));
            });
        }

        [Test]
        public void ShowcaseUsesCurrentHeroPrefabWithoutReplacingIt()
        {
            WithShowcaseScene(scene =>
            {
                GameObject root = FindRoot(
                    scene,
                    CozyFarmShowcaseScenePipeline.RootObjectName);
                Assert.That(root, Is.Not.Null);

                Transform hero = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.HeroObjectName);
                GameObject expectedPrefab =
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        PlayerPrefabAssetCatalog.PrefabAssetPath);

                Assert.That(hero, Is.Not.Null);
                Assert.That(expectedPrefab, Is.Not.Null);
                Assert.That(
                    PrefabUtility.GetCorrespondingObjectFromSource(
                        hero.gameObject),
                    Is.EqualTo(expectedPrefab));
                Assert.That(
                    hero.GetComponent<PlayerPrefabIdentity>(),
                    Is.Not.Null);
                Assert.That(hero.localScale, Is.EqualTo(Vector3.one));

                SpriteRenderer renderer =
                    hero.GetComponentInChildren<SpriteRenderer>(true);
                Assert.That(renderer, Is.Not.Null);
                Assert.That(renderer.sprite, Is.Not.Null);
            });
        }

        [Test]
        public void ShowcaseScalesOnlyHeroVisualForWorldComparison()
        {
            WithShowcaseScene(scene =>
            {
                GameObject root = FindRoot(
                    scene,
                    CozyFarmShowcaseScenePipeline.RootObjectName);
                Assert.That(root, Is.Not.Null);

                Transform hero = root.transform.Find(
                    CozyFarmShowcaseScenePipeline.HeroObjectName);
                Assert.That(hero, Is.Not.Null);
                Assert.That(hero.localScale, Is.EqualTo(Vector3.one));

                Transform visual = hero.Find(
                    PlayerSpriteAssetCatalog.SpriteVisualObjectName);
                Assert.That(visual, Is.Not.Null);
                Assert.That(
                    visual.localScale.x,
                    Is.EqualTo(
                        CozyFarmShowcaseScenePipeline.HeroVisualScale)
                        .Within(0.001f));
                Assert.That(
                    visual.localScale.y,
                    Is.EqualTo(
                        CozyFarmShowcaseScenePipeline.HeroVisualScale)
                        .Within(0.001f));
                Assert.That(visual.localScale.z, Is.EqualTo(1f));

                CapsuleCollider2D collider =
                    hero.GetComponent<CapsuleCollider2D>();
                Assert.That(collider, Is.Not.Null);
                Assert.That(
                    collider.size.x,
                    Is.EqualTo(LabSpatialCalibration.PlayerColliderWidth)
                        .Within(0.001f));
                Assert.That(
                    collider.size.y,
                    Is.EqualTo(LabSpatialCalibration.PlayerColliderHeight)
                        .Within(0.001f));
                Assert.That(
                    collider.offset.y,
                    Is.EqualTo(LabSpatialCalibration.PlayerColliderOffsetY)
                        .Within(0.001f));
            });
        }

        [Test]
        public void ShowcaseCameraUsesProjectOrthographicContract()
        {
            WithShowcaseScene(scene =>
            {
                GameObject cameraObject = FindRoot(
                    scene,
                    CozyFarmShowcaseScenePipeline.CameraObjectName);
                Assert.That(cameraObject, Is.Not.Null);

                Camera camera = cameraObject.GetComponent<Camera>();
                Assert.That(camera, Is.Not.Null);
                Assert.That(camera.orthographic, Is.True);
                Assert.That(
                    camera.orthographicSize,
                    Is.EqualTo(SpatialModel.CameraOrthographicSize)
                        .Within(0.001f));
                Assert.That(
                    cameraObject.GetComponent<ReferenceAspectCamera>(),
                    Is.Not.Null);
            });
        }

        private static void WithShowcaseScene(
            Action<Scene> assertion)
        {
            Scene scene = SceneManager.GetSceneByPath(
                CozyFarmShowcaseScenePipeline.SceneAssetPath);
            bool openedForAssertion =
                !scene.IsValid() || !scene.isLoaded;

            if (openedForAssertion)
            {
                scene = EditorSceneManager.OpenScene(
                    CozyFarmShowcaseScenePipeline.SceneAssetPath,
                    OpenSceneMode.Additive);
            }

            try
            {
                assertion(scene);
            }
            finally
            {
                if (openedForAssertion &&
                    scene.IsValid() &&
                    scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(
                        scene,
                        removeScene: true);
                }
            }
        }

        private static GameObject FindRoot(
            Scene scene,
            string objectName)
        {
            return scene.GetRootGameObjects()
                .SingleOrDefault(root => root.name == objectName);
        }
    }
}
