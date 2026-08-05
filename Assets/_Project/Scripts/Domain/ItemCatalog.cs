using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FarmSimulator.Domain.Items
{
    public enum ItemCategory
    {
        Tool,
        Seed,
        Crop,
        Material,
        Food,
        Gift
    }

    public readonly struct ItemId : IEquatable<ItemId>
    {
        private readonly string value;

        private ItemId(string value)
        {
            this.value = value;
        }

        public static ItemId Hoe { get; } = new ItemId("hoe");
        public static ItemId WateringCan { get; } = new ItemId("watering-can");
        public static ItemId TurnipSeeds { get; } = new ItemId("turnip-seeds");
        public static ItemId Turnip { get; } = new ItemId("turnip");
        public static ItemId CarrotSeeds { get; } = new ItemId("carrot-seeds");
        public static ItemId Carrot { get; } = new ItemId("carrot");
        public static ItemId CabbageSeeds { get; } = new ItemId("cabbage-seeds");
        public static ItemId Cabbage { get; } = new ItemId("cabbage");

        public string Value => value ?? string.Empty;

        public bool IsKnown => TryParse(Value, out _);

        public static ItemId Parse(string value)
        {
            if (TryParse(value, out ItemId itemId))
            {
                return itemId;
            }

            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Unknown item id: {value ?? "<null>"}.");
        }

        public static bool TryParse(string value, out ItemId itemId)
        {
            switch (value)
            {
                case "hoe":
                    itemId = Hoe;
                    return true;
                case "watering-can":
                    itemId = WateringCan;
                    return true;
                case "turnip-seeds":
                    itemId = TurnipSeeds;
                    return true;
                case "turnip":
                    itemId = Turnip;
                    return true;
                case "carrot-seeds":
                    itemId = CarrotSeeds;
                    return true;
                case "carrot":
                    itemId = Carrot;
                    return true;
                case "cabbage-seeds":
                    itemId = CabbageSeeds;
                    return true;
                case "cabbage":
                    itemId = Cabbage;
                    return true;
                default:
                    itemId = default;
                    return false;
            }
        }

        public bool Equals(ItemId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.Ordinal.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(ItemId left, ItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemId left, ItemId right)
        {
            return !left.Equals(right);
        }
    }

    public sealed class ItemDefinition
    {
        public ItemDefinition(
            ItemId id,
            string name,
            string shortLabel,
            ItemCategory category,
            int stackLimit,
            int? buyPrice = null,
            int? sellPrice = null)
        {
            if (!id.IsKnown)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item id must be registered.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Item name is required.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(shortLabel))
            {
                throw new ArgumentException("Item short label is required.", nameof(shortLabel));
            }

            if (!Enum.IsDefined(typeof(ItemCategory), category))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(category),
                    category,
                    "Item category must be registered.");
            }

            if (stackLimit < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(stackLimit),
                    stackLimit,
                    "Stack limit must be at least one.");
            }

            if (buyPrice.HasValue && buyPrice.Value < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(buyPrice),
                    buyPrice,
                    "Buy price must be positive when present.");
            }

            if (sellPrice.HasValue && sellPrice.Value < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sellPrice),
                    sellPrice,
                    "Sell price must be positive when present.");
            }

            Id = id;
            Name = name;
            ShortLabel = shortLabel;
            Category = category;
            StackLimit = stackLimit;
            BuyPrice = buyPrice;
            SellPrice = sellPrice;
        }

        public ItemId Id { get; }
        public string Name { get; }
        public string ShortLabel { get; }
        public ItemCategory Category { get; }
        public int StackLimit { get; }
        public int? BuyPrice { get; }
        public int? SellPrice { get; }
        public bool IsPurchasable => BuyPrice.HasValue;
        public bool IsSellable => SellPrice.HasValue;
    }

    public static class ItemCatalog
    {
        private static readonly ItemDefinition[] Definitions =
        {
            new ItemDefinition(ItemId.Hoe, "Azada", "AZ", ItemCategory.Tool, 1),
            new ItemDefinition(ItemId.WateringCan, "Regadera", "RG", ItemCategory.Tool, 1),
            new ItemDefinition(ItemId.TurnipSeeds, "Semillas de nabo", "SN", ItemCategory.Seed, 99, buyPrice: 30),
            new ItemDefinition(ItemId.Turnip, "Nabo", "NB", ItemCategory.Crop, 99, sellPrice: 60),
            new ItemDefinition(ItemId.CarrotSeeds, "Semillas de zanahoria", "SZ", ItemCategory.Seed, 99, buyPrice: 50),
            new ItemDefinition(ItemId.Carrot, "Zanahoria", "ZN", ItemCategory.Crop, 99, sellPrice: 100),
            new ItemDefinition(ItemId.CabbageSeeds, "Semillas de col", "SC", ItemCategory.Seed, 99, buyPrice: 75),
            new ItemDefinition(ItemId.Cabbage, "Col", "CL", ItemCategory.Crop, 99, sellPrice: 150)
        };

        private static readonly IReadOnlyList<ItemDefinition> ReadOnlyDefinitions =
            Array.AsReadOnly(Definitions);

        private static readonly Dictionary<ItemId, ItemDefinition> DefinitionsById =
            CreateDefinitionsById();

        public static IReadOnlyList<ItemDefinition> All => ReadOnlyDefinitions;

        public static ItemDefinition Get(ItemId itemId)
        {
            if (TryGet(itemId, out ItemDefinition definition))
            {
                return definition;
            }

            throw new KeyNotFoundException($"Unknown item id: {itemId.Value}.");
        }

        public static ItemDefinition Get(string itemId)
        {
            return Get(ItemId.Parse(itemId));
        }

        public static bool TryGet(ItemId itemId, out ItemDefinition definition)
        {
            return itemId.IsKnown && DefinitionsById.TryGetValue(itemId, out definition);
        }

        public static bool TryGet(string itemId, out ItemDefinition definition)
        {
            if (!ItemId.TryParse(itemId, out ItemId parsed))
            {
                definition = null;
                return false;
            }

            return TryGet(parsed, out definition);
        }

        public static bool IsPurchasable(string itemId)
        {
            return TryGet(itemId, out ItemDefinition definition) && definition.IsPurchasable;
        }

        public static bool IsSellable(string itemId)
        {
            return TryGet(itemId, out ItemDefinition definition) && definition.IsSellable;
        }

        private static Dictionary<ItemId, ItemDefinition> CreateDefinitionsById()
        {
            var definitionsById = new Dictionary<ItemId, ItemDefinition>();
            foreach (ItemDefinition definition in Definitions)
            {
                definitionsById.Add(definition.Id, definition);
            }

            return definitionsById;
        }
    }
}
