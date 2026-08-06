using FarmSimulator.Application.Player;
using UnityEngine;

namespace FarmSimulator.Presentation.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public sealed class TopDownPlayerMotor : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        private float speed = PlayerMovementModel.DefaultSpeedUnitsPerSecond;

        private Rigidbody2D body;

        public Vector2 DesiredMovement { get; private set; }

        public FacingDirection Facing { get; private set; } = FacingDirection.Down;

        public bool IsMoving => DesiredMovement.sqrMagnitude > 0f;

        public float Speed => speed;

        public Rigidbody2D Body => body;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void OnDisable()
        {
            Stop();
        }

        private void FixedUpdate()
        {
            if (DesiredMovement.sqrMagnitude <= 0f)
            {
                return;
            }

            Vector2 displacement =
                DesiredMovement * speed *
                global::UnityEngine.Time.fixedDeltaTime;
            body.MovePosition(body.position + displacement);
        }

        public void Configure(float movementSpeed)
        {
            speed = Mathf.Max(0f, movementSpeed);
        }

        public void SetDesiredInput(Vector2 rawInput)
        {
            ProcessedMovement processed = PlayerMovementModel.Process(
                rawInput.x,
                rawInput.y,
                Facing);

            DesiredMovement = new Vector2(processed.X, processed.Y);
            Facing = processed.Facing;
        }

        public void Stop()
        {
            DesiredMovement = Vector2.zero;
        }
    }
}
