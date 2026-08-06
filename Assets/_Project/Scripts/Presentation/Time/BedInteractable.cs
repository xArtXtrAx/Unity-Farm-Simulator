using System;
using System.Collections;
using FarmSimulator.Presentation.Interaction;
using FarmSimulator.Presentation.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Presentation.Time
{
    [DisallowMultipleComponent]
    public sealed class BedInteractable :
        InteractableBehaviour
    {
        [SerializeField]
        private string wakeSpawnId =
            "HouseBedWake";

        private bool isSleeping;

        public string WakeSpawnId => wakeSpawnId;

        public override bool CanInteract(
            GameObject interactor)
        {
            return base.CanInteract(interactor) &&
                !isSleeping;
        }

        public void Configure(
            string prompt,
            string destinationWakeSpawnId,
            int interactionPriority = 200)
        {
            if (string.IsNullOrWhiteSpace(
                    destinationWakeSpawnId))
            {
                throw new ArgumentException(
                    "Wake spawn ID cannot be empty.",
                    nameof(destinationWakeSpawnId));
            }

            ConfigureInteraction(
                prompt,
                interactionPriority);
            wakeSpawnId = destinationWakeSpawnId;
        }

        public override void Interact(
            GameObject interactor)
        {
            if (!CanInteract(interactor))
            {
                return;
            }

            StartCoroutine(SleepRoutine());
        }

        private IEnumerator SleepRoutine()
        {
            isSleeping = true;

            GameSessionRuntime.Instance.AdvanceDay();
            GameSessionRuntime.Instance.SetPendingSpawn(
                wakeSpawnId);

            AsyncOperation operation =
                SceneManager.LoadSceneAsync(
                    SceneManager.GetActiveScene().name,
                    LoadSceneMode.Single);

            if (operation == null)
            {
                isSleeping = false;
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}
