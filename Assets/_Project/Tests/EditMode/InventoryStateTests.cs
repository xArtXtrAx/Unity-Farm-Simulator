using System;
using System.Collections.Generic;
using FarmSimulator.Domain.Inventory;
using FarmSimulator.Domain.Items;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class InventoryStateTests
    {
        [Test]
        public void ConstructorCreatesEightEmptySlotsWithFirstSlotSelected()
        {
            var inventory = new InventoryState();

            Assert.That(inventory.SlotCount, Is.EqualTo(8));
            Assert.That(inventory.SelectedIndex, Is.Zero);
            foreach (InventorySlot slot in inventory.GetSlots())
            {
                Assert.That(slot.IsEmpty, Is.True);
                Assert.That(slot.Quantity, Is.Zero);
            }
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void ConstructorRejectsInvalidSlotCount(int slotCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new InventoryState(slotCount));
        }

        [Test]
        public void ConstructorRejectsNullInitialSlots()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InventoryState((IEnumerable<InventorySlotSnapshot>)null));
        }

        [Test]
        public void ConstructorTruncatesInitialSlotsAndCopiesInput()
        {
            var initial = new[]
            {
                Slot("hoe", 1),
                Slot("watering-can", 1),
                Slot("turnip-seeds", 3)
            };
            var inventory = new InventoryState(initial, 2);
            initial[0] = Slot("turnip-seeds", 99);

            AssertSlot(inventory, 0, "hoe", 1);
            AssertSlot(inventory, 1, "watering-can", 1);
        }

        [TestCase("unknown", 1)]
        [TestCase("turnip-seeds", 0)]
        [TestCase("turnip-seeds", 100)]
        [TestCase("hoe", 2)]
        public void ConstructorRejectsInvalidInitialSlot(string itemId, int quantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState(new[] { Slot(itemId, quantity) }));
        }

        [Test]
        public void SelectSlotReportsOnlyRealChanges()
        {
            var inventory = new InventoryState();

            Assert.That(inventory.SelectSlot(0), Is.False);
            Assert.That(inventory.SelectSlot(3), Is.True);
            Assert.That(inventory.SelectedIndex, Is.EqualTo(3));
            Assert.That(inventory.SelectSlot(3), Is.False);
        }

        [TestCase(-1)]
        [TestCase(8)]
        public void SelectSlotRejectsInvalidIndex(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().SelectSlot(index));
        }

        [Test]
        public void CycleSelectionWrapsForward()
        {
            var inventory = new InventoryState(4);
            inventory.SelectSlot(3);

            Assert.That(inventory.CycleSelection(1), Is.True);
            Assert.That(inventory.SelectedIndex, Is.Zero);
        }

        [Test]
        public void CycleSelectionWrapsBackward()
        {
            var inventory = new InventoryState(4);

            Assert.That(inventory.CycleSelection(-1), Is.True);
            Assert.That(inventory.SelectedIndex, Is.EqualTo(3));
        }

        [TestCase(5, 1)]
        [TestCase(-5, 3)]
        public void CycleSelectionNormalizesLargeDeltas(int delta, int expectedIndex)
        {
            var inventory = new InventoryState(4);

            Assert.That(inventory.CycleSelection(delta), Is.True);
            Assert.That(inventory.SelectedIndex, Is.EqualTo(expectedIndex));
        }

        [TestCase(0)]
        [TestCase(4)]
        public void CycleSelectionReportsNoChangeWhenSelectionDoesNotMove(int delta)
        {
            var inventory = new InventoryState(4);

            Assert.That(inventory.CycleSelection(delta), Is.False);
            Assert.That(inventory.SelectedIndex, Is.Zero);
        }

        [Test]
        public void GetSlotsReturnsDefensiveArrayCopy()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 4) });
            InventorySlot[] copy = inventory.GetSlots();
            copy[0] = InventorySlot.Empty;

            AssertSlot(inventory, 0, "turnip-seeds", 4);
        }

        [TestCase(-1)]
        [TestCase(8)]
        public void GetSlotRejectsInvalidIndex(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().GetSlot(index));
        }

        [Test]
        public void SetSlotWithNullIdClearsRegardlessOfQuantity()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 4) });

            inventory.SetSlot(0, null, 999);

            AssertEmptySlot(inventory, 0);
        }

        [TestCase("turnip-seeds", 0)]
        [TestCase("turnip-seeds", 100)]
        [TestCase("hoe", 2)]
        public void SetSlotRejectsQuantityOutsideItemStack(string itemId, int quantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().SetSlot(0, itemId, quantity));
        }

        [Test]
        public void CapacityIncludesPartialStacksAndEveryEmptySlot()
        {
            var inventory = new InventoryState(new[]
            {
                Slot("turnip-seeds", 90),
                Slot("hoe", 1)
            }, 4);

            Assert.That(inventory.GetItemCapacity("turnip-seeds"), Is.EqualTo(9 + 99 + 99));
        }

        [Test]
        public void CapacityIsZeroWhenSlotsContainOnlyIncompatibleItems()
        {
            var inventory = new InventoryState(new[]
            {
                Slot("hoe", 1),
                Slot("watering-can", 1)
            }, 2);

            Assert.That(inventory.GetItemCapacity("turnip-seeds"), Is.Zero);
        }

        [Test]
        public void CanAddItemRequiresCompleteRequestedCapacity()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 98) }, 1);

            Assert.That(inventory.CanAddItem("turnip-seeds", 1), Is.True);
            Assert.That(inventory.CanAddItem("turnip-seeds", 2), Is.False);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void CanAddItemRejectsInvalidQuantity(int quantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().CanAddItem("turnip-seeds", quantity));
        }

        [Test]
        public void CapacityRejectsUnknownItem()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().GetItemCapacity("unknown"));
        }

        [Test]
        public void AddItemFillsExistingStackBeforeEmptySlot()
        {
            var inventory = new InventoryState(new[]
            {
                Slot("turnip-seeds", 95),
                Slot(null, 0)
            }, 2);

            AddItemResult result = inventory.AddItem("turnip-seeds", 8);

            Assert.That(result, Is.EqualTo(new AddItemResult(ItemId.TurnipSeeds, 8, 8, 0, true)));
            AssertSlot(inventory, 0, "turnip-seeds", 99);
            AssertSlot(inventory, 1, "turnip-seeds", 4);
        }

        [Test]
        public void AddItemFillsExistingStacksInSlotOrder()
        {
            var inventory = new InventoryState(new[]
            {
                Slot("turnip-seeds", 98),
                Slot("turnip-seeds", 97),
                Slot(null, 0)
            }, 3);

            inventory.AddItem("turnip-seeds", 5);

            AssertSlot(inventory, 0, "turnip-seeds", 99);
            AssertSlot(inventory, 1, "turnip-seeds", 99);
            AssertSlot(inventory, 2, "turnip-seeds", 2);
        }

        [Test]
        public void AddItemSplitsAcrossMultipleEmptySlots()
        {
            var inventory = new InventoryState(3);

            inventory.AddItem("turnip-seeds", 200);

            AssertSlot(inventory, 0, "turnip-seeds", 99);
            AssertSlot(inventory, 1, "turnip-seeds", 99);
            AssertSlot(inventory, 2, "turnip-seeds", 2);
        }

        [Test]
        public void AddItemIsAtomicWhenCompleteQuantityDoesNotFit()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 98) }, 1);
            InventorySnapshot before = inventory.Snapshot();

            AddItemResult result = inventory.AddItem("turnip-seeds", 2);

            Assert.That(result, Is.EqualTo(new AddItemResult(ItemId.TurnipSeeds, 2, 0, 2, false)));
            AssertSnapshot(inventory.Snapshot(), before);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void AddItemRejectsInvalidQuantity(int quantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().AddItem("turnip-seeds", quantity));
        }

        [Test]
        public void AddItemRejectsUnknownItem()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().AddItem("unknown"));
        }

        [Test]
        public void ToolsOccupySeparateSlotsInsteadOfStacking()
        {
            var inventory = new InventoryState(new[] { Slot("hoe", 1) }, 2);

            Assert.That(inventory.AddItem("hoe"),
                Is.EqualTo(new AddItemResult(ItemId.Hoe, 1, 1, 0, true)));
            AssertSlot(inventory, 0, "hoe", 1);
            AssertSlot(inventory, 1, "hoe", 1);
        }

        [Test]
        public void AddingMultipleToolsIsAtomicWhenEmptySlotsAreInsufficient()
        {
            var inventory = new InventoryState(new[] { Slot("hoe", 1) }, 2);
            InventorySnapshot before = inventory.Snapshot();

            Assert.That(inventory.AddItem("hoe", 2).Changed, Is.False);
            AssertSnapshot(inventory.Snapshot(), before);
        }

        [Test]
        public void ConsumeFromSlotRemovesPartOfStack()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 5) });

            Assert.That(inventory.ConsumeFromSlot(0, 2), Is.True);
            AssertSlot(inventory, 0, "turnip-seeds", 3);
        }

        [Test]
        public void ConsumeFromSlotClearsExactStack()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 2) });

            Assert.That(inventory.ConsumeFromSlot(0, 2), Is.True);
            AssertEmptySlot(inventory, 0);
        }

        [Test]
        public void ConsumeFromSlotDoesNotMutateWhenQuantityIsUnavailable()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 2) });
            InventorySnapshot before = inventory.Snapshot();

            Assert.That(inventory.ConsumeFromSlot(0, 3), Is.False);
            AssertSnapshot(inventory.Snapshot(), before);
        }

        [Test]
        public void ConsumeFromEmptySlotReportsFalse()
        {
            var inventory = new InventoryState();

            Assert.That(inventory.ConsumeFromSlot(0), Is.False);
            AssertEmptySlot(inventory, 0);
        }

        [Test]
        public void ConsumeSelectedUsesCurrentSelection()
        {
            var inventory = new InventoryState(new[]
            {
                Slot("hoe", 1),
                Slot("turnip-seeds", 2)
            });
            inventory.SelectSlot(1);

            Assert.That(inventory.ConsumeSelected(), Is.True);
            AssertSlot(inventory, 1, "turnip-seeds", 1);
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void ConsumeRejectsInvalidQuantity(int quantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().ConsumeFromSlot(0, quantity));
        }

        [TestCase(-1)]
        [TestCase(8)]
        public void ConsumeRejectsInvalidIndex(int index)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new InventoryState().ConsumeFromSlot(index));
        }

        [Test]
        public void SnapshotCapturesSlotsAndSelection()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 2) }, 3);
            inventory.SelectSlot(2);

            InventorySnapshot snapshot = inventory.Snapshot();

            Assert.That(snapshot.SelectedIndex, Is.EqualTo(2));
            Assert.That(snapshot.Slots, Is.EqualTo(new[]
            {
                Slot("turnip-seeds", 2),
                Slot(null, 0),
                Slot(null, 0)
            }));
        }

        [Test]
        public void SnapshotSlotsCannotMutateSnapshotOrInventory()
        {
            var inventory = new InventoryState(new[] { Slot("turnip-seeds", 2) });
            InventorySnapshot snapshot = inventory.Snapshot();
            var list = (IList<InventorySlotSnapshot>)snapshot.Slots;

            Assert.Throws<NotSupportedException>(() => list[0] = Slot("turnip-seeds", 50));
            AssertSlot(inventory, 0, "turnip-seeds", 2);
            Assert.That(snapshot.Slots[0], Is.EqualTo(Slot("turnip-seeds", 2)));
        }

        [Test]
        public void RestoreRecoversSlotsAndClampsHighSelection()
        {
            var inventory = new InventoryState(3);

            inventory.Restore(new InventorySnapshot(
                new[] { Slot("turnip-seeds", 4) },
                selectedIndex: 99));

            AssertSlot(inventory, 0, "turnip-seeds", 4);
            AssertEmptySlot(inventory, 1);
            AssertEmptySlot(inventory, 2);
            Assert.That(inventory.SelectedIndex, Is.EqualTo(2));
        }

        [Test]
        public void RestoreClampsNegativeSelectionToZero()
        {
            var inventory = new InventoryState(3);
            inventory.SelectSlot(2);

            inventory.Restore(new InventorySnapshot(
                Array.Empty<InventorySlotSnapshot>(),
                selectedIndex: -10));

            Assert.That(inventory.SelectedIndex, Is.Zero);
        }

        [Test]
        public void RestoreClearsSlotsOmittedFromSnapshot()
        {
            var inventory = new InventoryState(new[]
            {
                Slot("hoe", 1),
                Slot("turnip-seeds", 5)
            }, 2);

            inventory.RestoreSlots(new[] { Slot("watering-can", 1) });

            AssertSlot(inventory, 0, "watering-can", 1);
            AssertEmptySlot(inventory, 1);
        }

        [Test]
        public void RestoreIgnoresSnapshotSlotsBeyondInventoryCapacity()
        {
            var inventory = new InventoryState(2);

            inventory.RestoreSlots(new[]
            {
                Slot("hoe", 1),
                Slot("watering-can", 1),
                Slot("turnip-seeds", 20)
            });

            AssertSlot(inventory, 0, "hoe", 1);
            AssertSlot(inventory, 1, "watering-can", 1);
        }

        [Test]
        public void RestoreSlotsPreservesCurrentSelection()
        {
            var inventory = new InventoryState(3);
            inventory.SelectSlot(2);

            inventory.RestoreSlots(new[] { Slot("turnip-seeds", 4) });

            Assert.That(inventory.SelectedIndex, Is.EqualTo(2));
        }

        [Test]
        public void InvalidUnknownItemRestoreIsAtomic()
        {
            var inventory = InventoryState.CreateInitialPlayerInventory();
            InventorySnapshot before = inventory.Snapshot();
            var invalid = new InventorySnapshot(new[]
            {
                Slot("turnip-seeds", 50),
                Slot("unknown", 1)
            }, 4);

            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Restore(invalid));
            AssertSnapshot(inventory.Snapshot(), before);
        }

        [Test]
        public void InvalidQuantityRestoreIsAtomic()
        {
            var inventory = InventoryState.CreateInitialPlayerInventory();
            InventorySnapshot before = inventory.Snapshot();
            var invalid = new InventorySnapshot(new[]
            {
                Slot("turnip-seeds", 50),
                Slot("hoe", 2)
            }, 4);

            Assert.Throws<ArgumentOutOfRangeException>(() => inventory.Restore(invalid));
            AssertSnapshot(inventory.Snapshot(), before);
        }

        [Test]
        public void RestoreRejectsNullSnapshot()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InventoryState().Restore(null));
        }

        [Test]
        public void InitialPlayerInventoryMatchesFrozenSource()
        {
            InventoryState inventory = InventoryState.CreateInitialPlayerInventory();

            Assert.That(inventory.SlotCount, Is.EqualTo(8));
            Assert.That(inventory.SelectedIndex, Is.Zero);
            AssertSlot(inventory, 0, "hoe", 1);
            AssertSlot(inventory, 1, "watering-can", 1);
            AssertSlot(inventory, 2, "turnip-seeds", 20);
            for (int index = 3; index < inventory.SlotCount; index += 1)
            {
                AssertEmptySlot(inventory, index);
            }
        }

        private static InventorySlotSnapshot Slot(string itemId, int quantity)
        {
            return new InventorySlotSnapshot(itemId, quantity);
        }

        private static void AssertSlot(
            InventoryState inventory,
            int index,
            string expectedItemId,
            int expectedQuantity)
        {
            InventorySlot slot = inventory.GetSlot(index);
            Assert.That(slot.IsEmpty, Is.EqualTo(expectedItemId == null));
            Assert.That(slot.ItemId.HasValue ? slot.ItemId.Value.Value : null,
                Is.EqualTo(expectedItemId));
            Assert.That(slot.Quantity, Is.EqualTo(expectedQuantity));
        }

        private static void AssertEmptySlot(InventoryState inventory, int index)
        {
            AssertSlot(inventory, index, null, 0);
        }

        private static void AssertSnapshot(
            InventorySnapshot actual,
            InventorySnapshot expected)
        {
            Assert.That(actual.SelectedIndex, Is.EqualTo(expected.SelectedIndex));
            Assert.That(actual.Slots, Is.EqualTo(expected.Slots));
        }
    }
}
