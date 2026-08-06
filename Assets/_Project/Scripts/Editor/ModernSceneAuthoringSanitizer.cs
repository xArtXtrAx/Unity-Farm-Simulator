using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class ModernSceneAuthoringSanitizer
    {
        static ModernSceneAuthoringSanitizer()
        {
            EditorSceneManager.sceneSaving -= RemoveEditorOnlyMarkers;
            EditorSceneManager.sceneSaving += RemoveEditorOnlyMarkers;
        }

        private static void RemoveEditorOnlyMarkers(Scene scene, string path)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                ModernSceneAuthoringMarker[] markers =
                    root.GetComponentsInChildren<ModernSceneAuthoringMarker>(true);
                foreach (ModernSceneAuthoringMarker marker in markers)
                {
                    if (marker != null)
                    {
                        Object.DestroyImmediate(marker.gameObject);
                    }
                }
            }
        }
    }
}
