using System;
using FarmSimulator.Presentation.Interaction;
using UnityEngine;
using UnityEngine.UI;

namespace FarmSimulator.Presentation.UI
{
    [DisallowMultipleComponent]
    public sealed class InteractionPromptView :
        MonoBehaviour
    {
        [SerializeField]
        private GameObject promptRoot;

        [SerializeField]
        private Text promptText;

        public Text PromptText => promptText;

        private void OnEnable()
        {
            PlayerInteractionController.PromptChanged +=
                HandlePromptChanged;

            if (promptRoot != null)
            {
                promptRoot.SetActive(false);
            }
        }

        private void OnDisable()
        {
            PlayerInteractionController.PromptChanged -=
                HandlePromptChanged;
        }

        public void Configure(Text label)
        {
            promptText = label ??
                throw new ArgumentNullException(
                    nameof(label));
            promptRoot = label.gameObject;
            promptRoot.SetActive(false);
        }

        private void HandlePromptChanged(
            string message,
            bool visible)
        {
            if (promptText != null)
            {
                promptText.text =
                    visible
                        ? $"[E / X] {message}"
                        : string.Empty;
            }

            if (promptRoot != null)
            {
                promptRoot.SetActive(visible);
            }
        }
    }
}
