using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmPilotArtTests
    {
        private const string SourceRoot =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/";

        private static readonly IReadOnlyDictionary<string, Vector2Int>
            SourceDimensions = new Dictionary<string, Vector2Int>
            {
                { SourceRoot + "items.png", new Vector2Int(160, 192) },
                { SourceRoot + "seeds.png", new Vector2Int(112, 96) },
                { SourceRoot + "tools.png", new Vector2Int(592, 64) },
                { SourceRoot + "crops.png", new Vector2Int(96, 592) },
                { SourceRoot + "tiles.png", new Vector2Int(864, 800) }
            };

        [Test]
        public void SourceSheetsKeepExpectedDimensions()
        {
            foreach (KeyValuePair<string, Vector2Int> expectation in
                     SourceDimensions)
            {
                Texture2D texture =
                    AssetDatabase.LoadAssetAtPath<Texture2D>(
                        expectation.Key);

                Assert.That(texture, Is.Not.Null, expectation.Key);
                Assert.That(
                    new Vector2Int(texture.width, texture.height),
                    Is.EqualTo(expectation.Value),
                    expectation.Key);
            }
        }

        [Test]
        public void AllPilotSheetsUsePixelArtImportSettings()
        {
            foreach (string path in SourceDimensions.Keys)
            {
                TextureImporter importer =
                    AssetImporter.GetAtPath(path) as TextureImporter;

                Assert.That(importer, Is.Not.Null, path);
                Assert.That(
                    importer.textureType,
                    Is.EqualTo(TextureImporterType.Sprite),
                    path);
                Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point), path);
                Assert.That(importer.mipmapEnabled, Is.False, path);
                Assert.That(importer.wrapMode, Is.EqualTo(TextureWrapMode.Clamp), path);
                Assert.That(
                    importer.textureCompression,
                    Is.EqualTo(TextureImporterCompression.Uncompressed),
                    path);
                Assert.That(
                    importer.spritePixelsPerUnit,
                    Is.EqualTo(16f).Within(0.001f),
                    path);
            }
        }

        [Test]
        public void MachineSheetRemainsUnslicedUntilARealToolConsumerExists()
        {
            string path = SourceRoot + "tools.png";
            TextureImporter importer =
                AssetImporter.GetAtPath(path) as TextureImporter;

            Assert.That(importer, Is.Not.Null);
            Assert.That(
                importer.spriteImportMode,
                Is.EqualTo(SpriteImportMode.Single));
            Assert.That(
                AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                    .OfType<Sprite>(),
                Is.Empty);
        }

        [Test]
        public void ItemAndSeedSheetsExposeTheSixApprovedCatalogMappings()
        {
            AssertSpriteSet(
                SourceRoot + "items.png",
                new[]
                {
                    "cozy_turnip",
                    "cozy_carrot",
                    "cozy_cabbage"
                });

            AssertSpriteSet(
                SourceRoot + "seeds.png",
                new[]
                {
                    "cozy_turnip_seeds",
                    "cozy_carrot_seeds",
                    "cozy_cabbage_seeds"
                });
        }

        [Test]
        public void CropSheetExposesThreeCropsAndSixStagesEach()
        {
            string[] expected =
                new[] { "turnip", "carrot", "cabbage" }
                    .SelectMany(crop =>
                        Enumerable.Range(0, 6)
                            .Select(stage =>
                                $"cozy_{crop}_stage_{stage}"))
                    .ToArray();

            AssertSpriteSet(SourceRoot + "crops.png", expected);
        }

        [Test]
        public void TileSheetExposesOnlyTheFourPilotTerrainTiles()
        {
            AssertSpriteSet(
                SourceRoot + "tiles.png",
                new[]
                {
                    "cozy_grass",
                    "cozy_dirt",
                    "cozy_water",
                    "cozy_tilled_soil"
                });
        }

        private static void AssertSpriteSet(
            string path,
            IReadOnlyCollection<string> expectedNames)
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(path) as TextureImporter;
            Sprite[] sprites =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                    .OfType<Sprite>()
                    .ToArray();

            Assert.That(importer, Is.Not.Null, path);
            Assert.That(
                importer.spriteImportMode,
                Is.EqualTo(SpriteImportMode.Multiple),
                path);
            Assert.That(sprites, Has.Length.EqualTo(expectedNames.Count), path);
            CollectionAssert.AreEquivalent(
                expectedNames,
                sprites.Select(sprite => sprite.name).ToArray(),
                path);
            Assert.That(
                sprites.All(sprite =>
                    Mathf.Approximately(sprite.rect.width, 16f) &&
                    Mathf.Approximately(sprite.rect.height, 16f) &&
                    Mathf.Approximately(sprite.pixelsPerUnit, 16f)),
                Is.True,
                path);
        }
    }
}
