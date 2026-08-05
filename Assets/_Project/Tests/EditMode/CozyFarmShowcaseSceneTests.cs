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

                SpriteRenderer renderer =
                    hero.GetComponentInChildren<SpriteRenderer>(true);
                Assert.That(renderer, Is.Not.Null);
                Assert.That(renderer.sprite, Is.Not.Null);
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
