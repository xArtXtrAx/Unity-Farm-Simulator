using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Editor;
using FarmSimulator.Presentation.Buildings;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmBuildingPrefabGeneratorTests
    {
        [Test]
        public void GeneratesReusablePrefabForEveryDefinition()
        {
            IReadOnlyList<CozyBuildingDefinition> definitions = CozyFarmBuildingRegistry.Rebuild();
            int generated = CozyFarmBuildingPrefabGenerator.GenerateAll(definitions);
            Assert.That(generated, Is.EqualTo(definitions.Count));
        }

        [Test]
        public void GeneratedHousePrefabContainsAuthoritativeFootprintAnchor()
        {
            CozyBuildingDefinition definition = CozyFarmBuildingRegistry.Get(CozyFarmBuildingCatalog.DefaultHouseId);
            GameObject prefab = CozyFarmBuildingPrefabGenerator.Generate(definition);
            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Transform anchor = contents.transform.Find(CozyFarmBuildingPrefabGenerator.FootprintAnchorName);
                Assert.That(anchor, Is.Not.Null);
                Assert.That((Vector2)anchor.localPosition, Is.EqualTo(definition.FootprintAnchorOffset));
                GridBuildingFootprint footprint = contents.GetComponent<GridBuildingFootprint>();
                Assert.That(footprint, Is.Not.Null);
                Assert.That(footprint.FootprintAnchor, Is.EqualTo(anchor));
                Assert.That(footprint.OccupiedOffsets, Is.EquivalentTo(definition.FootprintOffsets));
                Assert.That(footprint.GetOccupiedCells().Count(), Is.LessThanOrEqualTo(footprint.GridSize.x * footprint.GridSize.y));
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
        }
    }
}
