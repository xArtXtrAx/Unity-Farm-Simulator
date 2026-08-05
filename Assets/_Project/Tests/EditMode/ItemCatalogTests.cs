using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Domain.Items;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class ItemCatalogTests
    {
        [Test]
        public void CatalogContainsExactlyTheEightApprovedItems()
        {
            string[] ids = ItemCatalog.All
                .Select(definition => definition.Id.Value)
                .ToArray();

            Assert.That(ids, Is.EqualTo(new[]
            {
                "hoe",
                "watering-can",
                "turnip-seeds",
                "turnip",
                "carrot-seeds",
                "carrot",
                "cabbage-seeds",
                "cabbage"
            }));
        }

        [TestCase("hoe", "Azada", "AZ", ItemCategory.Tool, 1, 0, 0)]
        [TestCase("watering-can", "Regadera", "RG", ItemCategory.Tool, 1, 0, 0)]
        [TestCase("turnip-seeds", "Semillas de nabo", "SN", ItemCategory.Seed, 99, 30, 0)]
        [TestCase("turnip", "Nabo", "NB", ItemCategory.Crop, 99, 0, 60)]
        [TestCase("carrot-seeds", "Semillas de zanahoria", "SZ", ItemCategory.Seed, 99, 50, 0)]
        [TestCase("carrot", "Zanahoria", "ZN", ItemCategory.Crop, 99, 0, 100)]
        [TestCase("cabbage-seeds", "Semillas de col", "SC", ItemCategory.Seed, 99, 75, 0)]
        [TestCase("cabbage", "Col", "CL", ItemCategory.Crop, 99, 0, 150)]
        public void DefinitionMatchesApprovedSourceData(
            string itemId,
            string expectedName,
            string expectedShortLabel,
            ItemCategory expectedCategory,
            int expectedStackLimit,
            int expectedBuyPrice,
            int expectedSellPrice)
        {
            ItemDefinition definition = ItemCatalog.Get(itemId);

            Assert.That(definition.Id.Value, Is.EqualTo(itemId));
            Assert.That(definition.Name, Is.EqualTo(expectedName));
            Assert.That(definition.ShortLabel, Is.EqualTo(expectedShortLabel));
            Assert.That(definition.Category, Is.EqualTo(expectedCategory));
            Assert.That(definition.StackLimit, Is.EqualTo(expectedStackLimit));
            Assert.That(definition.BuyPrice, Is.EqualTo(ToOptionalPrice(expectedBuyPrice)));
            Assert.That(definition.SellPrice, Is.EqualTo(ToOptionalPrice(expectedSellPrice)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("unknown")]
        public void UnknownStringIdsAreRejectedExplicitly(string itemId)
        {
            Assert.That(ItemCatalog.TryGet(itemId, out _), Is.False);
            Assert.Throws<ArgumentOutOfRangeException>(() => ItemCatalog.Get(itemId));
        }

        [Test]
        public void EveryRegisteredDefinitionCanBeFoundByTypedId()
        {
            foreach (ItemDefinition definition in ItemCatalog.All)
            {
                Assert.That(ItemCatalog.TryGet(definition.Id, out ItemDefinition found), Is.True);
                Assert.That(found, Is.SameAs(definition));
            }
        }

        [Test]
        public void OnlySpringSeedsArePurchasable()
        {
            string[] purchasable = ItemCatalog.All
                .Where(definition => definition.IsPurchasable)
                .Select(definition => definition.Id.Value)
                .ToArray();

            Assert.That(purchasable, Is.EqualTo(new[]
            {
                "turnip-seeds",
                "carrot-seeds",
                "cabbage-seeds"
            }));
            Assert.That(ItemCatalog.IsPurchasable("unknown"), Is.False);
            Assert.That(ItemCatalog.IsPurchasable(null), Is.False);
        }

        [Test]
        public void OnlySpringCropsAreSellable()
        {
            string[] sellable = ItemCatalog.All
                .Where(definition => definition.IsSellable)
                .Select(definition => definition.Id.Value)
                .ToArray();

            Assert.That(sellable, Is.EqualTo(new[]
            {
                "turnip",
                "carrot",
                "cabbage"
            }));
            Assert.That(ItemCatalog.IsSellable("unknown"), Is.False);
            Assert.That(ItemCatalog.IsSellable(null), Is.False);
        }

        [Test]
        public void CatalogCollectionCannotBeMutatedExternally()
        {
            var list = (IList<ItemDefinition>)ItemCatalog.All;

            Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
            Assert.That(ItemCatalog.All, Has.Count.EqualTo(8));
        }

        [Test]
        public void CategoryEnumPreservesAllKnownSourceCategories()
        {
            Assert.That(
                Enum.GetValues(typeof(ItemCategory)).Cast<ItemCategory>(),
                Is.EqualTo(new[]
                {
                    ItemCategory.Tool,
                    ItemCategory.Seed,
                    ItemCategory.Crop,
                    ItemCategory.Material,
                    ItemCategory.Food,
                    ItemCategory.Gift
                }));
        }

        [Test]
        public void ItemIdsUseOrdinalCaseSensitiveEquality()
        {
            Assert.That(ItemId.Parse("hoe"), Is.EqualTo(ItemId.Hoe));
            Assert.That(ItemId.TryParse("HOE", out _), Is.False);
            Assert.That(default(ItemId).IsKnown, Is.False);
        }

        [Test]
        public void DefinitionRejectsUnregisteredId()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ItemDefinition(default(ItemId), "Objeto", "OB", ItemCategory.Material, 1));
        }

        [Test]
        public void DefinitionRejectsBlankName()
        {
            Assert.Throws<ArgumentException>(() =>
                new ItemDefinition(ItemId.Hoe, " ", "AZ", ItemCategory.Tool, 1));
        }

        [Test]
        public void DefinitionRejectsBlankShortLabel()
        {
            Assert.Throws<ArgumentException>(() =>
                new ItemDefinition(ItemId.Hoe, "Azada", " ", ItemCategory.Tool, 1));
        }

        [Test]
        public void DefinitionRejectsUnknownCategory()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ItemDefinition(ItemId.Hoe, "Azada", "AZ", (ItemCategory)99, 1));
        }

        [Test]
        public void DefinitionRejectsNonPositiveStackLimit()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ItemDefinition(ItemId.Hoe, "Azada", "AZ", ItemCategory.Tool, 0));
        }

        [Test]
        public void DefinitionRejectsNonPositiveBuyPrice()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ItemDefinition(
                    ItemId.TurnipSeeds,
                    "Semillas de nabo",
                    "SN",
                    ItemCategory.Seed,
                    99,
                    buyPrice: 0));
        }

        [Test]
        public void DefinitionRejectsNonPositiveSellPrice()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new ItemDefinition(
                    ItemId.Turnip,
                    "Nabo",
                    "NB",
                    ItemCategory.Crop,
                    99,
                    sellPrice: 0));
        }

        private static int? ToOptionalPrice(int price)
        {
            return price == 0 ? (int?)null : price;
        }
    }
}
