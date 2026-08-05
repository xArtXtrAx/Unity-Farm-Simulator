using System.Collections;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Application.Spatial;
using FarmSimulator.Presentation.Calibration;
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

        [UnityTest]
        public IEnumerator LabBuildsFrontFacingOrthographicXYCalibration()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Lab,
                LoadSceneMode.Single);

            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;
            yield return null;

            LabSpatialCalibration calibration =
                Object.FindFirstObjectByType<LabSpatialCalibration>();
            Assert.That(calibration, Is.Not.Null);

            Camera sceneCamera = Camera.main;
            Assert.That(sceneCamera, Is.Not.Null);
            Assert.That(sceneCamera.orthographic, Is.True);
            Assert.That(
                sceneCamera.orthographicSize,
                Is.EqualTo(SpatialModel.CameraOrthographicSize).Within(0.001f));
            Assert.That(
                Vector3.Distance(
                    sceneCamera.transform.position,
                    new Vector3(0f, 0f, SpatialModel.CameraDepth)),
                Is.LessThan(0.001f));
            Assert.That(
                Vector3.Dot(sceneCamera.transform.forward, Vector3.forward),
                Is.GreaterThan(0.999f));

            GameObject ground = GameObject.Find(LabSpatialCalibration.GroundObjectName);
            Assert.That(ground, Is.Not.Null);
            Assert.That(ground.GetComponent<BoxCollider2D>(), Is.Not.Null);
            Assert.That(ground.GetComponent<BoxCollider>(), Is.Null);
            Assert.That(
                ground.transform.localScale.x,
                Is.EqualTo(SpatialModel.GridColumns * SpatialModel.GridCellSize).Within(0.001f));
            Assert.That(
                ground.transform.localScale.y,
                Is.EqualTo(SpatialModel.GridRows * SpatialModel.GridCellSize).Within(0.001f));

            GameObject spriteProxy = GameObject.Find(LabSpatialCalibration.SpriteProxyObjectName);
            Assert.That(spriteProxy, Is.Not.Null);
            Assert.That(
                spriteProxy.transform.localScale.y,
                Is.EqualTo(SpatialModel.ReferenceCharacterHeight).Within(0.001f));
            Assert.That(
                spriteProxy.transform.position.z,
                Is.LessThan(ground.transform.position.z));

            GameObject depthStackFront = GameObject.Find(
                LabSpatialCalibration.DepthStackObjectName);
            Assert.That(depthStackFront, Is.Not.Null);
        }
    }
}
