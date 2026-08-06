using UnityEditor;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    internal static class LegacyCropPaletteCleanup
    {
        private const string LegacyCropPalettePath =
            "Assets/_Project/Tiles/Palettes/Cozy Farm - Crops.prefab";

        static LegacyCropPaletteCleanup()
        {
            EditorApplication.delayCall += RemoveLegacyPalette;
        }

        private static void RemoveLegacyPalette()
        {
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                    LegacyCropPalettePath) == null)
            {
                return;
            }

            AssetDatabase.DeleteAsset(LegacyCropPalettePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
