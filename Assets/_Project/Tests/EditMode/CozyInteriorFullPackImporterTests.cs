using System.IO;
using FarmSimulator.Editor;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyInteriorFullPackImporterTests
    {
        [Test]
        public void NormalizesPurchasedArchiveRootAndSeparators()
        {
            Assert.That(
                CozyInteriorFullPackImporter.NormalizeArchiveEntryPath(
                    "interior full\\furniture\\beds.png"),
                Is.EqualTo("furniture/beds.png"));
            Assert.That(
                CozyInteriorFullPackImporter.NormalizeArchiveEntryPath(
                    "interior full/basics/doors.png"),
                Is.EqualTo("basics/doors.png"));
        }

        [Test]
        public void IgnoresDirectoriesAndRejectsTraversal()
        {
            Assert.That(
                CozyInteriorFullPackImporter.NormalizeArchiveEntryPath(
                    "interior full/furniture/"),
                Is.Null);
            Assert.Throws<InvalidDataException>(() =>
                CozyInteriorFullPackImporter.NormalizeArchiveEntryPath(
                    "interior full/../outside.png"));
        }

        [Test]
        public void DescribesReviewedPackageAndSeparateLocalRoots()
        {
            Assert.That(
                CozyInteriorFullPackImporter.ExpectedPngCount,
                Is.EqualTo(39));
            Assert.That(
                CozyInteriorFullPackImporter.ExpectedGifCount,
                Is.EqualTo(154));
            Assert.That(
                CozyInteriorFullPackImporter.ExpectedTextCount,
                Is.EqualTo(2));
            Assert.That(
                CozyInteriorFullPackImporter.FullAssetRoot,
                Does.StartWith("Assets/"));
            Assert.That(
                CozyInteriorFullPackImporter.PreviewRoot,
                Does.Not.StartWith("Assets/"));
        }

        [Test]
        public void ExposesStableCoreSheetCatalog()
        {
            Assert.That(
                CozyInteriorAssetCatalog.GridSizePixels,
                Is.EqualTo(16));
            Assert.That(
                CozyInteriorAssetCatalog.DoorCellWidthPixels,
                Is.EqualTo(48));
            Assert.That(
                CozyInteriorAssetCatalog.DoorCellHeightPixels,
                Is.EqualTo(32));
            Assert.That(
                CozyInteriorAssetCatalog.CoreSheetPaths,
                Does.Contain(CozyInteriorAssetCatalog.WallpapersPath));
            Assert.That(
                CozyInteriorAssetCatalog.CoreSheetPaths,
                Does.Contain(CozyInteriorAssetCatalog.BedsPath));
            Assert.That(
                CozyInteriorAssetCatalog.CoreSheetPaths,
                Does.Contain(CozyInteriorAssetCatalog.DecorationsPath));
        }
    }
}
