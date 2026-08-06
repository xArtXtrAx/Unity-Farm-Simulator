using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Compatibility stub for the retired Cozy Interior scene upgrader.
    /// The original implementation depended on the legacy HouseInterior
    /// hierarchy (Wood Floor, Back Interior Wall, Hero Bed, and similar
    /// objects) and must never run automatically against modern scenes.
    /// </summary>
    public static class CozyInteriorHouseSceneUpgrader
    {
        public const string UpgradeSignature =
            "house-interior-cozy-interior-v2-disabled";

        public const string MarkerName =
            "Cozy Interior Visual Upgrade (Legacy Disabled)";

        [MenuItem("Tools/Farm Simulator/Legacy/Apply Cozy Interior To House Scene (Disabled)")]
        public static void ApplyFromMenu()
        {
            Debug.LogWarning(
                "The legacy Cozy Interior scene upgrader is disabled because it " +
                "requires the obsolete HouseInterior hierarchy (Wood Floor, " +
                "Back Interior Wall, Hero Bed, and related objects). Use the " +
                "placeholder-art and semantic replacement pipeline instead.");
        }

        public static void EnsureUpgradedScene()
        {
            // Intentionally no-op. Kept only for source compatibility.
        }

        public static bool ApplyToHouseScene(bool force)
        {
            Debug.LogWarning(
                "CozyInteriorHouseSceneUpgrader.ApplyToHouseScene is disabled. " +
                "The modern HouseInterior scene must be authored through the " +
                "semantic placeholder/replacement pipeline.");
            return false;
        }
    }
}
