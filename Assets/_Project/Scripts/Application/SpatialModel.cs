using FarmSimulator.Application.Display;

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
        public const int GridColumns = 15;
        public const int GridRows = 8;
        public const float ReferenceCharacterHeight =
            PixelArtDisplayModel.FarmerHeightUnits;
        public const float ReferenceCharacterWidth =
            PixelArtDisplayModel.FarmerWidthUnits;
        public const float CameraOrthographicSize =
            PixelArtDisplayModel.CameraOrthographicSize;
        public const float CameraDepth = -10f;
    }
}
