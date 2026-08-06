using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class LegacyHouseScenePipelineGuard
    {
        private const string LegacyMenu =
            "Tools/Farm Simulator/Rebuild House and Sleep Scenes";

        static LegacyHouseScenePipelineGuard()
        {
            EditorApplication.delayCall += DisableLegacyAutoGeneration;
        }

        private static void DisableLegacyAutoGeneration()
        {
            EditorApplication.delayCall -= HouseAndSleepScenePipeline.EnsureScenes;
        }

        [MenuItem(LegacyMenu, true)]
        private static bool ValidateLegacyRebuild()
        {
            Debug.LogWarning(
                "The legacy HouseAndSleepScenePipeline is obsolete and disabled. " +
                "Use Farm Development Kit > Scene Recovery instead.");
            return false;
        }
    }
}
