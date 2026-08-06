using System;
using System.Collections.Generic;
using FarmSimulator.Domain.Items;

namespace FarmSimulator.Domain.Farming
{
    public sealed class CropDefinition
    {
        public CropDefinition(
            string id,
            string name,
            ItemId seedItemId,
            ItemId harvestItemId,
            int daysToMature)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "Crop id is required.",
                    nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Crop name is required.",
                    nameof(name));
            }

            if (ItemCatalog.Get(seedItemId).Category !=
                ItemCategory.Seed)
            {
                throw new ArgumentException(
                    "Seed item must belong to the Seed category.",
                    nameof(seedItemId));
            }

            if (ItemCatalog.Get(harvestItemId).Category !=
                ItemCategory.Crop)
            {
                throw new ArgumentException(
                    "Harvest item must belong to the Crop category.",
                    nameof(harvestItemId));
            }

            if (daysToMature < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(daysToMature),
                    daysToMature,
                    "Days to mature must be at least one.");
            }

            Id = id;
            Name = name;
            SeedItemId = seedItemId;
            HarvestItemId = harvestItemId;
            DaysToMature = daysToMature;
        }

        public string Id { get; }
        public string Name { get; }
        public ItemId SeedItemId { get; }
        public ItemId HarvestItemId { get; }
        public int DaysToMature { get; }
    }

    public static class CropCatalog
    {
        public const int FinalVisualStage = 5;

        private static readonly CropDefinition[] Definitions =
        {
            new CropDefinition(
                "turnip",
                "Nabo",
                ItemId.TurnipSeeds,
                ItemId.Turnip,
                daysToMature: 2),
            new CropDefinition(
                "carrot",
                "Zanahoria",
                ItemId.CarrotSeeds,
                ItemId.Carrot,
                daysToMature: 3),
            new CropDefinition(
                "cabbage",
                "Col",
                ItemId.CabbageSeeds,
                ItemId.Cabbage,
                daysToMature: 4)
        };

        private static readonly Dictionary<ItemId, CropDefinition>
            BySeed = CreateBySeed();

        public static IReadOnlyList<CropDefinition> All =>
            Array.AsReadOnly(Definitions);

        public static bool TryGetBySeed(
            ItemId seedItemId,
            out CropDefinition definition)
        {
            return BySeed.TryGetValue(seedItemId, out definition);
        }

        public static CropDefinition GetBySeed(ItemId seedItemId)
        {
            if (TryGetBySeed(seedItemId, out CropDefinition definition))
            {
                return definition;
            }

            throw new KeyNotFoundException(
                $"No crop is registered for seed '{seedItemId.Value}'.");
        }

        private static Dictionary<ItemId, CropDefinition> CreateBySeed()
        {
            var result =
                new Dictionary<ItemId, CropDefinition>();

            foreach (CropDefinition definition in Definitions)
            {
                result.Add(definition.SeedItemId, definition);
            }

            return result;
        }
    }
}
