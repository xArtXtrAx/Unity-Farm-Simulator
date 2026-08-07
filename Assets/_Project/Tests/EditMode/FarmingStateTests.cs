using FarmSimulator.Domain.Farming;
using FarmSimulator.Domain.Inventory;
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

        [TestCase("turnip-seeds", 5, 2)]
        [TestCase("potato-seeds", 6, 3)]
        [TestCase("radish-seeds", 5, 4)]
        public void CropUsesItsRealVisualStageCount(string seedId, int expectedStages, int expectedDays)
        {
            CropDefinition crop = CropCatalog.GetBySeed(ItemId.Parse(seedId));
            Assert.That(crop.VisualStageCount, Is.EqualTo(expectedStages));
            Assert.That(crop.FinalVisualStage, Is.EqualTo(expectedStages - 1));
            Assert.That(crop.DaysToMature, Is.EqualTo(expectedDays));
            Assert.That(crop.GetVisualStage(0), Is.Zero);
            Assert.That(crop.GetVisualStage(expectedDays), Is.EqualTo(expectedStages - 1));
        }

        [TestCase("turnip-seeds", "turnip")]
        [TestCase("potato-seeds", "potato")]
        [TestCase("radish-seeds", "radish")]
        public void CompleteLoopHarvestsEachCropIntoInventory(string seedId, string harvestId)
        {
            ItemId seed = ItemId.Parse(seedId);
            ItemId harvest = ItemId.Parse(harvestId);
            CropDefinition crop = CropCatalog.GetBySeed(seed);
            var plot = new FarmPlotState();
            var inventory = new InventoryState(new[] { new InventorySlotSnapshot(seed.Value, 1) });

            Assert.That(plot.Till(), Is.True);
            Assert.That(plot.Plant(seed), Is.True);
            Assert.That(inventory.ConsumeSelected(), Is.True);

            for (int day = 0; day < crop.DaysToMature; day++)
            {
                Assert.That(plot.Water(), Is.True);
                Assert.That(plot.AdvanceDay(), Is.True);
            }

            Assert.That(plot.IsMature, Is.True);
            Assert.That(plot.VisualStage, Is.EqualTo(crop.FinalVisualStage));
            Assert.That(plot.TryHarvest(out ItemId harvested), Is.True);
            Assert.That(harvested, Is.EqualTo(harvest));
            Assert.That(inventory.AddItem(harvested).Changed, Is.True);
            Assert.That(inventory.GetSlot(0).ItemId, Is.EqualTo(harvest));
            Assert.That(inventory.GetSlot(0).Quantity, Is.EqualTo(1));
            Assert.That(plot.HasCrop, Is.False);
            Assert.That(plot.IsTilled, Is.True);
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
        public void CropCatalogMapsAllFinalSeeds()
        {
            Assert.That(CropCatalog.GetBySeed(ItemId.TurnipSeeds).HarvestItemId, Is.EqualTo(ItemId.Turnip));
            Assert.That(CropCatalog.GetBySeed(ItemId.PotatoSeeds).HarvestItemId, Is.EqualTo(ItemId.Potato));
            Assert.That(CropCatalog.GetBySeed(ItemId.RadishSeeds).HarvestItemId, Is.EqualTo(ItemId.Radish));
        }

        [Test]
        public void InitialHotbarContainsToolsAndAllFinalSeeds()
        {
            InventoryState inventory = InventoryState.CreateInitialPlayerInventory();
            Assert.That(inventory.GetSlot(0).ItemId, Is.EqualTo(ItemId.Hoe));
            Assert.That(inventory.GetSlot(1).ItemId, Is.EqualTo(ItemId.WateringCan));
            Assert.That(inventory.GetSlot(2).ItemId, Is.EqualTo(ItemId.TurnipSeeds));
            Assert.That(inventory.GetSlot(3).ItemId, Is.EqualTo(ItemId.PotatoSeeds));
            Assert.That(inventory.GetSlot(4).ItemId, Is.EqualTo(ItemId.RadishSeeds));
        }
    }
}
