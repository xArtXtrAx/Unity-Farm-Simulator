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
using UnityEngine.Tilemaps;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class HouseAndSleepScenePipelineTests
    {
        [Test]
        public void LegacyPipelineDoesNotGenerateScenesAutomatically()
        {
            HouseAndSleepScenePipeline.EnsureScenes();

            Assert.That(
                HouseAndSleepScenePipeline.FarmImportSignature,
                Does.Contain("disabled"));
            Assert.That(
                HouseAndSleepScenePipeline.HouseImportSignature,
                Does.Contain("disabled"));
        }

        [Test]
        public void ModernRecoveryCreatesBothScenesAndBuildSettingsEntries()
        {
            ModernFarmSceneAuthoring.GenerateMissingScenesSilently();

            Assert.That(
                AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.FarmPath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.HouseInteriorPath),
                Is.Not.Null);

            string[] enabledPaths = EditorBuildSettings.scenes
                .Where(entry => entry.enabled)
                .Select(entry => entry.path)
                .ToArray();
            Assert.That(enabledPaths, Does.Contain(ProjectSceneNames.FarmPath));
            Assert.That(enabledPaths, Does.Contain(ProjectSceneNames.HouseInteriorPath));
        }

        [Test]
        public void FarmUsesModernTilemapHierarchyAndRuntimeFlowObjects()
        {
            ModernFarmSceneAuthoring.GenerateMissingScenesSilently();
            Scene farm = EditorSceneManager.OpenScene(
                ProjectSceneNames.FarmPath,
                OpenSceneMode.Additive);

            try
            {
                Assert.That(FindObject(farm, "Farm Authoring Grid"), Is.Not.Null);
                Assert.That(FindObject(farm, "Ground")?.GetComponent<Tilemap>(), Is.Not.Null);
                Assert.That(FindObject(farm, "Paths")?.GetComponent<Tilemap>(), Is.Not.Null);
                Assert.That(FindObject(farm, "Soil")?.GetComponent<Tilemap>(), Is.Not.Null);
                Assert.That(FindObject(farm, "Decoration")?.GetComponent<Tilemap>(), Is.Not.Null);
                Assert.That(FindObject(farm, "Movement Boundary"), Is.Not.Null);
                Assert.That(FindObject(farm, "Scene Authoring Bounds"), Is.Not.Null);

                PlayerPrefabIdentity player = FindComponent<PlayerPrefabIdentity>(farm);
                Assert.That(player, Is.Not.Null);
                Assert.That(player.transform.localScale, Is.EqualTo(Vector3.one));

                ScenePortal portal = FindComponent<ScenePortal>(farm);
                Assert.That(portal, Is.Not.Null);
                Assert.That(portal.TargetScene, Is.EqualTo(ProjectSceneNames.HouseInterior));
            }
            finally
            {
                EditorSceneManager.CloseScene(farm, true);
            }
        }

        [Test]
        public void HouseContainsBedPortalSpawnsAndModernAuthoringLayers()
        {
            ModernFarmSceneAuthoring.GenerateMissingScenesSilently();
            Scene house = EditorSceneManager.OpenScene(
                ProjectSceneNames.HouseInteriorPath,
                OpenSceneMode.Additive);

            try
            {
                Assert.That(FindObject(house, "House Authoring Grid"), Is.Not.Null);
                Assert.That(FindObject(house, "Ground")?.GetComponent<Tilemap>(), Is.Not.Null);
                Assert.That(FindObject(house, "Walls")?.GetComponent<Tilemap>(), Is.Not.Null);
                Assert.That(FindObject(house, "Decoration")?.GetComponent<Tilemap>(), Is.Not.Null);
                Assert.That(FindComponent<BedInteractable>(house), Is.Not.Null);

                ScenePortal portal = FindComponent<ScenePortal>(house);
                Assert.That(portal, Is.Not.Null);
                Assert.That(portal.TargetScene, Is.EqualTo(ProjectSceneNames.Farm));

                string[] spawnIds = Components<SceneSpawnPoint>(house)
                    .Select(spawn => spawn.SpawnId)
                    .ToArray();
                Assert.That(spawnIds, Does.Contain(ProjectSpawnIds.HouseEntrance));
                Assert.That(spawnIds, Does.Contain(ProjectSpawnIds.HouseBedWake));
            }
            finally
            {
                EditorSceneManager.CloseScene(house, true);
            }
        }

        private static GameObject FindObject(Scene scene, string name)
        {
            return Components<Transform>(scene)
                .FirstOrDefault(transform => transform.name == name)
                ?.gameObject;
        }

        private static T FindComponent<T>(Scene scene)
            where T : Component
        {
            return Components<T>(scene).FirstOrDefault();
        }

        private static IEnumerable<T> Components<T>(Scene scene)
            where T : Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (T component in root.GetComponentsInChildren<T>(true))
                {
                    yield return component;
                }
            }
        }
    }
}
