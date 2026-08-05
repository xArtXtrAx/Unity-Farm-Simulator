namespace FarmSimulator.Application.Spatial
{
    public static class SpatialModel
    {
        public const string GroundPlane = "XZ";
        public const string HeightAxis = "Y";

        public const bool UsesThreeDimensionalPhysics = true;
        public const bool UsesOrthographicCamera = true;

        public const float GridCellSize = 1f;
        public const int GridColumns = 16;
        public const int GridRows = 12;
        public const float ReferenceCharacterHeight = 1.8f;
        public const float CameraOrthographicSize = 7.5f;
    }
}
