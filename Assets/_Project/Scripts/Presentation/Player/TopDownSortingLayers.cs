namespace FarmSimulator.Presentation.Player
{
    public static class TopDownSortingLayers
    {
        public const string Ground = "Ground";
        public const string World = "World";
        public const string Actors = "Actors";
        public const string Effects = "Effects";
        public const string UserInterface = "UI";

        public static readonly string[] RequiredNames =
        {
            Ground,
            World,
            Actors,
            Effects,
            UserInterface,
        };
    }
}
