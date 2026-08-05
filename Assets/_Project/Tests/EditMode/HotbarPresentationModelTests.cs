using System;
using System.Collections.Generic;
using FarmSimulator.Application.Inventory;
using FarmSimulator.Domain.Inventory;
using FarmSimulator.Domain.Items;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class HotbarPresentationModelTests
    {
        [Test]
        public void ConstructorRejectsNullInventory()
        {
            Assert.That(
                () => new HotbarPresentationModel(null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void InitialPlayerInventoryProducesEightSlots()
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            IReadOnlyList<HotbarSlotPresentation> snapshot =
                model.Snapshot();

            Assert.That(model.SlotCount, Is.EqualTo(8));
            Assert.That(snapshot, Has.Count.EqualTo(8));
            Assert.That(model.SelectedIndex, Is.Zero);
        }

        [TestCase(0, "hoe", "Azada", "AZ", 1, true)]
        [TestCase(1, "watering-can", "Regadera", "RG", 1, false)]
        [TestCase(2, "turnip-seeds", "Semillas de nabo", "SN", 20, false)]
        public void InitialOccupiedSlotsMapCatalogPresentation(
            int index,
            string expectedItemId,
            string expectedName,
            string expectedShortLabel,
            int expectedQuantity,
            bool expectedSelected)
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            HotbarSlotPresentation slot = model.Snapshot()[index];

            Assert.That(slot.Index, Is.EqualTo(index));
            Assert.That(slot.ItemId, Is.EqualTo(expectedItemId));
            Assert.That(slot.Name, Is.EqualTo(expectedName));
            Assert.That(slot.ShortLabel, Is.EqualTo(expectedShortLabel));
            Assert.That(slot.Quantity, Is.EqualTo(expectedQuantity));
            Assert.That(slot.Selected, Is.EqualTo(expectedSelected));
            Assert.That(slot.IsEmpty, Is.False);
        }

        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public void InitialEmptySlotsUseExplicitEmptyPresentation(int index)
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            HotbarSlotPresentation slot = model.Snapshot()[index];

            Assert.That(slot.Index, Is.EqualTo(index));
            Assert.That(slot.ItemId, Is.Null);
            Assert.That(slot.Name, Is.EqualTo("Vacío"));
            Assert.That(slot.ShortLabel, Is.Empty);
            Assert.That(slot.Quantity, Is.Zero);
            Assert.That(slot.Selected, Is.False);
            Assert.That(slot.IsEmpty, Is.True);
        }

        [Test]
        public void SelectedItemNameUsesCatalogName()
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            Assert.That(model.SelectedItemName, Is.EqualTo("Azada"));

            model.SelectSlot(2);

            Assert.That(
                model.SelectedItemName,
                Is.EqualTo("Semillas de nabo"));
        }

        [Test]
        public void EmptySelectionUsesHandsEmptyLabel()
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            model.SelectSlot(7);

            Assert.That(model.SelectedItemName, Is.EqualTo("Manos vacías"));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(7)]
        public void DirectSelectionMarksExactlyOneSlot(int index)
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            bool changed = model.SelectSlot(index);
            IReadOnlyList<HotbarSlotPresentation> snapshot =
                model.Snapshot();

            Assert.That(changed, Is.True);
            Assert.That(model.SelectedIndex, Is.EqualTo(index));
            for (int slotIndex = 0;
                 slotIndex < snapshot.Count;
                 slotIndex++)
            {
                Assert.That(
                    snapshot[slotIndex].Selected,
                    Is.EqualTo(slotIndex == index));
            }
        }

        [Test]
        public void SelectingCurrentSlotReportsNoChange()
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            Assert.That(model.SelectSlot(0), Is.False);
        }

        [TestCase(-1, 7)]
        [TestCase(1, 1)]
        [TestCase(8, 0)]
        [TestCase(9, 1)]
        public void CycleSelectionWrapsThroughAllSlots(
            int delta,
            int expectedIndex)
        {
            var model = new HotbarPresentationModel(
                InventoryState.CreateInitialPlayerInventory());

            bool changed = model.CycleSelection(delta);

            Assert.That(changed, Is.EqualTo(expectedIndex != 0));
            Assert.That(model.SelectedIndex, Is.EqualTo(expectedIndex));
        }

        [Test]
        public void SnapshotIsDefensiveAgainstLaterInventoryChanges()
        {
            InventoryState inventory =
                InventoryState.CreateInitialPlayerInventory();
            var model = new HotbarPresentationModel(inventory);
            IReadOnlyList<HotbarSlotPresentation> before =
                model.Snapshot();

            inventory.SetSlot(0, ItemId.Cabbage, 12);
            IReadOnlyList<HotbarSlotPresentation> after =
                model.Snapshot();

            Assert.That(before[0].ItemId, Is.EqualTo("hoe"));
            Assert.That(before[0].Quantity, Is.EqualTo(1));
            Assert.That(after[0].ItemId, Is.EqualTo("cabbage"));
            Assert.That(after[0].Quantity, Is.EqualTo(12));
        }
    }
}
