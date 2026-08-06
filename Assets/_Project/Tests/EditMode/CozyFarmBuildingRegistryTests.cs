using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Editor;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmBuildingRegistryTests
    {
        [Test]
        public void RegistryCreatesOneDefinitionPerHouseVariant()
        {
            IReadOnlyList<CozyBuildingDefinition> definitions =
                CozyFarmBuildingRegistry.Rebuild();
            IReadOnlyList<CozyBuildingDefinition> houses = definitions
                .Where(definition =>
                    definition.Category == CozyBuildingCategory.House)
                .ToArray();

            Assert.That(
                houses.Count,
                Is.EqualTo(CozyFarmBuildingCatalog.Houses.Count));
            Assert.That(
                houses.Select(definition => definition.Id).Distinct().Count(),
                Is.EqualTo(houses.Count));
        }

        [Test]
        public void HouseDefinitionsPreservePlacementMetadata()
        {
            CozyFarmBuildingRegistry.Rebuild();
            CozyBuildingDefinition definition =
                CozyFarmBuildingRegistry.Get(
                    CozyFarmBuildingCatalog.DefaultHouseId);
            CozyFarmBuildingCatalog.HouseVariant source =
                CozyFarmBuildingCatalog.GetHouse(
                    CozyFarmBuildingCatalog.DefaultHouseId);

            Assert.That(definition.DisplayName, Is.EqualTo(source.DisplayName));
            Assert.That(definition.AtlasRect, Is.EqualTo(source.SourceRect));
            Assert.That(definition.ColliderSize, Is.EqualTo(source.ColliderSize));
            Assert.That(definition.PortalOffset, Is.EqualTo(source.PortalOffset));
            Assert.That(definition.SpawnOffset, Is.EqualTo(source.SpawnOffset));
            Assert.That(definition.SupportsInterior, Is.True);
            Assert.That(definition.GeneratedSprite, Is.Not.Null);
            Assert.That(definition.GridSize.x, Is.GreaterThan(0));
            Assert.That(definition.GridSize.y, Is.GreaterThan(0));
        }

        [Test]
        public void CatalogExposesPlannedBuildingCategories()
        {
            Assert.That(System.Enum.IsDefined(
                typeof(CozyBuildingCategory),
                CozyBuildingCategory.Barn), Is.True);
            Assert.That(System.Enum.IsDefined(
                typeof(CozyBuildingCategory),
                CozyBuildingCategory.Windmill), Is.True);
            Assert.That(System.Enum.IsDefined(
                typeof(CozyBuildingCategory),
                CozyBuildingCategory.Greenhouse), Is.True);
            Assert.That(System.Enum.IsDefined(
                typeof(CozyBuildingCategory),
                CozyBuildingCategory.Shop), Is.True);
            Assert.That(System.Enum.IsDefined(
                typeof(CozyBuildingCategory),
                CozyBuildingCategory.Market), Is.True);
        }
    }
}
