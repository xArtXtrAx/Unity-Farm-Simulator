using System;

namespace FarmSimulator.Application.Player
{
    public enum FacingDirection
    {
        Down,
        Left,
        Right,
        Up,
    }

    public readonly struct ProcessedMovement
    {
        public ProcessedMovement(
            float x,
            float y,
            float strength,
            FacingDirection facing)
        {
            X = x;
            Y = y;
            Strength = strength;
            Facing = facing;
        }

        public float X { get; }

        public float Y { get; }

        public float Strength { get; }

        public FacingDirection Facing { get; }

        public bool IsMoving => Strength > 0f;
    }

    public static class PlayerMovementModel
    {
        public const float DefaultDeadZone = 0.18f;
        public const float DefaultSpeedUnitsPerSecond = 3.25f;

        public static ProcessedMovement Process(
            float rawX,
            float rawY,
            FacingDirection previousFacing,
            float deadZone = DefaultDeadZone)
        {
            float clampedDeadZone = Clamp(deadZone, 0f, 0.99f);
            float magnitudeSquared = rawX * rawX + rawY * rawY;

            if (magnitudeSquared <= clampedDeadZone * clampedDeadZone)
            {
                return new ProcessedMovement(0f, 0f, 0f, previousFacing);
            }

            float magnitude = (float)Math.Sqrt(magnitudeSquared);
            float directionX = rawX / magnitude;
            float directionY = rawY / magnitude;
            float strength = magnitude >= 1f
                ? 1f
                : (magnitude - clampedDeadZone) / (1f - clampedDeadZone);
            strength = Clamp(strength, 0f, 1f);

            FacingDirection facing = ResolveFacing(
                directionX,
                directionY,
                previousFacing);

            return new ProcessedMovement(
                directionX * strength,
                directionY * strength,
                strength,
                facing);
        }

        public static FacingDirection ResolveFacing(
            float x,
            float y,
            FacingDirection previousFacing)
        {
            float absoluteX = Math.Abs(x);
            float absoluteY = Math.Abs(y);

            if (absoluteX <= float.Epsilon && absoluteY <= float.Epsilon)
            {
                return previousFacing;
            }

            if (absoluteX > absoluteY)
            {
                return x < 0f
                    ? FacingDirection.Left
                    : FacingDirection.Right;
            }

            return y < 0f
                ? FacingDirection.Down
                : FacingDirection.Up;
        }

        private static float Clamp(float value, float minimum, float maximum)
        {
            if (value < minimum)
            {
                return minimum;
            }

            return value > maximum ? maximum : value;
        }
    }
}
