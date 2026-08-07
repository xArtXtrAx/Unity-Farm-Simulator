using FarmSimulator.Domain.Farming;
using FarmSimulator.Domain.Items;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class FarmingStateTests
    {
        [Test]
        public void UntilledPlotRejectsWaterAndSeeds()
        {
            var plot = new FarmPlotState();
            Assert.That(plot.Water(), Is.False);
            Assert.That(plot.Plant(ItemId.TurnipSeeds), Is.False);
            Assert.That(plot.IsTilled, Is.False);
            Assert.That(plot.HasCrop, Is.False);
        }

        [Test]
        public void TilledPlotAcceptsWaterAndOneCrop()
        {
            var plot = new FarmPlotState();
            Assert.That(plot.Till(), Is.True);
            Assert.That(plot.Till(), Is.False);
            Assert.That(plot.Water(), Is.True);
            Assert.That(plot.Water(), Is.False);
            Assert.That(plot.Plant(ItemId.TurnipSeeds), Is.True);
            Assert.That(plot.Plant(ItemId.PotatoSeeds), Is.False);
            Assert.That(plot.IsWatered, Is.True);
            Assert.That(plot.SeedItemId, Is.EqualTo(ItemId.TurnipSeeds));
        }

        [Test]
        public void CropOnlyGrowsOnWateredDaysAndSoilDries()
        {
            var plot = new FarmPlotState();
            plot.Till();
            plot.Plant(ItemId.PotatoSeeds);
            plot.AdvanceDay();
            Assert.That(plot.GrowthDays, Is.Zero);
            plot.Water();
            plot.AdvanceDay();
            Assert.That(plot.GrowthDays, Is.EqualTo(1));
            Assert.That(plot.IsWatered, Is.False);
            Assert.That(plot.IsMature, Is.False);
        }

        [Test]
        public void MatureCropHarvestsAndLeavesTilledSoil()
        {
            var plot = new FarmPlotState();
            plot.Till();
            plot.Plant(ItemId.TurnipSeeds);
            for (int day = 0; day < 2; day++)
            {
                plot.Water();
                plot.AdvanceDay();
            }
            Assert.That(plot.IsMature, Is.True);
            Assert.That(plot.VisualStage, Is.EqualTo(CropCatalog.FinalVisualStage));
            Assert.That(plot.TryHarvest(out ItemId crop), Is.True);
            Assert.That(crop, Is.EqualTo(ItemId.Turnip));
            Assert.That(plot.HasCrop, Is.False);
            Assert.That(plot.IsTilled, Is.True);
            Assert.That(plot.IsWatered, Is.False);
        }

        [Test]
        public void FarmStateKeepsIndependentPlots()
        {
            var farm = new FarmState();
            FarmPlotState first = farm.GetOrCreatePlot("plot-a");
            FarmPlotState second = farm.GetOrCreatePlot("plot-b");
            first.Till();
            first.Plant(ItemId.RadishSeeds);
            first.Water();
            second.Till();
            int changed = farm.AdvanceDay();
            Assert.That(changed, Is.EqualTo(1));
            Assert.That(first.GrowthDays, Is.EqualTo(1));
            Assert.That(second.HasCrop, Is.False);
            Assert.That(farm.PlotCount, Is.EqualTo(2));
            Assert.That(farm.GetOrCreatePlot("plot-a"), Is.SameAs(first));
        }

        [Test]
        public void CropCatalogMapsAllExistingSeeds()
        {
            Assert.That(CropCatalog.GetBySeed(ItemId.TurnipSeeds).HarvestItemId, Is.EqualTo(ItemId.Turnip));
            Assert.That(CropCatalog.GetBySeed(ItemId.PotatoSeeds).HarvestItemId, Is.EqualTo(ItemId.Potato));
            Assert.That(CropCatalog.GetBySeed(ItemId.RadishSeeds).HarvestItemId, Is.EqualTo(ItemId.Radish));
        }
    }
}
