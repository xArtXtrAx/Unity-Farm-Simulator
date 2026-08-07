using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Compatibility entry point for the old free-placeholder scene command.
    /// Scene authoring is now owned exclusively by ModernFarmSceneAuthoring and
    /// SceneRecoveryArtProfile. This wrapper intentionally has no delayed
    /// post-processing and never opens scenes through EditorSceneManager.
    /// </summary>
    public static class FreePlaceholderSceneBuilder
    {
        private const string MenuPath =
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Scenes/Replace Farm + HouseInterior";

        [MenuItem(MenuPath)]
        public static void ReplaceScenes()
        {
            if (!CanAuthorScenes())
            {
                Debug.LogWarning(
                    "[Free Placeholder Scenes] Scene authoring is unavailable during Play Mode.");
                return;
            }

            // Preserve the old entry point, but route all current work through the
            // single canonical first-party recovery pipeline. No delayCall is
            // scheduled here, so entering Play Mode cannot leave an OpenScene
            // operation pending in the Editor queue.
            ModernFarmSceneAuthoring.ReplaceScenesWithBackup();
        }

        private static bool CanAuthorScenes() =>
            !EditorApplication.isPlaying &&
            !EditorApplication.isPlayingOrWillChangePlaymode &&
            !UnityEngine.Application.isPlaying;
    }
}
