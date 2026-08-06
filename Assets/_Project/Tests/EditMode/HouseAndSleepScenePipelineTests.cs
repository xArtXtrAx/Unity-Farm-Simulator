using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Editor;
using FarmSimulator.Presentation.Player;
using FarmSimulator.Presentation.Scenes;
using FarmSimulator.Presentation.Time;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class HouseAndSleepScenePipelineTests
    {
        [Test]
        public void GeneratesBothScenesAndAddsThemToBuildSettings()
        {
            CozyFarmHouseArtPipeline.EnsureAssets();
            HouseAndSleepScenePipeline.EnsureScenes();

            Assert.That(
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    ProjectSceneNames.FarmPath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    ProjectSceneNames.HouseInteriorPath),
                Is.Not.Null);

            Assert.That(
                AssetImporter.GetAtPath(
                    ProjectSceneNames.FarmPath)
                    ?.userData,
                Is.EqualTo(
                    HouseAndSleepScenePipeline
                        .FarmImportSignature));
            Assert.That(
                AssetImporter.GetAtPath(
                    ProjectSceneNames.HouseInteriorPath)
                    ?.userData,
                Is.EqualTo(
                    HouseAndSleepScenePipeline
                        .HouseImportSignature));

            string[] enabledPaths =
                EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => scene.path)
                    .ToArray();

            Assert.That(
                enabledPaths,
                Does.Contain(ProjectSceneNames.FarmPath));
            Assert.That(
                enabledPaths,
                Does.Contain(
                    ProjectSceneNames
                        .HouseInteriorPath));
        }

        [Test]
        public void GeneratedScenesPreservePlayerRootAndContainFlowObjects()
        {
            CozyFarmHouseArtPipeline.EnsureAssets();
            HouseAndSleepScenePipeline.EnsureScenes();

            Scene farm =
                EditorSceneManager.OpenScene(
                    ProjectSceneNames.FarmPath,
                    OpenSceneMode.Additive);

            try
            {
                PlayerPrefabIdentity farmPlayer =
                    FindInScene<PlayerPrefabIdentity>(
                        farm);

                Assert.That(farmPlayer, Is.Not.Null);
                Assert.That(
                    farmPlayer.transform.localScale,
                    Is.EqualTo(Vector3.one));
                Assert.That(
                    farmPlayer.GetComponents<Collider2D>(),
                    Has.Length.EqualTo(1));
                Assert.That(
                    FindInScene<ScenePortal>(farm)
                        ?.TargetScene,
                    Is.EqualTo(
                        ProjectSceneNames
                            .HouseInterior));
            }
            finally
            {
                EditorSceneManager.CloseScene(
                    farm,
                    removeScene: true);
            }

            Scene house =
                EditorSceneManager.OpenScene(
                    ProjectSceneNames.HouseInteriorPath,
                    OpenSceneMode.Additive);

            try
            {
                PlayerPrefabIdentity housePlayer =
                    FindInScene<PlayerPrefabIdentity>(
                        house);

                Assert.That(housePlayer, Is.Not.Null);
                Assert.That(
                    housePlayer.transform.localScale,
                    Is.EqualTo(Vector3.one));
                Assert.That(
                    housePlayer.GetComponents<Collider2D>(),
                    Has.Length.EqualTo(1));
                Assert.That(
                    FindInScene<BedInteractable>(house),
                    Is.Not.Null);
                Assert.That(
                    FindInScene<ScenePortal>(house)
                        ?.TargetScene,
                    Is.EqualTo(ProjectSceneNames.Farm));
            }
            finally
            {
                EditorSceneManager.CloseScene(
                    house,
                    removeScene: true);
            }
        }

        [Test]
        public void GeneratedScenesUseCuratedCozyFarmCabinSprites()
        {
            CozyFarmHouseArtPipeline.EnsureAssets();
            HouseAndSleepScenePipeline.EnsureScenes();

            string selectedVariantId = CozyFarmHouseStyleWindow.SelectedVariantId;
            CozyFarmBuildingCatalog.HouseVariant selectedVariant;
            try
            {
                selectedVariant = CozyFarmBuildingCatalog.GetHouse(selectedVariantId);
            }
            catch (System.ArgumentException)
            {
                selectedVariant = CozyFarmBuildingCatalog.GetHouse(
                    CozyFarmBuildingCatalog.DefaultHouseId);
            }

            CozyFarmHouseExteriorUpgrader.ApplyVariant(selectedVariant.Id);

            Scene farm =
                EditorSceneManager.OpenScene(
                    ProjectSceneNames.FarmPath,
                    OpenSceneMode.Additive);

            try
            {
                string[] farmSprites = SpriteNames(farm);
                Assert.That(farmSprites, Does.Contain(selectedVariant.Id));
                Assert.That(farmSprites, Does.Contain("cozy_tree_spring"));
                Assert.That(farmSprites, Does.Contain("cozy_lamp_green"));
                Assert.That(
                    FindGameObject(farm, "House Body"),
                    Is.Null);
                Assert.That(
                    FindGameObject(farm, "Roof"),
                    Is.Null);
                Assert.That(
                    FindGameObject(farm, CozyFarmHouseExteriorUpgrader.VisualRootName),
                    Is.Not.Null);
                AssertRenderersAreUntinted(farm);
            }
            finally
            {
                EditorSceneManager.CloseScene(
                    farm,
                    removeScene: true);
            }

            Scene house =
                EditorSceneManager.OpenScene(
                    ProjectSceneNames.HouseInteriorPath,
                    OpenSceneMode.Additive);

            try
            {
                string[] houseSprites = SpriteNames(house);
                CollectionAssert.IsSubsetOf(
                    new[]
                    {
                        "cozy_wood_panel_light",
                        "cozy_wood_panel_dark",
                        "cozy_bench_light",
                        "cozy_fence_horizontal",
                        "cozy_flower_crates",
                        "cozy_crates_dark",
                        "cozy_lamp_green",
                    },
                    houseSprites);
                Assert.That(
                    FindGameObject(house, "Bed Mattress"),
                    Is.Not.Null);
                Assert.That(
                    FindGameObject(house, "Bed Pillow"),
                    Is.Not.Null);
                AssertRenderersAreUntinted(house);
            }
            finally
            {
                EditorSceneManager.CloseScene(
                    house,
                    removeScene: true);
            }
        }

        private static void AssertRenderersAreUntinted(Scene scene)
        {
            foreach (SpriteRenderer renderer in
                     ComponentsInScene<SpriteRenderer>(scene))
            {
                if (UsesIntentionalTint(renderer))
                {
                    continue;
                }

                Assert.That(
                    renderer.color,
                    Is.EqualTo(Color.white),
                    renderer.gameObject.name);
            }
        }

        private static bool UsesIntentionalTint(SpriteRenderer renderer)
        {
            string objectName = renderer.gameObject.name;
            return objectName == "Entrance Grounding Shadow" ||
                   objectName == "Soil Visual";
        }

        private static string[] SpriteNames(Scene scene)
        {
            return ComponentsInScene<SpriteRenderer>(scene)
                .Where(renderer => renderer.sprite != null)
                .Select(renderer => renderer.sprite.name)
                .Distinct()
                .ToArray();
        }

        private static GameObject FindGameObject(
            Scene scene,
            string objectName)
        {
            foreach (Transform transform in
                     ComponentsInScene<Transform>(scene))
            {
                if (transform.name == objectName)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }

        private static IEnumerable<T> ComponentsInScene<T>(Scene scene)
            where T : Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (T component in
                         root.GetComponentsInChildren<T>(
                             includeInactive: true))
                {
                    yield return component;
                }
            }
        }

        private static T FindInScene<T>(Scene scene)
            where T : Component
        {
            return ComponentsInScene<T>(scene).FirstOrDefault();
        }
    }
}
