using System.Collections;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Interaction;
using FarmSimulator.Presentation.Player;
using FarmSimulator.Presentation.Scenes;
using FarmSimulator.Presentation.Time;
using FarmSimulator.Presentation.UI;
using FarmSimulator.Presentation.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FarmSimulator.Tests.PlayMode
{
    public sealed class HouseAndSleepRuntimeTests
    {
        [UnityTest]
        public IEnumerator FarmLoadsHouseAndReturnsToDoorSpawn()
        {
            GameSessionRuntime.Instance.ResetSession();

            yield return LoadScene(ProjectSceneNames.Farm);
            yield return WaitForPlayerAtSpawn(
                ProjectSpawnIds.FarmStart);

            GameObject farmPlayer = FindPlayerInActiveScene();
            Assert.That(farmPlayer, Is.Not.Null);
            Assert.That(
                farmPlayer.GetComponent<
                    PlayerInteractionController>(),
                Is.Not.Null);
            Assert.That(
                Object.FindFirstObjectByType<
                    DayLabelView>(),
                Is.Not.Null);

            ScenePortal entrance =
                FindPortal(
                    ProjectSceneNames.HouseInterior);
            Assert.That(entrance, Is.Not.Null);

            entrance.Interact(farmPlayer);
            yield return WaitForScene(
                ProjectSceneNames.HouseInterior);
            yield return WaitForPlayerAtSpawn(
                ProjectSpawnIds.HouseEntrance);

            GameObject housePlayer =
                FindPlayerInActiveScene();
            Assert.That(housePlayer, Is.Not.Null);

            SceneSpawnPoint entranceSpawn =
                FindSpawn(ProjectSpawnIds.HouseEntrance);
            Assert.That(entranceSpawn, Is.Not.Null);
            Assert.That(
                Vector2.Distance(
                    housePlayer.transform.position,
                    entranceSpawn.transform.position),
                Is.LessThan(0.05f));

            ScenePortal exit =
                FindPortal(ProjectSceneNames.Farm);
            Assert.That(exit, Is.Not.Null);

            exit.Interact(housePlayer);
            yield return WaitForScene(
                ProjectSceneNames.Farm);
            yield return WaitForPlayerAtSpawn(
                ProjectSpawnIds.FarmHouseDoor);

            GameObject returnedPlayer =
                FindPlayerInActiveScene();
            SceneSpawnPoint exteriorSpawn =
                FindSpawn(ProjectSpawnIds.FarmHouseDoor);

            Assert.That(returnedPlayer, Is.Not.Null);
            Assert.That(exteriorSpawn, Is.Not.Null);
            Assert.That(
                Vector2.Distance(
                    returnedPlayer.transform.position,
                    exteriorSpawn.transform.position),
                Is.LessThan(0.05f));
        }

        [UnityTest]
        public IEnumerator SleepingAdvancesDayAndWakesByBed()
        {
            GameSessionRuntime.Instance.ResetSession();

            yield return LoadScene(
                ProjectSceneNames.HouseInterior);
            yield return WaitForPlayerAtSpawn(
                ProjectSpawnIds.HouseEntrance);

            int initialDay =
                GameSessionRuntime.Instance
                    .CurrentDate.DayOfSeason;

            BedInteractable bed =
                Object.FindFirstObjectByType<
                    BedInteractable>();
            GameObject player =
                FindPlayerInActiveScene();

            Assert.That(bed, Is.Not.Null);
            Assert.That(player, Is.Not.Null);

            bed.Interact(player);
            yield return WaitForScene(
                ProjectSceneNames.HouseInterior);
            yield return WaitForPlayerAtSpawn(
                ProjectSpawnIds.HouseBedWake);

            Assert.That(
                GameSessionRuntime.Instance
                    .CurrentDate.DayOfSeason,
                Is.EqualTo(initialDay + 1));

            GameObject awakenedPlayer =
                FindPlayerInActiveScene();
            SceneSpawnPoint wakeSpawn =
                FindSpawn(ProjectSpawnIds.HouseBedWake);

            Assert.That(awakenedPlayer, Is.Not.Null);
            Assert.That(wakeSpawn, Is.Not.Null);
            Assert.That(
                Vector2.Distance(
                    awakenedPlayer.transform.position,
                    wakeSpawn.transform.position),
                Is.LessThan(0.05f));
        }

        private static IEnumerator LoadScene(
            string sceneName)
        {
            AsyncOperation operation =
                SceneManager.LoadSceneAsync(
                    sceneName,
                    LoadSceneMode.Single);
            Assert.That(operation, Is.Not.Null);
            yield return operation;
        }

        private static IEnumerator WaitForScene(
            string sceneName)
        {
            const int maximumFrames = 180;
            for (int frame = 0;
                 frame < maximumFrames;
                 frame++)
            {
                if (SceneManager
                        .GetActiveScene()
                        .name == sceneName)
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail(
                $"Scene '{sceneName}' did not load " +
                $"within {maximumFrames} frames.");
        }

        private static IEnumerator WaitForPlayerAtSpawn(
            string spawnId)
        {
            const int maximumFrames = 180;
            for (int frame = 0;
                 frame < maximumFrames;
                 frame++)
            {
                GameObject player =
                    FindPlayerInActiveScene();
                SceneSpawnPoint spawn = FindSpawn(spawnId);

                if (player != null &&
                    spawn != null &&
                    Vector2.Distance(
                        player.transform.position,
                        spawn.transform.position) < 0.05f)
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail(
                $"Player did not reach spawn '{spawnId}' " +
                $"within {maximumFrames} frames.");
        }

        private static GameObject FindPlayerInActiveScene()
        {
            Scene activeScene =
                SceneManager.GetActiveScene();

            foreach (GameObject root in
                     activeScene.GetRootGameObjects())
            {
                PlayerPrefabIdentity identity =
                    root.GetComponentInChildren<
                        PlayerPrefabIdentity>(
                        includeInactive: true);
                if (identity != null)
                {
                    return identity.gameObject;
                }
            }

            return null;
        }

        private static ScenePortal FindPortal(
            string targetScene)
        {
            ScenePortal[] portals =
                Object.FindObjectsByType<
                    ScenePortal>(
                    FindObjectsSortMode.None);
            Scene activeScene =
                SceneManager.GetActiveScene();

            foreach (ScenePortal portal in portals)
            {
                if (portal.gameObject.scene == activeScene &&
                    portal.TargetScene == targetScene)
                {
                    return portal;
                }
            }

            return null;
        }

        private static SceneSpawnPoint FindSpawn(
            string spawnId)
        {
            SceneSpawnPoint[] points =
                Object.FindObjectsByType<
                    SceneSpawnPoint>(
                    FindObjectsSortMode.None);
            Scene activeScene =
                SceneManager.GetActiveScene();

            foreach (SceneSpawnPoint point in points)
            {
                if (point.gameObject.scene == activeScene &&
                    point.SpawnId == spawnId)
                {
                    return point;
                }
            }

            return null;
        }
    }
}
