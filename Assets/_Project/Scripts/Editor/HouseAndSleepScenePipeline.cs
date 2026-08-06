using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Compatibility stub retained for older tests and references.
    /// The original pipeline rebuilt Farm and HouseInterior with obsolete,
    /// hard-coded sprite patches and must never author current scenes.
    /// </summary>
    public static class HouseAndSleepScenePipeline
    {
        public const string FarmImportSignature =
            "legacy-farm-house-entry-scene-disabled";
        public const string HouseImportSignature =
            "legacy-house-interior-sleep-scene-disabled";

        [MenuItem("Tools/Farm Simulator/Legacy/Rebuild Obsolete House Scenes")]
        public static void RebuildScenes()
        {
            EditorUtility.DisplayDialog(
                "Legacy scene generator disabled",
                "This generator used obsolete tiles and can no longer rebuild " +
                "Farm or HouseInterior. Use Farm Development Kit > Scene Recovery.",
                "OK");
        }

        public static void EnsureScenes()
        {
            // Intentionally empty. Missing scenes must be created explicitly
            // through ModernFarmSceneAuthoring; never during domain reload.
        }
    }
}
