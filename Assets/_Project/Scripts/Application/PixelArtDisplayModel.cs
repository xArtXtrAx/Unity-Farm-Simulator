namespace FarmSimulator.Application.Display
{
    public readonly struct NormalizedViewport
    {
        public static readonly NormalizedViewport Full = new(0f, 0f, 1f, 1f);

        public NormalizedViewport(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X { get; }

        public float Y { get; }

        public float Width { get; }

        public float Height { get; }
    }

    public static class PixelArtDisplayModel
    {
        public const int ReferenceWidthPixels = 960;
        public const int ReferenceHeightPixels = 540;
        public const int PixelsPerUnit = 64;
        public const int ReferenceTilePixels = 64;
        public const int FarmerFrameWidthPixels = 64;
        public const int FarmerFrameHeightPixels = 72;

        public const float TargetAspectRatio =
            ReferenceWidthPixels / (float)ReferenceHeightPixels;
        public const float VisibleWorldWidth =
            ReferenceWidthPixels / (float)PixelsPerUnit;
        public const float VisibleWorldHeight =
            ReferenceHeightPixels / (float)PixelsPerUnit;
        public const float CameraOrthographicSize = VisibleWorldHeight * 0.5f;
        public const float FarmerWidthUnits =
            FarmerFrameWidthPixels / (float)PixelsPerUnit;
        public const float FarmerHeightUnits =
            FarmerFrameHeightPixels / (float)PixelsPerUnit;

        public static NormalizedViewport CalculateViewport(
            int outputWidthPixels,
            int outputHeightPixels)
        {
            if (outputWidthPixels <= 0 || outputHeightPixels <= 0)
            {
                return NormalizedViewport.Full;
            }

            float outputAspect =
                outputWidthPixels / (float)outputHeightPixels;

            if (outputAspect > TargetAspectRatio)
            {
                float width = TargetAspectRatio / outputAspect;
                return new NormalizedViewport(
                    (1f - width) * 0.5f,
                    0f,
                    width,
                    1f);
            }

            if (outputAspect < TargetAspectRatio)
            {
                float height = outputAspect / TargetAspectRatio;
                return new NormalizedViewport(
                    0f,
                    (1f - height) * 0.5f,
                    1f,
                    height);
            }

            return NormalizedViewport.Full;
        }
    }
}
