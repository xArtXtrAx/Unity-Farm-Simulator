using System.Linq;
using FarmSimulator.Application;
using FarmSimulator.Application.Scenes;
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
    }
}
