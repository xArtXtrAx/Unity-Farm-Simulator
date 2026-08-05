namespace FarmSimulator.Application.Player
{
    public enum PlayerAnimationState
    {
        IdleDown,
        WalkDown,
        IdleLeft,
        WalkLeft,
        IdleRight,
        WalkRight,
        IdleUp,
        WalkUp,
    }

    public static class PlayerAnimationModel
    {
        public const int FrameWidthPixels = 64;
        public const int FrameHeightPixels = 72;
        public const int Columns = 3;
        public const int Rows = 4;
        public const int FrameRate = 8;
        public const float PixelsPerUnit = 64f;
        public const float PivotNormalizedX = 0.5f;
        public const float PivotNormalizedY = 0.12f;

        public static PlayerAnimationState Resolve(
            FacingDirection facing,
            bool isMoving)
        {
            return (facing, isMoving) switch
            {
                (FacingDirection.Left, false) => PlayerAnimationState.IdleLeft,
                (FacingDirection.Left, true) => PlayerAnimationState.WalkLeft,
                (FacingDirection.Right, false) => PlayerAnimationState.IdleRight,
                (FacingDirection.Right, true) => PlayerAnimationState.WalkRight,
                (FacingDirection.Up, false) => PlayerAnimationState.IdleUp,
                (FacingDirection.Up, true) => PlayerAnimationState.WalkUp,
                (_, true) => PlayerAnimationState.WalkDown,
                _ => PlayerAnimationState.IdleDown,
            };
        }

        public static string StateName(PlayerAnimationState state)
        {
            return state.ToString();
        }

        public static bool Loops(PlayerAnimationState state)
        {
            return state is PlayerAnimationState.WalkDown
                or PlayerAnimationState.WalkLeft
                or PlayerAnimationState.WalkRight
                or PlayerAnimationState.WalkUp;
        }

        public static int[] Frames(PlayerAnimationState state)
        {
            FacingDirection facing = FacingForState(state);
            int firstFrame = RowForFacing(facing) * Columns;

            if (!Loops(state))
            {
                return new[] { firstFrame + 1 };
            }

            return new[]
            {
                firstFrame,
                firstFrame + 1,
                firstFrame + 2,
                firstFrame + 1,
            };
        }

        public static string SpriteName(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= Columns * Rows)
            {
                throw new System.ArgumentOutOfRangeException(
                    nameof(frameIndex),
                    frameIndex,
                    "Frame index must belong to the 3 x 4 farmer spritesheet.");
            }

            int row = frameIndex / Columns;
            int column = frameIndex % Columns;
            return $"farmer_{DirectionName(row)}_{column}";
        }

        public static FacingDirection FacingForState(PlayerAnimationState state)
        {
            return state switch
            {
                PlayerAnimationState.IdleLeft or
                PlayerAnimationState.WalkLeft => FacingDirection.Left,

                PlayerAnimationState.IdleRight or
                PlayerAnimationState.WalkRight => FacingDirection.Right,

                PlayerAnimationState.IdleUp or
                PlayerAnimationState.WalkUp => FacingDirection.Up,

                _ => FacingDirection.Down,
            };
        }

        public static int RowForFacing(FacingDirection facing)
        {
            return facing switch
            {
                FacingDirection.Left => 1,
                FacingDirection.Right => 2,
                FacingDirection.Up => 3,
                _ => 0,
            };
        }

        private static string DirectionName(int row)
        {
            return row switch
            {
                1 => "left",
                2 => "right",
                3 => "up",
                _ => "down",
            };
        }
    }
}
