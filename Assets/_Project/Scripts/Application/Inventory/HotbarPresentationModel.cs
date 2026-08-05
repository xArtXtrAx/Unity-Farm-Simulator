using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FarmSimulator.Domain.Inventory;
using FarmSimulator.Domain.Items;

namespace FarmSimulator.Application.Inventory
{
    public readonly struct HotbarSlotPresentation :
        IEquatable<HotbarSlotPresentation>
    {
        public HotbarSlotPresentation(
            int index,
            string itemId,
            string name,
            string shortLabel,
            int quantity,
            bool selected)
        {
            Index = index;
            ItemId = itemId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ShortLabel = shortLabel ??
                throw new ArgumentNullException(nameof(shortLabel));
            Quantity = quantity;
            Selected = selected;
        }

        public int Index { get; }
        public string ItemId { get; }
        public string Name { get; }
        public string ShortLabel { get; }
        public int Quantity { get; }
        public bool Selected { get; }
        public bool IsEmpty => ItemId == null;

        public bool Equals(HotbarSlotPresentation other)
        {
            return Index == other.Index &&
                string.Equals(ItemId, other.ItemId, StringComparison.Ordinal) &&
                string.Equals(Name, other.Name, StringComparison.Ordinal) &&
                string.Equals(
                    ShortLabel,
                    other.ShortLabel,
                    StringComparison.Ordinal) &&
                Quantity == other.Quantity &&
                Selected == other.Selected;
        }

        public override bool Equals(object obj)
        {
            return obj is HotbarSlotPresentation other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Index;
                hashCode = (hashCode * 397) ^
                    (ItemId != null
                        ? StringComparer.Ordinal.GetHashCode(ItemId)
                        : 0);
                hashCode = (hashCode * 397) ^
                    StringComparer.Ordinal.GetHashCode(Name);
                hashCode = (hashCode * 397) ^
                    StringComparer.Ordinal.GetHashCode(ShortLabel);
                hashCode = (hashCode * 397) ^ Quantity;
                hashCode = (hashCode * 397) ^ Selected.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(
            HotbarSlotPresentation left,
            HotbarSlotPresentation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(
            HotbarSlotPresentation left,
            HotbarSlotPresentation right)
        {
            return !left.Equals(right);
        }
    }

    public sealed class HotbarPresentationModel
    {
        private readonly InventoryState state;

        public HotbarPresentationModel(InventoryState state)
        {
            this.state = state ??
                throw new ArgumentNullException(nameof(state));
        }

        public int SlotCount => state.SlotCount;
        public int SelectedIndex => state.SelectedIndex;

        public string SelectedItemName
        {
            get
            {
                InventorySlot selected = state.GetSelectedSlot();
                return selected.IsEmpty
                    ? "Manos vacías"
                    : ItemCatalog.Get(selected.ItemId.Value).Name;
            }
        }

        public bool SelectSlot(int index)
        {
            return state.SelectSlot(index);
        }

        public bool CycleSelection(int delta)
        {
            return state.CycleSelection(delta);
        }

        public IReadOnlyList<HotbarSlotPresentation> Snapshot()
        {
            var slots = new HotbarSlotPresentation[state.SlotCount];
            for (int index = 0; index < state.SlotCount; index++)
            {
                InventorySlot slot = state.GetSlot(index);
                if (slot.IsEmpty)
                {
                    slots[index] = new HotbarSlotPresentation(
                        index,
                        itemId: null,
                        name: "Vacío",
                        shortLabel: string.Empty,
                        quantity: 0,
                        selected: index == state.SelectedIndex);
                    continue;
                }

                ItemDefinition definition =
                    ItemCatalog.Get(slot.ItemId.Value);
                slots[index] = new HotbarSlotPresentation(
                    index,
                    definition.Id.Value,
                    definition.Name,
                    definition.ShortLabel,
                    slot.Quantity,
                    index == state.SelectedIndex);
            }

            return new ReadOnlyCollection<HotbarSlotPresentation>(slots);
        }
    }
}
