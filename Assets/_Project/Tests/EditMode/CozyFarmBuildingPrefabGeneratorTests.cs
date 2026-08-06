using System.Collections.Generic;
using FarmSimulator.Editor;
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
            IReadOnlyList<CozyBuildingDefinition> definitions =
                CozyFarmBuildingRegistry.Rebuild();

            int generated =
                CozyFarmBuildingPrefabGenerator.GenerateAll(definitions);

            Assert.That(generated, Is.EqualTo(definitions.Count));
            foreach (CozyBuildingDefinition definition in definitions)
            {
                Assert.That(definition.GeneratedPrefab, Is.Not.Null, definition.Id);
                Assert.That(
                    AssetDatabase.LoadAssetAtPath<GameObject>(
                        CozyFarmBuildingPrefabGenerator.GetPrefabPath(definition)),
                    Is.Not.Null,
                    definition.Id);
            }
        }

        [Test]
        public void GeneratedHousePrefabContainsVisualColliderAndAnchors()
        {
            CozyBuildingDefinition definition =
                CozyFarmBuildingRegistry.Get(
                    CozyFarmBuildingCatalog.DefaultHouseId);
            GameObject prefab =
                CozyFarmBuildingPrefabGenerator.Generate(definition);

            string path = AssetDatabase.GetAssetPath(prefab);
            GameObject contents = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Assert.That(
                    contents.transform.Find(
                        CozyFarmBuildingPrefabGenerator.CompositionRootName + "/" +
                        CozyFarmBuildingPrefabGenerator.VisualName),
                    Is.Not.Null);
                Assert.That(contents.GetComponent<BoxCollider2D>(), Is.Not.Null);
                Assert.That(
                    contents.transform.Find(
                        CozyFarmBuildingPrefabGenerator.DoorAnchorName),
                    Is.Not.Null);
                Assert.That(
                    contents.transform.Find(
                        CozyFarmBuildingPrefabGenerator.PortalAnchorName),
                    Is.Not.Null);
                Assert.That(
                    contents.transform.Find(
                        CozyFarmBuildingPrefabGenerator.SpawnAnchorName),
                    Is.Not.Null);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }
        }
    }
}
