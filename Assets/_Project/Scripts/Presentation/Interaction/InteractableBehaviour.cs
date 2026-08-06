using System;
using UnityEngine;

namespace FarmSimulator.Presentation.Interaction
{
    public abstract class InteractableBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string interactionPrompt = "Interactuar";

        [SerializeField]
        private int priority;

        public virtual string InteractionPrompt => interactionPrompt;

        public int Priority => priority;

        public virtual Vector2 InteractionPosition => transform.position;

        public virtual bool CanInteract(GameObject interactor)
        {
            return interactor != null &&
                isActiveAndEnabled &&
                gameObject.activeInHierarchy;
        }

        public abstract void Interact(GameObject interactor);

        public void ConfigureInteraction(
            string prompt,
            int interactionPriority = 0)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException(
                    "Interaction prompt cannot be empty.",
                    nameof(prompt));
            }

            interactionPrompt = prompt;
            priority = interactionPriority;
        }
    }
}
