using System;
using FarmSimulator.Application.Player;
using UnityEngine;

namespace FarmSimulator.Presentation.Scenes
{
    [DisallowMultipleComponent]
    public sealed class SceneSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private string spawnId;

        [SerializeField]
        private FacingDirection facing =
            FacingDirection.Down;

        public string SpawnId => spawnId;

        public FacingDirection Facing => facing;

        public void Configure(
            string id,
            FacingDirection spawnFacing)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "Spawn ID cannot be empty.",
                    nameof(id));
            }

            spawnId = id;
            facing = spawnFacing;
        }
    }
}
