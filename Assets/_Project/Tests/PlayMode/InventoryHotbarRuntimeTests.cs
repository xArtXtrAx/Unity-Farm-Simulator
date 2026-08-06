using System.Collections;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Inventory;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FarmSimulator.Tests.PlayMode
{
    public sealed class InventoryHotbarRuntimeTests
    {
        [UnityTest]
        public IEnumerator LabInstallsInitialEightSlotHotbar()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Lab,
                LoadSceneMode.Single);

            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;
            yield return null;

            InventoryHotbarView view =
                Object.FindFirstObjectByType<InventoryHotbarView>();
            Assert.That(view, Is.Not.Null);

            // The inventory intentionally persists between scenes and tests can
            // run in any order. Normalize the selected slot before asserting
            // the Lab hotbar contents and presentation.
            view.SelectSlot(0);

            Assert.That(view.SlotCount, Is.EqualTo(8));
            Assert.That(view.SelectedIndex, Is.Zero);
            Assert.That(view.SelectedItemName, Is.EqualTo("Azada"));
            Assert.That(view.GetSlotView(0).PlaceholderText.text, Is.EqualTo("AZ"));
            Assert.That(view.GetSlotView(1).PlaceholderText.text, Is.EqualTo("RG"));
            Assert.That(view.GetSlotView(2).Icon.enabled, Is.True);
            Assert.That(view.GetSlotView(2).QuantityText.text, Is.EqualTo("×20"));
        }

        [UnityTest]
        public IEnumerator HotbarSelectionDoesNotReplacePlayablePlayer()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Lab,
                LoadSceneMode.Single);

            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;
            yield return null;

            InventoryHotbarView view =
                Object.FindFirstObjectByType<InventoryHotbarView>();
            GameObject player = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerObjectName);

            Assert.That(view, Is.Not.Null);
            Assert.That(player, Is.Not.Null);

            // Ensure this test is independent of any selection left by a
            // previously executed PlayMode test.
            view.SelectSlot(0);
            bool changed = view.CycleSelection(1);

            Assert.That(changed, Is.True);
            Assert.That(view.SelectedIndex, Is.EqualTo(1));
            Assert.That(view.SelectedItemName, Is.EqualTo("Regadera"));
            Assert.That(
                GameObject.Find(LabSpatialCalibration.PlayablePlayerObjectName),
                Is.SameAs(player));
        }
    }
}
