using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FarmSimulator.Domain.Items;

namespace FarmSimulator.Domain.Inventory
{
    public readonly struct InventorySlot : IEquatable<InventorySlot>
    {
        public static InventorySlot Empty => default;

        public InventorySlot(ItemId itemId, int quantity)
        {
            ItemDefinition definition = ItemCatalog.Get(itemId);
            if (quantity < 1 || quantity > definition.StackLimit)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity), quantity,
                    $"Quantity for {definition.Name} must be between 1 and {definition.StackLimit}.");
            }

            ItemId = itemId;
            Quantity = quantity;
        }

        public ItemId? ItemId { get; }
        public int Quantity { get; }
        public bool IsEmpty => !ItemId.HasValue;
        public bool Equals(InventorySlot other) => Nullable.Equals(ItemId, other.ItemId) && Quantity == other.Quantity;
        public override bool Equals(object obj) => obj is InventorySlot other && Equals(other);
        public override int GetHashCode() { unchecked { return ((ItemId?.GetHashCode() ?? 0) * 397) ^ Quantity; } }
        public static bool operator ==(InventorySlot left, InventorySlot right) => left.Equals(right);
        public static bool operator !=(InventorySlot left, InventorySlot right) => !left.Equals(right);
    }

    public readonly struct InventorySlotSnapshot : IEquatable<InventorySlotSnapshot>
    {
        public InventorySlotSnapshot(string itemId, int quantity) { ItemId = itemId; Quantity = quantity; }
        public string ItemId { get; }
        public int Quantity { get; }
        public bool IsEmpty => ItemId == null;
        public bool Equals(InventorySlotSnapshot other) => string.Equals(ItemId, other.ItemId, StringComparison.Ordinal) && Quantity == other.Quantity;
        public override bool Equals(object obj) => obj is InventorySlotSnapshot other && Equals(other);
        public override int GetHashCode() { unchecked { return ((ItemId != null ? StringComparer.Ordinal.GetHashCode(ItemId) : 0) * 397) ^ Quantity; } }
        public static bool operator ==(InventorySlotSnapshot left, InventorySlotSnapshot right) => left.Equals(right);
        public static bool operator !=(InventorySlotSnapshot left, InventorySlotSnapshot right) => !left.Equals(right);
    }

    public readonly struct AddItemResult : IEquatable<AddItemResult>
    {
        public AddItemResult(ItemId itemId, int requested, int added, int remaining, bool changed)
        { ItemId = itemId; Requested = requested; Added = added; Remaining = remaining; Changed = changed; }
        public ItemId ItemId { get; }
        public int Requested { get; }
        public int Added { get; }
        public int Remaining { get; }
        public bool Changed { get; }
        public bool Equals(AddItemResult other) => ItemId == other.ItemId && Requested == other.Requested && Added == other.Added && Remaining == other.Remaining && Changed == other.Changed;
        public override bool Equals(object obj) => obj is AddItemResult other && Equals(other);
        public override int GetHashCode() { unchecked { int hashCode = ItemId.GetHashCode(); hashCode = (hashCode * 397) ^ Requested; hashCode = (hashCode * 397) ^ Added; hashCode = (hashCode * 397) ^ Remaining; hashCode = (hashCode * 397) ^ Changed.GetHashCode(); return hashCode; } }
        public static bool operator ==(AddItemResult left, AddItemResult right) => left.Equals(right);
        public static bool operator !=(AddItemResult left, AddItemResult right) => !left.Equals(right);
    }

    public sealed class InventorySnapshot
    {
        private readonly IReadOnlyList<InventorySlotSnapshot> slots;
        public InventorySnapshot(IEnumerable<InventorySlotSnapshot> slots, int selectedIndex)
        {
            if (slots == null) throw new ArgumentNullException(nameof(slots));
            InventorySlotSnapshot[] copy = slots.ToArray();
            this.slots = new ReadOnlyCollection<InventorySlotSnapshot>(copy);
            SelectedIndex = selectedIndex;
        }
        public IReadOnlyList<InventorySlotSnapshot> Slots => slots;
        public int SelectedIndex { get; }
    }

    public sealed class InventoryState
    {
        public const int DefaultSlotCount = 8;
        private readonly InventorySlot[] slots;
        private int selectedIndex;

        public InventoryState() : this(Array.Empty<InventorySlotSnapshot>(), DefaultSlotCount) { }
        public InventoryState(int slotCount) : this(Array.Empty<InventorySlotSnapshot>(), slotCount) { }
        public InventoryState(IEnumerable<InventorySlotSnapshot> initialSlots) : this(initialSlots, DefaultSlotCount) { }
        public InventoryState(IEnumerable<InventorySlotSnapshot> initialSlots, int slotCount)
        {
            if (initialSlots == null) throw new ArgumentNullException(nameof(initialSlots));
            if (slotCount < 1) throw new ArgumentOutOfRangeException(nameof(slotCount), slotCount, "Inventory must contain at least one slot.");
            slots = BuildSlots(initialSlots, slotCount);
            selectedIndex = 0;
        }

        public int SlotCount => slots.Length;
        public int SelectedIndex => selectedIndex;

        public static InventoryState CreateInitialPlayerInventory()
        {
            return new InventoryState(new[]
            {
                new InventorySlotSnapshot(ItemId.Hoe.Value, 1),
                new InventorySlotSnapshot(ItemId.WateringCan.Value, 1),
                new InventorySlotSnapshot(ItemId.TurnipSeeds.Value, 20),
                new InventorySlotSnapshot(ItemId.PotatoSeeds.Value, 20),
                new InventorySlotSnapshot(ItemId.RadishSeeds.Value, 20)
            });
        }

        public bool SelectSlot(int index) { AssertIndex(index); if (selectedIndex == index) return false; selectedIndex = index; return true; }
        public bool CycleSelection(int delta) { if (delta == 0) return false; int normalizedDelta = delta % SlotCount; int nextIndex = (selectedIndex + normalizedDelta + SlotCount) % SlotCount; return SelectSlot(nextIndex); }
        public InventorySlot GetSlot(int index) { AssertIndex(index); return slots[index]; }
        public InventorySlot[] GetSlots() => (InventorySlot[])slots.Clone();
        public InventorySlot GetSelectedSlot() => GetSlot(selectedIndex);
        public void SetSlot(int index, ItemId itemId, int quantity) { AssertIndex(index); slots[index] = new InventorySlot(itemId, quantity); }
        public void SetSlot(int index, string itemId, int quantity) { AssertIndex(index); if (itemId == null) { ClearSlot(index); return; } SetSlot(index, ItemId.Parse(itemId), quantity); }
        public void ClearSlot(int index) { AssertIndex(index); slots[index] = InventorySlot.Empty; }

        public int GetItemCapacity(ItemId itemId)
        {
            ItemDefinition definition = ItemCatalog.Get(itemId);
            int capacity = 0;
            foreach (InventorySlot slot in slots)
            {
                if (slot.IsEmpty) { capacity += definition.StackLimit; continue; }
                if (slot.ItemId.Value == itemId) capacity += definition.StackLimit - slot.Quantity;
            }
            return capacity;
        }
        public int GetItemCapacity(string itemId) => GetItemCapacity(ItemId.Parse(itemId));
        public bool CanAddItem(ItemId itemId, int quantity = 1) { AssertPositiveQuantity(quantity, "Quantity to add"); return GetItemCapacity(itemId) >= quantity; }
        public bool CanAddItem(string itemId, int quantity = 1) => CanAddItem(ItemId.Parse(itemId), quantity);

        public AddItemResult AddItem(ItemId itemId, int quantity = 1)
        {
            AssertPositiveQuantity(quantity, "Quantity to add");
            ItemDefinition definition = ItemCatalog.Get(itemId);
            if (GetItemCapacity(itemId) < quantity) return new AddItemResult(itemId, quantity, 0, quantity, false);
            int remaining = quantity;
            for (int index = 0; index < SlotCount && remaining > 0; index++)
            {
                InventorySlot slot = slots[index];
                if (slot.IsEmpty || slot.ItemId.Value != itemId || slot.Quantity >= definition.StackLimit) continue;
                int added = Math.Min(remaining, definition.StackLimit - slot.Quantity);
                slots[index] = new InventorySlot(itemId, slot.Quantity + added);
                remaining -= added;
            }
            for (int index = 0; index < SlotCount && remaining > 0; index++)
            {
                if (!slots[index].IsEmpty) continue;
                int added = Math.Min(remaining, definition.StackLimit);
                slots[index] = new InventorySlot(itemId, added);
                remaining -= added;
            }
            return new AddItemResult(itemId, quantity, quantity, 0, true);
        }
        public AddItemResult AddItem(string itemId, int quantity = 1) => AddItem(ItemId.Parse(itemId), quantity);
        public bool ConsumeSelected(int quantity = 1) => ConsumeFromSlot(selectedIndex, quantity);
        public bool ConsumeFromSlot(int index, int quantity = 1)
        {
            AssertIndex(index); AssertPositiveQuantity(quantity, "Quantity to consume");
            InventorySlot slot = slots[index];
            if (slot.IsEmpty || slot.Quantity < quantity) return false;
            int remaining = slot.Quantity - quantity;
            if (remaining == 0) ClearSlot(index); else SetSlot(index, slot.ItemId.Value, remaining);
            return true;
        }

        public InventorySlotSnapshot[] SnapshotSlots()
        {
            var snapshotSlots = new InventorySlotSnapshot[SlotCount];
            for (int index = 0; index < SlotCount; index++)
            {
                InventorySlot slot = slots[index];
                snapshotSlots[index] = slot.IsEmpty ? new InventorySlotSnapshot(null, 0) : new InventorySlotSnapshot(slot.ItemId.Value.Value, slot.Quantity);
            }
            return snapshotSlots;
        }
        public InventorySnapshot Snapshot() => new InventorySnapshot(SnapshotSlots(), selectedIndex);
        public void RestoreSlots(IEnumerable<InventorySlotSnapshot> snapshotSlots)
        {
            if (snapshotSlots == null) throw new ArgumentNullException(nameof(snapshotSlots));
            InventorySlot[] restoredSlots = BuildSlots(snapshotSlots, SlotCount);
            Array.Copy(restoredSlots, slots, SlotCount);
        }
        public void Restore(InventorySnapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            InventorySlot[] restoredSlots = BuildSlots(snapshot.Slots, SlotCount);
            int restoredSelection = Clamp(snapshot.SelectedIndex, 0, SlotCount - 1);
            Array.Copy(restoredSlots, slots, SlotCount);
            selectedIndex = restoredSelection;
        }

        private static InventorySlot[] BuildSlots(IEnumerable<InventorySlotSnapshot> sourceSlots, int slotCount)
        {
            InventorySlotSnapshot[] source = sourceSlots.Take(slotCount).ToArray();
            var restoredSlots = new InventorySlot[slotCount];
            for (int index = 0; index < source.Length; index++)
            {
                InventorySlotSnapshot sourceSlot = source[index];
                if (sourceSlot.ItemId == null) { restoredSlots[index] = InventorySlot.Empty; continue; }
                restoredSlots[index] = new InventorySlot(ItemId.Parse(sourceSlot.ItemId), sourceSlot.Quantity);
            }
            return restoredSlots;
        }
        private void AssertIndex(int index) { if (index < 0 || index >= SlotCount) throw new ArgumentOutOfRangeException(nameof(index), index, $"Inventory slot index is outside the valid range: {index}."); }
        private static void AssertPositiveQuantity(int quantity, string label) { if (quantity < 1) throw new ArgumentOutOfRangeException(nameof(quantity), quantity, $"{label} must be a positive integer."); }
        private static int Clamp(int value, int minimum, int maximum) => Math.Min(Math.Max(value, minimum), maximum);
    }
}
