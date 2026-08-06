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
                AssertVector2((Vector2)anchor.localPosition, definition.FootprintAnchorOffset);

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

        [Test]
        public void GeneratedHousePrefabNormalizesVisualAndFunctionalMetadataToBase()
        {
            CozyBuildingDefinition definition = CozyFarmBuildingRegistry.Get(CozyFarmBuildingCatalog.DefaultHouseId);
            GameObject prefab = CozyFarmBuildingPrefabGenerator.Generate(definition);
            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Transform visual = contents.transform.Find(
                    CozyFarmBuildingPrefabGenerator.CompositionRootName + "/" +
                    CozyFarmBuildingPrefabGenerator.VisualName);
                Assert.That(visual, Is.Not.Null);
                Assert.That(visual.localPosition, Is.EqualTo(Vector3.zero));

                Vector2 expectedPortal = CozyFarmBuildingPrefabGenerator.ToPrefabBaseSpace(
                    definition.PortalOffset,
                    definition.Baseline);
                Vector2 expectedSpawn = CozyFarmBuildingPrefabGenerator.ToPrefabBaseSpace(
                    definition.SpawnOffset,
                    definition.Baseline);
                Vector2 expectedCollider = CozyFarmBuildingPrefabGenerator.ToPrefabBaseSpace(
                    definition.ColliderOffset,
                    definition.Baseline);

                AssertVector2(
                    contents.transform.Find(CozyFarmBuildingPrefabGenerator.PortalAnchorName).localPosition,
                    expectedPortal);
                AssertVector2(
                    contents.transform.Find(CozyFarmBuildingPrefabGenerator.SpawnAnchorName).localPosition,
                    expectedSpawn);
                AssertVector2(contents.GetComponent<BoxCollider2D>().offset, expectedCollider);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
        }

        private static void AssertVector2(Vector2 actual, Vector2 expected)
        {
            Assert.That(
                (actual - expected).sqrMagnitude,
                Is.LessThan(0.0001f),
                $"Expected {expected} but was {actual}.");
        }
    }
}
