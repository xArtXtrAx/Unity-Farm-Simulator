using System.Linq;
using FarmSimulator.Application.Scenes;
using NUnit.Framework;
using UnityEditor;

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
    }
}
