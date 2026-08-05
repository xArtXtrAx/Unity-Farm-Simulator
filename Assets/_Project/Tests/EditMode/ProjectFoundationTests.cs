using System.Linq;
using FarmSimulator.Application;
using FarmSimulator.Application.Display;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Application.Spatial;
using FarmSimulator.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class ProjectFoundationTests
    {
        [Test]
        public void BuildSettingsBeginWithBootstrapAndLab()
        {
            string[] enabledScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            Assert.That(enabledScenes, Has.Length.GreaterThanOrEqualTo(2));
            Assert.That(enabledScenes[0], Is.EqualTo(ProjectSceneNames.BootstrapPath));
            Assert.That(enabledScenes[1], Is.EqualTo(ProjectSceneNames.LabPath));
        }

        [Test]
        public void ProjectIdentityMatchesRepository()
        {
            ProjectIdentityConfigurator.EnsureProjectIdentity();

            Assert.That(PlayerSettings.companyName, Is.EqualTo(ProjectIdentity.CompanyName));
            Assert.That(PlayerSettings.productName, Is.EqualTo(ProjectIdentity.ProductName));
            Assert.That(
                PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Standalone),
                Is.EqualTo(ProjectIdentity.StandaloneApplicationIdentifier));
        }

        [Test]
        public void SpatialModelUsesTopDownXYWithTwoDimensionalPhysics()
        {
            Assert.That(SpatialModel.GroundPlane, Is.EqualTo("XY"));
            Assert.That(SpatialModel.DepthAxis, Is.EqualTo("Z"));
            Assert.That(SpatialModel.UsesTwoDimensionalPhysics, Is.True);
            Assert.That(SpatialModel.AllowsThreeDimensionalEffects, Is.True);
            Assert.That(SpatialModel.UsesOrthographicCamera, Is.True);
            Assert.That(SpatialModel.GridCellSize, Is.EqualTo(1f));
            Assert.That(
                SpatialModel.ReferenceCharacterHeight,
                Is.EqualTo(PixelArtDisplayModel.FarmerHeightUnits).Within(0.0001f));
            Assert.That(
                SpatialModel.ReferenceCharacterWidth,
                Is.EqualTo(PixelArtDisplayModel.FarmerWidthUnits).Within(0.0001f));
            Assert.That(SpatialModel.CameraDepth, Is.LessThan(0f));
        }

        [Test]
        public void PixelArtDisplayMatchesMigratedLogicalCanvas()
        {
            Assert.That(PixelArtDisplayModel.ReferenceWidthPixels, Is.EqualTo(960));
            Assert.That(PixelArtDisplayModel.ReferenceHeightPixels, Is.EqualTo(540));
            Assert.That(PixelArtDisplayModel.PixelsPerUnit, Is.EqualTo(64));
            Assert.That(PixelArtDisplayModel.ReferenceTilePixels, Is.EqualTo(64));
            Assert.That(
                PixelArtDisplayModel.TargetAspectRatio,
                Is.EqualTo(16f / 9f).Within(0.0001f));
            Assert.That(
                PixelArtDisplayModel.VisibleWorldWidth,
                Is.EqualTo(15f).Within(0.0001f));
            Assert.That(
                PixelArtDisplayModel.VisibleWorldHeight,
                Is.EqualTo(8.4375f).Within(0.0001f));
            Assert.That(
                PixelArtDisplayModel.CameraOrthographicSize,
                Is.EqualTo(4.21875f).Within(0.0001f));
            Assert.That(
                PixelArtDisplayModel.FarmerWidthUnits,
                Is.EqualTo(1f).Within(0.0001f));
            Assert.That(
                PixelArtDisplayModel.FarmerHeightUnits,
                Is.EqualTo(1.125f).Within(0.0001f));

            NormalizedViewport full =
                PixelArtDisplayModel.CalculateViewport(1920, 1080);
            AssertViewport(full, 0f, 0f, 1f, 1f);

            NormalizedViewport fourThree =
                PixelArtDisplayModel.CalculateViewport(1024, 768);
            AssertViewport(fourThree, 0f, 0.125f, 1f, 0.75f);

            NormalizedViewport ultrawide =
                PixelArtDisplayModel.CalculateViewport(2560, 1080);
            AssertViewport(ultrawide, 0.125f, 0f, 0.75f, 1f);
        }

        private static void AssertViewport(
            NormalizedViewport viewport,
            float expectedX,
            float expectedY,
            float expectedWidth,
            float expectedHeight)
        {
            Assert.That(viewport.X, Is.EqualTo(expectedX).Within(0.0001f));
            Assert.That(viewport.Y, Is.EqualTo(expectedY).Within(0.0001f));
            Assert.That(viewport.Width, Is.EqualTo(expectedWidth).Within(0.0001f));
            Assert.That(viewport.Height, Is.EqualTo(expectedHeight).Within(0.0001f));
        }
    }
}
