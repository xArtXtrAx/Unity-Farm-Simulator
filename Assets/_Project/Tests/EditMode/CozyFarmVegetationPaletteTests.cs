using FarmSimulator.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmVegetationPaletteTests
    {
        [Test]
        public void DecorationPaletteContainsOutdoorVegetation()
        {
            CozyFarmTileCatalog.Rebuild();
            CozyFarmVegetationPaletteExtension.EnsureApplied(force: true);

            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(
                    CozyFarmVegetationPaletteExtension.SpringTreeTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(
                    CozyFarmVegetationPaletteExtension.BushRowTilePath),
                Is.Not.Null);
            Assert.That(
                AssetDatabase.LoadAssetAtPath<Tile>(
                    CozyFarmVegetationPaletteExtension.FlowerCratesTilePath),
                Is.Not.Null);

            GameObject palette = AssetDatabase.LoadAssetAtPath<GameObject>(
                CozyFarmTileCatalog.GetPalettePath(
                    CozyPaletteCategory.Decoration));
            Assert.That(palette, Is.Not.Null);

            Tilemap tilemap = palette.GetComponentInChildren<Tilemap>(true);
            Assert.That(tilemap, Is.Not.Null);
            Assert.That(tilemap.GetTile(new Vector3Int(0, -1, 0)), Is.Not.Null);
            Assert.That(tilemap.GetTile(new Vector3Int(1, -1, 0)), Is.Not.Null);
            Assert.That(tilemap.GetTile(new Vector3Int(2, -1, 0)), Is.Not.Null);
        }
    }
}
