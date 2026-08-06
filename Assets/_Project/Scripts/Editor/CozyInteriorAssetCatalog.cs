using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public static class CozyInteriorAssetCatalog
    {
        public const int GridSizePixels = 16;
        public const int DoorCellWidthPixels = 48;
        public const int DoorCellHeightPixels = 32;
        public const int PetCellSizePixels = 18;
        public const int RetroTelevisionCellSizePixels = 16;
        public const int TelevisionCellSizePixels = 32;

        public const string Root = CozyInteriorFullPackImporter.FullAssetRoot;
        public const string GlobalPath = Root + "/global.png";
        public const string WallpapersPath = Root + "/basics/wallpapers.png";
        public const string DoorsPath = Root + "/basics/doors.png";
        public const string RugsPath = Root + "/basics/rugs.png";
        public const string FireplacesPath = Root + "/basics/fireplaces.png";
        public const string BedsPath = Root + "/furniture/beds.png";
        public const string DecorationsPath = Root + "/furniture/decorations.png";
        public const string TablesPath = Root + "/furniture/tables.png";
        public const string ChairsPath = Root + "/furniture/chairs.png";
        public const string StoragePath = Root + "/furniture/storage.png";
        public const string KitchensPath =
            Root + "/furniture/kitchens_assembled.png";
        public const string PetsPath = Root + "/pets/pets.png";
        public const string CatAnimationPath =
            Root + "/pets/cat animation.png";
        public const string YorkieAnimationPath =
            Root + "/pets/yorkie animation.png";

        public static IReadOnlyList<string> CoreSheetPaths { get; } =
            new[]
            {
                GlobalPath,
                WallpapersPath,
                DoorsPath,
                RugsPath,
                FireplacesPath,
                BedsPath,
                DecorationsPath,
                TablesPath,
                ChairsPath,
                StoragePath,
                KitchensPath,
                PetsPath,
            };

        public static bool IsImported =>
            AssetDatabase.LoadAssetAtPath<Texture2D>(GlobalPath) != null &&
            AssetDatabase.LoadAssetAtPath<Texture2D>(WallpapersPath) != null &&
            AssetDatabase.LoadAssetAtPath<Texture2D>(BedsPath) != null;
    }
}
