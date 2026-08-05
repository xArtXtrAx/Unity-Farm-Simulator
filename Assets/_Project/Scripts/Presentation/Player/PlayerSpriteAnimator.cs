using FarmSimulator.Application.Player;
using UnityEngine;

namespace FarmSimulator.Presentation.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TopDownPlayerMotor))]
    public sealed class PlayerSpriteAnimator : MonoBehaviour
    {
        private const string BaseLayerPrefix = "Base Layer.";

        private TopDownPlayerMotor motor;

        [SerializeField]
        private Animator animator;

        private int currentStateHash;

        public PlayerAnimationState CurrentState { get; private set; } =
            PlayerAnimationState.IdleDown;

        public string CurrentStateName =>
            PlayerAnimationModel.StateName(CurrentState);

        public Animator Animator => animator;

        private void Awake()
        {
            motor = GetComponent<TopDownPlayerMotor>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(includeInactive: true);
            }
        }

        private void Start()
        {
            Refresh();
        }

        private void LateUpdate()
        {
            Refresh();
        }

        public void Initialize(Animator targetAnimator)
        {
            animator = targetAnimator;
            currentStateHash = 0;

            if (Application.isPlaying)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            if (motor == null)
            {
                motor = GetComponent<TopDownPlayerMotor>();
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>(includeInactive: true);
            }

            if (motor == null || animator == null ||
                animator.runtimeAnimatorController == null)
            {
                return;
            }

            PlayerAnimationState nextState = PlayerAnimationModel.Resolve(
                motor.Facing,
                motor.IsMoving);
            string stateName = PlayerAnimationModel.StateName(nextState);
            int stateHash = Animator.StringToHash(BaseLayerPrefix + stateName);

            if (stateHash == currentStateHash)
            {
                return;
            }

            if (!animator.HasState(0, stateHash))
            {
                Debug.LogError(
                    $"Animator state '{stateName}' is missing from the farmer controller.",
                    animator);
                return;
            }

            animator.Play(stateHash, 0, 0f);
            CurrentState = nextState;
            currentStateHash = stateHash;
        }
    }
}
