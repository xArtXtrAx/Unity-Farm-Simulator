using System.Collections;
using FarmSimulator.Application.Player;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Application.Spatial;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Player;
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

            ReferenceAspectCamera aspectCamera =
                sceneCamera.GetComponent<ReferenceAspectCamera>();
            Assert.That(aspectCamera, Is.Not.Null);
            Assert.That(sceneCamera.rect.width, Is.GreaterThan(0f));
            Assert.That(sceneCamera.rect.height, Is.GreaterThan(0f));

            GameObject ground = GameObject.Find(LabSpatialCalibration.GroundObjectName);
            Assert.That(ground, Is.Not.Null);
            BoxCollider2D groundCollider = ground.GetComponent<BoxCollider2D>();
            Assert.That(groundCollider, Is.Not.Null);
            Assert.That(groundCollider.isTrigger, Is.True);
            Assert.That(ground.GetComponent<Collider>(), Is.Null);
            Assert.That(
                ground.transform.localScale.x,
                Is.EqualTo(SpatialModel.GridColumns * SpatialModel.GridCellSize).Within(0.001f));
            Assert.That(
                ground.transform.localScale.y,
                Is.EqualTo(SpatialModel.GridRows * SpatialModel.GridCellSize).Within(0.001f));

            GameObject groundVisual = GameObject.Find(
                LabSpatialCalibration.GroundVisualObjectName);
            Assert.That(groundVisual, Is.Not.Null);

            Assert.That(
                calibration.GetComponentsInChildren<Collider>(includeInactive: true),
                Is.Empty,
                "Generated calibration visuals must not retain 3D colliders.");

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

        [UnityTest]
        public IEnumerator LabBuildsPlayablePlayerWithPhysicsAndUnifiedInput()
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(
                ProjectSceneNames.Lab,
                LoadSceneMode.Single);

            Assert.That(loadOperation, Is.Not.Null);
            yield return loadOperation;
            yield return null;

            GameObject player = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerObjectName);
            Assert.That(player, Is.Not.Null);

            TopDownPlayerMotor motor = player.GetComponent<TopDownPlayerMotor>();
            Assert.That(motor, Is.Not.Null);
            Assert.That(player.GetComponent<UnifiedMovementInput>(), Is.Not.Null);

            PlayerProxyFacingView facingView =
                player.GetComponent<PlayerProxyFacingView>();
            Assert.That(facingView, Is.Not.Null);

            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            Assert.That(body, Is.Not.Null);
            Assert.That(body.bodyType, Is.EqualTo(RigidbodyType2D.Dynamic));
            Assert.That(body.gravityScale, Is.Zero);
            Assert.That(
                body.constraints & RigidbodyConstraints2D.FreezeRotation,
                Is.EqualTo(RigidbodyConstraints2D.FreezeRotation));

            CapsuleCollider2D playerCollider =
                player.GetComponent<CapsuleCollider2D>();
            Assert.That(playerCollider, Is.Not.Null);
            Assert.That(playerCollider.isTrigger, Is.False);

            GameObject playerVisual = GameObject.Find(
                LabSpatialCalibration.PlayablePlayerVisualObjectName);
            Assert.That(playerVisual, Is.Not.Null);

            GameObject facingMarker = GameObject.Find(
                LabSpatialCalibration.PlayerFacingMarkerObjectName);
            Assert.That(facingMarker, Is.Not.Null);
            Assert.That(
                facingMarker.transform.localPosition,
                Is.EqualTo(PlayerProxyFacingView.CalculateMarkerLocalPosition(motor.Facing)));

            motor.SetDesiredInput(Vector2.left);
            facingView.Refresh();
            Assert.That(motor.Facing, Is.EqualTo(FacingDirection.Left));
            Assert.That(
                facingMarker.transform.localPosition.y,
                Is.EqualTo(playerVisual.transform.localPosition.y).Within(0.001f),
                "Left/right markers must be vertically centered on the player visual.");
            Assert.That(
                facingMarker.transform.localPosition,
                Is.EqualTo(PlayerProxyFacingView.CalculateMarkerLocalPosition(
                    FacingDirection.Left)));

            motor.SetDesiredInput(Vector2.right);
            facingView.Refresh();
            Assert.That(motor.Facing, Is.EqualTo(FacingDirection.Right));
            Assert.That(
                facingMarker.transform.localPosition.y,
                Is.EqualTo(playerVisual.transform.localPosition.y).Within(0.001f),
                "Left/right markers must remain vertically centered on the player visual.");
            Assert.That(
                facingMarker.transform.localPosition,
                Is.EqualTo(PlayerProxyFacingView.CalculateMarkerLocalPosition(
                    FacingDirection.Right)));

            Assert.That(
                GameObject.Find(LabSpatialCalibration.MovementBoundsObjectName)
                    ?.GetComponent<EdgeCollider2D>(),
                Is.Not.Null);
            Assert.That(
                motor.Speed,
                Is.EqualTo(PlayerMovementModel.DefaultSpeedUnitsPerSecond)
                    .Within(0.001f));
        }

        [UnityTest]
        public IEnumerator PlayerMotorNormalizesDiagonalMovementAndTracksFacing()
        {
            var player = new GameObject("Movement Test Player");
            player.transform.position = new Vector3(20f, 20f, 0f);
            TopDownPlayerMotor motor = player.AddComponent<TopDownPlayerMotor>();
            motor.Configure(2f);

            yield return null;

            Vector2 start = motor.Body.position;
            motor.SetDesiredInput(Vector2.one);
            Assert.That(motor.Facing, Is.EqualTo(FacingDirection.Up));

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            Vector2 displacement = motor.Body.position - start;
            Assert.That(displacement.x, Is.GreaterThan(0f));
            Assert.That(displacement.y, Is.GreaterThan(0f));
            Assert.That(
                Mathf.Abs(displacement.x - displacement.y),
                Is.LessThan(0.02f));

            motor.SetDesiredInput(Vector2.right);
            Assert.That(motor.Facing, Is.EqualTo(FacingDirection.Right));

            Object.Destroy(player);
        }
    }
}
