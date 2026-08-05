namespace FarmSimulator.Application.Spatial
{
    public static class SpatialModel
    {
        public const string GroundPlane = "XY";
        public const string DepthAxis = "Z";

        public const bool UsesTwoDimensionalPhysics = true;
        public const bool AllowsThreeDimensionalEffects = true;
        public const bool UsesOrthographicCamera = true;

        public const float GridCellSize = 1f;
        public const int GridColumns = 16;
        public const int GridRows = 12;
        public const float ReferenceCharacterHeight = 1.8f;
        public const float ReferenceCharacterWidth = 0.8f;
        public const float CameraOrthographicSize = 6.75f;
        public const float CameraDepth = -10f;
    }
}
