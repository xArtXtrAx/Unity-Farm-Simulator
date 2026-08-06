using System.Collections.Generic;
using FarmSimulator.Editor;
using NUnit.Framework;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmBuildingCatalogTests
    {
        [Test]
        public void StarterHousePresetTargetsAValidAtlasRegion()
        {
            RectInt rect = CozyFarmBuildingCatalog.StarterHouseSource;

            Assert.That(rect.width, Is.GreaterThan(0));
            Assert.That(rect.height, Is.GreaterThan(0));
            Assert.That(rect.xMin, Is.GreaterThanOrEqualTo(0));
            Assert.That(rect.yMin, Is.GreaterThanOrEqualTo(0));
            Assert.That(rect.xMax, Is.LessThanOrEqualTo(1503));
            Assert.That(rect.yMax, Is.LessThanOrEqualTo(1072));
        }

        [Test]
        public void StarterHousePresetKeepsTheValidatedFullPackCoordinates()
        {
            Assert.That(
                CozyFarmBuildingCatalog.StarterHouseSource,
                Is.EqualTo(new RectInt(681, 548, 68, 86)));
        }

        [Test]
        public void CatalogContainsUniqueReusableHouseVariants()
        {
            var ids = new HashSet<string>();
            var paths = new HashSet<string>();

            Assert.That(CozyFarmBuildingCatalog.Houses.Count, Is.GreaterThanOrEqualTo(5));
            foreach (CozyFarmBuildingCatalog.HouseVariant variant in
                CozyFarmBuildingCatalog.Houses)
            {
                Assert.That(ids.Add(variant.Id), Is.True, $"Duplicate id: {variant.Id}");
                Assert.That(paths.Add(variant.GeneratedPath), Is.True,
                    $"Duplicate generated path: {variant.GeneratedPath}");
                Assert.That(variant.SourceRect.width, Is.GreaterThan(0));
                Assert.That(variant.SourceRect.height, Is.GreaterThan(0));
                Assert.That(variant.SourceRect.xMin, Is.GreaterThanOrEqualTo(0));
                Assert.That(variant.SourceRect.yMin, Is.GreaterThanOrEqualTo(0));
                Assert.That(variant.SourceRect.xMax, Is.LessThanOrEqualTo(1503));
                Assert.That(variant.SourceRect.yMax, Is.LessThanOrEqualTo(1072));
                Assert.That(variant.MaximumWidth, Is.GreaterThan(0f));
                Assert.That(variant.MaximumHeight, Is.GreaterThan(0f));
                Assert.That(variant.ColliderSize.x, Is.GreaterThan(0f));
                Assert.That(variant.ColliderSize.y, Is.GreaterThan(0f));
            }
        }

        [Test]
        public void DefaultHouseCanBeResolvedByStableId()
        {
            CozyFarmBuildingCatalog.HouseVariant variant =
                CozyFarmBuildingCatalog.GetHouse(
                    CozyFarmBuildingCatalog.DefaultHouseId);

            Assert.That(variant.Id, Is.EqualTo(CozyFarmBuildingCatalog.DefaultHouseId));
            Assert.That(variant.GeneratedPath, Is.EqualTo(CozyFarmBuildingCatalog.StarterHousePath));
        }

        [Test]
        public void GeneratedStarterHouseLivesOutsideThirdPartySource()
        {
            Assert.That(
                CozyFarmBuildingCatalog.StarterHousePath,
                Does.StartWith("Assets/_Project/Art/Generated/"));
            Assert.That(
                CozyFarmBuildingCatalog.StarterHousePath,
                Does.Not.Contain("ThirdParty"));
        }
    }
}
