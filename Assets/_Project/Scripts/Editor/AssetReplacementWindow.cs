using System;
using System.Linq;
using FarmSimulator.Presentation.Art;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public sealed class AssetReplacementWindow : EditorWindow
    {
        private const string MenuPath =
            "Tools/Farm Simulator/Farm Development Kit/Art Replacement/Open Window";

        [SerializeField]
        private AssetReplacementProfile profile;

        [MenuItem(MenuPath)]
        public static void Open()
        {
            GetWindow<AssetReplacementWindow>("Art Replacement");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Semantic Art Replacement", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Tiles are replaced by explicit asset references. Placeholder objects keep " +
                "their Transform, collider, footprint and gameplay components; only their " +
                "SpriteRenderer visual is changed.",
                MessageType.Info);

            profile = (AssetReplacementProfile)EditorGUILayout.ObjectField(
                "Replacement Profile",
                profile,
                typeof(AssetReplacementProfile),
                false);

            using (new EditorGUI.DisabledScope(profile == null))
            {
                if (GUILayout.Button("Preview Open Scenes"))
                {
                    Run(previewOnly: true);
                }

                if (GUILayout.Button("Apply To Open Scenes"))
                {
                    Run(previewOnly: false);
                }
            }
        }

        private void Run(bool previewOnly)
        {
            if (profile == null)
            {
                return;
            }

            int tileCount = 0;
            int objectCount = 0;
            int missingVisualCount = 0;

            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.IsValid() || !scene.isLoaded)
                {
                    continue;
                }

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    foreach (Tilemap tilemap in root.GetComponentsInChildren<Tilemap>(true))
                    {
                        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin)
                        {
                            TileBase current = tilemap.GetTile(position);
                            if (current == null)
                            {
                                continue;
                            }

                            AssetReplacementProfile.TileEntry entry = profile.TileEntries
                                .FirstOrDefault(candidate =>
                                    candidate != null &&
                                    candidate.source == current &&
                                    candidate.replacement != null);
                            if (entry == null)
                            {
                                continue;
                            }

                            tileCount++;
                            if (!previewOnly)
                            {
                                Undo.RecordObject(tilemap, "Replace farm tile art");
                                tilemap.SetTile(position, entry.replacement);
                            }
                        }
                    }

                    foreach (PlaceholderAssetIdentity identity in
                             root.GetComponentsInChildren<PlaceholderAssetIdentity>(true))
                    {
                        AssetReplacementProfile.ObjectVisualEntry entry =
                            profile.ObjectVisualEntries.FirstOrDefault(candidate =>
                                candidate != null &&
                                string.Equals(
                                    candidate.assetKey,
                                    identity.AssetKey,
                                    StringComparison.Ordinal));
                        if (entry == null)
                        {
                            continue;
                        }

                        SpriteRenderer sourceRenderer = ResolveReplacementRenderer(entry);
                        Sprite replacementSprite = entry.replacementSprite != null
                            ? entry.replacementSprite
                            : sourceRenderer != null ? sourceRenderer.sprite : null;

                        if (replacementSprite == null)
                        {
                            missingVisualCount++;
                            continue;
                        }

                        SpriteRenderer target = identity.GetComponent<SpriteRenderer>();
                        if (target == null)
                        {
                            target = identity.GetComponentInChildren<SpriteRenderer>(true);
                        }

                        if (target == null)
                        {
                            missingVisualCount++;
                            continue;
                        }

                        objectCount++;
                        if (!previewOnly)
                        {
                            Undo.RecordObject(target, "Replace farm object visual");
                            target.sprite = replacementSprite;
                            target.color = Color.white;

                            if (entry.copySortingFromReplacement && sourceRenderer != null)
                            {
                                target.sortingLayerID = sourceRenderer.sortingLayerID;
                                target.sortingOrder = sourceRenderer.sortingOrder;
                            }
                        }
                    }
                }

                if (!previewOnly && (tileCount > 0 || objectCount > 0))
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                }
            }

            string mode = previewOnly ? "Preview" : "Applied";
            string message =
                $"{mode}: {tileCount} tile replacements, {objectCount} object visuals" +
                (missingVisualCount > 0
                    ? $", {missingVisualCount} entries without a usable visual."
                    : ".");
            Debug.Log("[Art Replacement] " + message);
            EditorUtility.DisplayDialog("Art Replacement", message, "OK");
        }

        private static SpriteRenderer ResolveReplacementRenderer(
            AssetReplacementProfile.ObjectVisualEntry entry)
        {
            return entry.replacementPrefab == null
                ? null
                : entry.replacementPrefab.GetComponentInChildren<SpriteRenderer>(true);
        }
    }
}
