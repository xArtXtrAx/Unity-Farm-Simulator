using System.Collections;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Application.Spatial;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FarmSimulator.Tests.PlayMode
{
    public sealed class PlayerPrefabSortingTests
    {
        [UnityTest]
        public IEnumerator LabReplacesProvisionalPlayerWithReusablePrefabAndSortsByFeet()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Lab,
                LoadSceneMode.Single);
            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;
            yield return null;
            yield return null;

            GameObject player = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerObjectName);
            Assert.That(player, Is.Not.Null);
            Assert.That(
                player.GetComponent<PlayerPrefabIdentity>(),
                Is.Not.Null);

            Transform proxy = player.transform.Find(
                LabSpatialCalibration.PlayablePlayerVisualObjectName);
            Transform marker = player.transform.Find(
                LabSpatialCalibration.PlayerFacingMarkerObjectName);
            Assert.That(proxy, Is.Not.Null);
            Assert.That(marker, Is.Not.Null);
            Assert.That(proxy.GetComponent<Renderer>().enabled, Is.False);

            TopDownSpriteSorting sorting =
                player.GetComponent<TopDownSpriteSorting>();
            SpriteRenderer renderer =
                player.GetComponentInChildren<SpriteRenderer>(
                    includeInactive: true);
            Assert.That(sorting, Is.Not.Null);
            Assert.That(renderer, Is.Not.Null);
            Assert.That(
                renderer.sortingLayerName,
                Is.EqualTo(TopDownSortingLayers.Actors));

            player.transform.position = new Vector3(0f, 2f, -0.55f);
            sorting.Refresh();
            int higherFeetOrder = renderer.sortingOrder;
            Assert.That(
                higherFeetOrder,
                Is.EqualTo(TopDownSortingModel.OrderForFeetY(2f)));

            player.transform.position = new Vector3(0f, -2f, -0.55f);
            sorting.Refresh();
            int lowerFeetOrder = renderer.sortingOrder;
            Assert.That(
                lowerFeetOrder,
                Is.EqualTo(TopDownSortingModel.OrderForFeetY(-2f)));
            Assert.That(lowerFeetOrder, Is.GreaterThan(higherFeetOrder));

            GameObject samePlayer = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerObjectName);
            Assert.That(PlayerSpriteRuntimeInstaller.Install(), Is.True);
            Assert.That(
                GameObject.Find(LabSpatialCalibration.PlayablePlayerObjectName),
                Is.EqualTo(samePlayer));
        }
    }
}
