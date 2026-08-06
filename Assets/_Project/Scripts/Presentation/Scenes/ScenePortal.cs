using System;
using System.Collections;
using FarmSimulator.Presentation.Interaction;
using FarmSimulator.Presentation.World;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Presentation.Scenes
{
    [DisallowMultipleComponent]
    public sealed class ScenePortal :
        InteractableBehaviour
    {
        [SerializeField]
        private string targetScene;

        [SerializeField]
        private string targetSpawnId;

        private bool isLoading;

        public string TargetScene => targetScene;

        public string TargetSpawnId => targetSpawnId;

        public override bool CanInteract(
            GameObject interactor)
        {
            return base.CanInteract(interactor) &&
                !isLoading;
        }

        public void Configure(
            string prompt,
            string destinationScene,
            string destinationSpawnId,
            int interactionPriority = 100)
        {
            if (string.IsNullOrWhiteSpace(
                    destinationScene))
            {
                throw new ArgumentException(
                    "Destination scene cannot be empty.",
                    nameof(destinationScene));
            }

            if (string.IsNullOrWhiteSpace(
                    destinationSpawnId))
            {
                throw new ArgumentException(
                    "Destination spawn cannot be empty.",
                    nameof(destinationSpawnId));
            }

            ConfigureInteraction(
                prompt,
                interactionPriority);
            targetScene = destinationScene;
            targetSpawnId = destinationSpawnId;
        }

        public override void Interact(
            GameObject interactor)
        {
            if (!CanInteract(interactor))
            {
                return;
            }

            if (!global::UnityEngine.Application
                    .CanStreamedLevelBeLoaded(
                        targetScene))
            {
                Debug.LogError(
                    $"Scene '{targetScene}' is not " +
                    "available in Build Settings.");
                return;
            }

            StartCoroutine(LoadDestination());
        }

        private IEnumerator LoadDestination()
        {
            isLoading = true;
            GameSessionRuntime.Instance.SetPendingSpawn(
                targetSpawnId);

            AsyncOperation operation =
                SceneManager.LoadSceneAsync(
                    targetScene,
                    LoadSceneMode.Single);

            if (operation == null)
            {
                isLoading = false;
                yield break;
            }

            while (!operation.isDone)
            {
                yield return null;
            }
        }
    }
}
