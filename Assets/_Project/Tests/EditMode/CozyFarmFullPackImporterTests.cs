using System.IO;
using FarmSimulator.Editor;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyFarmFullPackImporterTests
    {
        [Test]
        public void NormalizesPurchasedArchiveRootAndSeparators()
        {
            Assert.That(
                CozyFarmFullPackImporter.NormalizeArchiveEntryPath(
                    "full version\\Buildings\\buildings.png"),
                Is.EqualTo("Buildings/buildings.png"));
            Assert.That(
                CozyFarmFullPackImporter.NormalizeArchiveEntryPath(
                    "full version/animals/heart.png"),
                Is.EqualTo("animals/heart.png"));
        }

        [Test]
        public void IgnoresDirectoriesAndRejectsTraversal()
        {
            Assert.That(
                CozyFarmFullPackImporter.NormalizeArchiveEntryPath(
                    "full version/animals/"),
                Is.Null);
            Assert.Throws<InvalidDataException>(() =>
                CozyFarmFullPackImporter.NormalizeArchiveEntryPath(
                    "full version/../outside.png"));
        }

        [Test]
        public void KeepsLicensedAssetsAndPreviewsInSeparateRoots()
        {
            Assert.That(
                CozyFarmFullPackImporter.FullAssetRoot,
                Does.StartWith("Assets/"));
            Assert.That(
                CozyFarmFullPackImporter.PreviewRoot,
                Does.Not.StartWith("Assets/"));
            Assert.That(
                CozyFarmFullPackImporter.ManifestAssetPath,
                Does.StartWith(CozyFarmFullPackImporter.FullAssetRoot));
        }
    }
}
