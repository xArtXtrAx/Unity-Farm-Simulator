using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class LegacyHouseScenePipelineGuard
    {
        static LegacyHouseScenePipelineGuard()
        {
            EditorApplication.delayCall += DisableLegacyAutoGeneration;
        }

        private static void DisableLegacyAutoGeneration()
        {
            EditorApplication.delayCall -= HouseAndSleepScenePipeline.EnsureScenes;
        }

        [MenuItem(
            "Tools/Farm Simulator/Legacy/Rebuild House and Sleep Scenes",
            true)]
        private static bool ValidateLegacyRebuild()
        {
            Debug.LogWarning(
                "The legacy HouseAndSleepScenePipeline is obsolete and must not be " +
                "used for the current Farm or HouseInterior. Use Farm Development " +
                "Kit > Scene Recovery instead.");
            return false;
        }
    }
}
