using System.Collections;
using FarmSimulator.Application.Scenes;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace FarmSimulator.Tests.PlayMode
{
    public sealed class BootstrapStartupTests
    {
        [UnityTest]
        public IEnumerator BootstrapTransitionsToLab()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Bootstrap,
                LoadSceneMode.Single);

            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;

            const int maximumFrames = 120;
            for (int frame = 0; frame < maximumFrames; frame++)
            {
                if (SceneManager.GetActiveScene().name == ProjectSceneNames.Lab)
                {
                    yield break;
                }

                yield return null;
            }

            Assert.Fail(
                $"Bootstrap did not transition to '{ProjectSceneNames.Lab}' " +
                $"within {maximumFrames} frames.");
        }
    }
}
