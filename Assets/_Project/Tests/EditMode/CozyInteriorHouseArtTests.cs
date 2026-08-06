using System.Linq;
using FarmSimulator.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyInteriorHouseArtTests
    {
        [Test]
        public void CuratesHouseSpritesFromImportedInteriorSheets()
        {
            CozyInteriorHouseArtPipeline.EnsureAssets();

            string[] expected =
            {
                "cozy_interior_wall_cream",
                "cozy_interior_floor_wood",
                "cozy_interior_door_cream",
                "cozy_interior_bed_cream",
                "cozy_interior_rug_warm",
            };

            string[] actual =
                CozyInteriorHouseArtPipeline.LoadSprites()
                    .Keys
                    .OrderBy(name => name)
                    .ToArray();

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void CuratedInteriorSpritesUsePixelArtSettings()
        {
            string[] paths =
            {
                CozyInteriorHouseArtPipeline.WallpapersPath,
                CozyInteriorHouseArtPipeline.DoorsPath,
                CozyInteriorHouseArtPipeline.BedsPath,
                CozyInteriorHouseArtPipeline.RugsPath,
            };

            CozyInteriorHouseArtPipeline.EnsureAssets();

            foreach (string path in paths)
            {
                TextureImporter importer =
                    AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer, Is.Not.Null);
                Assert.That(
                    importer.spriteImportMode,
                    Is.EqualTo(SpriteImportMode.Multiple));
                Assert.That(importer.spritePixelsPerUnit, Is.EqualTo(16f));
                Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point));
                Assert.That(importer.mipmapEnabled, Is.False);
            }
        }
    }
}
