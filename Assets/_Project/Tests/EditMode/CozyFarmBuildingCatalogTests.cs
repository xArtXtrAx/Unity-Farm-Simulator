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
