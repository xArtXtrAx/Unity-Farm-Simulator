using System;
using FarmSimulator.Domain.Items;

namespace FarmSimulator.Domain.Farming
{
    public sealed class FarmPlotState
    {
        private ItemId? seedItemId;
        private int growthDays;

        public bool IsTilled { get; private set; }
        public bool IsWatered { get; private set; }
        public ItemId? SeedItemId => seedItemId;
        public int GrowthDays => growthDays;
        public bool HasCrop => seedItemId.HasValue;

        public CropDefinition Crop =>
            HasCrop
                ? CropCatalog.GetBySeed(seedItemId.Value)
                : null;

        public bool IsMature =>
            HasCrop && growthDays >= Crop.DaysToMature;

        public int VisualStage
        {
            get
            {
                if (!HasCrop)
                {
                    return -1;
                }

                if (IsMature)
                {
                    return CropCatalog.FinalVisualStage;
                }

                return Math.Min(
                    CropCatalog.FinalVisualStage - 1,
                    growthDays *
                    CropCatalog.FinalVisualStage /
                    Crop.DaysToMature);
            }
        }

        public bool Till()
        {
            if (IsTilled)
            {
                return false;
            }

            IsTilled = true;
            return true;
        }

        public bool Water()
        {
            if (!IsTilled || IsWatered)
            {
                return false;
            }

            IsWatered = true;
            return true;
        }

        public bool Plant(ItemId candidateSeed)
        {
            if (!IsTilled || HasCrop ||
                !CropCatalog.TryGetBySeed(candidateSeed, out _))
            {
                return false;
            }

            seedItemId = candidateSeed;
            growthDays = 0;
            return true;
        }

        public bool AdvanceDay()
        {
            bool changed = IsWatered;

            if (HasCrop && IsWatered && !IsMature)
            {
                growthDays++;
                changed = true;
            }

            IsWatered = false;
            return changed;
        }

        public bool TryHarvest(out ItemId harvestItemId)
        {
            if (!IsMature)
            {
                harvestItemId = default;
                return false;
            }

            harvestItemId = Crop.HarvestItemId;
            seedItemId = null;
            growthDays = 0;
            IsWatered = false;
            return true;
        }
    }
}
