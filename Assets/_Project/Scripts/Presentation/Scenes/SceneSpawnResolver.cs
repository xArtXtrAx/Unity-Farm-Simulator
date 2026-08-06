using System;
using FarmSimulator.Application.Player;
using FarmSimulator.Presentation.Player;
using FarmSimulator.Presentation.World;
using UnityEngine;

namespace FarmSimulator.Presentation.Scenes
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TopDownPlayerMotor))]
    public sealed class SceneSpawnResolver : MonoBehaviour
    {
        [SerializeField]
        private string defaultSpawnId;

        private void Awake()
        {
            ResolveNow();
        }

        public void Configure(string sceneDefaultSpawnId)
        {
            if (string.IsNullOrWhiteSpace(
                    sceneDefaultSpawnId))
            {
                throw new ArgumentException(
                    "Default spawn ID cannot be empty.",
                    nameof(sceneDefaultSpawnId));
            }

            defaultSpawnId = sceneDefaultSpawnId;
        }

        public bool ResolveNow()
        {
            string requested =
                GameSessionRuntime.Instance
                    .ConsumePendingSpawn();

            string spawnId =
                string.IsNullOrWhiteSpace(requested)
                    ? defaultSpawnId
                    : requested;

            SceneSpawnPoint[] spawnPoints =
                FindObjectsByType<SceneSpawnPoint>(
                    FindObjectsSortMode.None);

            foreach (SceneSpawnPoint spawnPoint in
                     spawnPoints)
            {
                if (spawnPoint.gameObject.scene !=
                        gameObject.scene ||
                    !string.Equals(
                        spawnPoint.SpawnId,
                        spawnId,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                ApplySpawn(spawnPoint);
                return true;
            }

            Debug.LogWarning(
                $"Could not find spawn '{spawnId}' " +
                $"in scene '{gameObject.scene.name}'.");
            return false;
        }

        private void ApplySpawn(
            SceneSpawnPoint spawnPoint)
        {
            Rigidbody2D body =
                GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.position =
                    spawnPoint.transform.position;
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
            else
            {
                transform.position =
                    spawnPoint.transform.position;
            }

            TopDownPlayerMotor motor =
                GetComponent<TopDownPlayerMotor>();
            motor.SetDesiredInput(
                FacingVector(spawnPoint.Facing));
            motor.Stop();
        }

        private static Vector2 FacingVector(
            FacingDirection facing)
        {
            return facing switch
            {
                FacingDirection.Up => Vector2.up,
                FacingDirection.Down => Vector2.down,
                FacingDirection.Left => Vector2.left,
                FacingDirection.Right => Vector2.right,
                _ => Vector2.down
            };
        }
    }
}
