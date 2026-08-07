using System;
using FarmSimulator.Application.Player;
using FarmSimulator.Presentation.Farming;
using FarmSimulator.Presentation.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FarmSimulator.Presentation.Interaction
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TopDownPlayerMotor))]
    public sealed class PlayerInteractionController : MonoBehaviour
    {
        public const float DefaultRange = 1.35f;
        public const float DefaultLateralTolerance = 0.9f;

        [SerializeField]
        [Min(0.1f)]
        private float interactionRange = DefaultRange;

        [SerializeField]
        [Min(0.1f)]
        private float lateralTolerance = DefaultLateralTolerance;

        private TopDownPlayerMotor motor;
        private InteractableBehaviour selected;
        private string selectedPrompt;

        public static event Action<string, bool> PromptChanged;

        public InteractableBehaviour SelectedInteractable => selected;

        private void Awake()
        {
            motor = GetComponent<TopDownPlayerMotor>();
        }

        private void Update()
        {
            RefreshSelection();

            if (ReadInteractionPressed())
                TryInteract();
        }

        private void OnDisable()
        {
            SetSelected(null);
        }

        public void Configure(float range, float maximumLateralDistance)
        {
            interactionRange = Mathf.Max(0.1f, range);
            lateralTolerance = Mathf.Max(0.1f, maximumLateralDistance);
        }

        public bool TryInteract()
        {
            RefreshSelection();

            if (selected == null || !selected.CanInteract(gameObject))
                return false;

            selected.Interact(gameObject);
            RefreshSelection();
            return true;
        }

        public void RefreshSelection()
        {
            InteractableBehaviour best = null;
            float bestScore = float.NegativeInfinity;
            Vector2 origin = transform.position;
            Vector2 facing = FacingVector(motor.Facing);
            Vector2 lateralAxis = new Vector2(-facing.y, facing.x);
            Vector2Int frontCell = CellAt(origin) + Vector2Int.RoundToInt(facing);

            InteractableBehaviour[] candidates =
                FindObjectsByType<InteractableBehaviour>(FindObjectsSortMode.None);

            foreach (InteractableBehaviour candidate in candidates)
            {
                if (candidate == null ||
                    candidate.gameObject.scene != gameObject.scene ||
                    !candidate.CanInteract(gameObject))
                {
                    continue;
                }

                if (candidate is FarmPlotBehaviour)
                {
                    if (CellAt(candidate.InteractionPosition) != frontCell)
                        continue;

                    float farmScore = candidate.Priority * 100f;
                    if (farmScore > bestScore)
                    {
                        best = candidate;
                        bestScore = farmScore;
                    }
                    continue;
                }

                Vector2 delta = candidate.InteractionPosition - origin;
                float distance = delta.magnitude;
                if (distance > interactionRange)
                    continue;

                float forwardDistance = Vector2.Dot(delta, facing);
                float sideDistance = Mathf.Abs(Vector2.Dot(delta, lateralAxis));

                if (forwardDistance < -0.05f || sideDistance > lateralTolerance)
                    continue;

                float score =
                    candidate.Priority * 100f -
                    distance -
                    sideDistance * 0.25f +
                    forwardDistance * 0.05f;

                if (score <= bestScore)
                    continue;

                best = candidate;
                bestScore = score;
            }

            SetSelected(best);
        }

        public static Vector2Int CellAt(Vector2 worldPosition) =>
            new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));

        private void SetSelected(InteractableBehaviour candidate)
        {
            string prompt = candidate?.InteractionPrompt ?? string.Empty;

            if (selected == candidate && string.Equals(selectedPrompt, prompt, StringComparison.Ordinal))
                return;

            selected = candidate;
            selectedPrompt = prompt;

            if (selected == null)
            {
                PromptChanged?.Invoke(string.Empty, false);
                return;
            }

            PromptChanged?.Invoke(selectedPrompt, true);
        }

        private static bool ReadInteractionPressed()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
                return true;

            Gamepad gamepad = Gamepad.current;
            if (gamepad == null && Gamepad.all.Count > 0)
                gamepad = Gamepad.all[0];

            return gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;
        }

        private static Vector2 FacingVector(FacingDirection direction)
        {
            return direction switch
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
