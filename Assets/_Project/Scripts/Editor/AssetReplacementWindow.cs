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
        private const string PlaceholderRoot = "Assets/_Project/Art/Placeholder";
        private const string ProfileFolder = "Assets/_Project/Art/Replacement Profiles";
        private const string DefaultProfilePath =
            ProfileFolder + "/Local Art Replacement Profile.asset";

        [SerializeField]
        private AssetReplacementProfile profile;

        [SerializeField]
        private Vector2 scroll;

        [SerializeField]
        private bool showTiles = true;

        [SerializeField]
        private bool showObjects = true;

        private SerializedObject serializedProfile;

        [MenuItem(MenuPath)]
        public static void Open()
        {
            AssetReplacementWindow window =
                GetWindow<AssetReplacementWindow>("Art Replacement");
            window.minSize = new Vector2(560f, 420f);
            window.Show();
        }

        private void OnEnable()
        {
            TryLoadDefaultProfile();
            RebuildSerializedProfile();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawProfileToolbar();

            if (profile == null)
            {
                EditorGUILayout.HelpBox(
                    "Create or select a replacement profile to begin. The profile stores only " +
                    "your mappings; placeholder assets remain unchanged.",
                    MessageType.Info);
                return;
            }

            if (serializedProfile == null || serializedProfile.targetObject != profile)
            {
                RebuildSerializedProfile();
            }

            DrawSummary();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            serializedProfile.Update();
            DrawTileEntries();
            EditorGUILayout.Space(8f);
            DrawObjectEntries();
            serializedProfile.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();

            DrawActionButtons();
        }

        private static void DrawHeader()
        {
            EditorGUILayout.LabelField("Semantic Art Replacement", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Assign final art to stable semantic placeholders. Tilemaps replace Tile assets. " +
                "Scene objects keep their Transform, collider, footprint, portal and gameplay " +
                "components; only their SpriteRenderer visual changes.",
                MessageType.Info);
        }

        private void DrawProfileToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            AssetReplacementProfile selected =
                (AssetReplacementProfile)EditorGUILayout.ObjectField(
                    profile,
                    typeof(AssetReplacementProfile),
                    false,
                    GUILayout.MinWidth(220f));
            if (selected != profile)
            {
                profile = selected;
                RebuildSerializedProfile();
            }

            if (GUILayout.Button("Create/Open Local Profile", EditorStyles.toolbarButton))
            {
                profile = CreateOrLoadDefaultProfile();
                RebuildSerializedProfile();
            }

            using (new EditorGUI.DisabledScope(profile == null))
            {
                if (GUILayout.Button("Sync Placeholders", EditorStyles.toolbarButton))
                {
                    SynchronizeProfile();
                }

                if (GUILayout.Button("Ping", EditorStyles.toolbarButton, GUILayout.Width(42f)))
                {
                    EditorGUIUtility.PingObject(profile);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSummary()
        {
            int tileTotal = profile.TileEntries.Count;
            int tileAssigned = profile.TileEntries.Count(entry =>
                entry != null && entry.source != null && entry.replacement != null);
            int objectTotal = profile.ObjectVisualEntries.Count;
            int objectAssigned = profile.ObjectVisualEntries.Count(entry =>
                entry != null &&
                (entry.replacementSprite != null || entry.replacementPrefab != null));
            int sceneObjects = CountOpenScenePlaceholders();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                $"Tiles: {tileAssigned}/{tileTotal} assigned    " +
                $"Objects: {objectAssigned}/{objectTotal} assigned    " +
                $"Placeholders in open scenes: {sceneObjects}");

            if (tileTotal == 0 || objectTotal == 0)
            {
                EditorGUILayout.HelpBox(
                    "The profile is not synchronized with the official placeholder catalog. " +
                    "Press Sync Placeholders.",
                    MessageType.Warning);
            }
            else if (tileAssigned < tileTotal || objectAssigned < objectTotal)
            {
                EditorGUILayout.HelpBox(
                    "Unassigned rows are allowed. Preview reports what will change and leaves " +
                    "unassigned placeholders untouched.",
                    MessageType.None);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTileEntries()
        {
            SerializedProperty entries = serializedProfile.FindProperty("tileEntries");
            showTiles = EditorGUILayout.Foldout(
                showTiles,
                $"Tile replacements ({entries.arraySize})",
                true,
                EditorStyles.foldoutHeader);
            if (!showTiles)
            {
                return;
            }

            EditorGUI.indentLevel++;
            for (int index = 0; index < entries.arraySize; index++)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(index);
                SerializedProperty source = entry.FindPropertyRelative("source");
                SerializedProperty replacement = entry.FindPropertyRelative("replacement");

                EditorGUILayout.BeginHorizontal();
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(
                        source,
                        GUIContent.none,
                        GUILayout.MinWidth(180f));
                }
                GUILayout.Label("→", GUILayout.Width(18f));
                EditorGUILayout.PropertyField(
                    replacement,
                    GUIContent.none,
                    GUILayout.MinWidth(180f));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        private void DrawObjectEntries()
        {
            SerializedProperty entries = serializedProfile.FindProperty("objectVisualEntries");
            showObjects = EditorGUILayout.Foldout(
                showObjects,
                $"Object visual replacements ({entries.arraySize})",
                true,
                EditorStyles.foldoutHeader);
            if (!showObjects)
            {
                return;
            }

            EditorGUI.indentLevel++;
            for (int index = 0; index < entries.arraySize; index++)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(index);
                SerializedProperty key = entry.FindPropertyRelative("assetKey");
                SerializedProperty sprite = entry.FindPropertyRelative("replacementSprite");
                SerializedProperty prefab = entry.FindPropertyRelative("replacementPrefab");
                SerializedProperty copySorting =
                    entry.FindPropertyRelative("copySortingFromReplacement");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(key, new GUIContent("Semantic key"));
                }
                EditorGUILayout.PropertyField(sprite, new GUIContent("Replacement sprite"));
                EditorGUILayout.PropertyField(prefab, new GUIContent("Or replacement prefab"));
                EditorGUILayout.PropertyField(
                    copySorting,
                    new GUIContent("Copy sorting from prefab"));

                if (sprite.objectReferenceValue != null && prefab.objectReferenceValue != null)
                {
                    EditorGUILayout.HelpBox(
                        "Both are assigned. The explicit sprite takes priority; the prefab may " +
                        "still provide sorting settings.",
                        MessageType.Info);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview Open Scenes", GUILayout.Height(28f)))
            {
                Run(previewOnly: true);
            }

            if (GUILayout.Button("Apply To Open Scenes", GUILayout.Height(28f)))
            {
                Run(previewOnly: false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                "Apply records Unity Undo and marks affected scenes dirty. Save scenes manually " +
                "after reviewing the result.",
                MessageType.None);
        }

        private void SynchronizeProfile()
        {
            if (profile == null)
            {
                return;
            }

            int addedTiles = 0;
            int addedObjects = 0;

            foreach (string guid in AssetDatabase.FindAssets(
                         "t:TileBase",
                         new[] { PlaceholderRoot + "/Tiles" }))
            {
                TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (profile.EnsureTileEntry(tile))
                {
                    addedTiles++;
                }
            }

            foreach (string guid in AssetDatabase.FindAssets(
                         "t:Prefab",
                         new[] { PlaceholderRoot + "/Prefabs" }))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                    AssetDatabase.GUIDToAssetPath(guid));
                PlaceholderAssetIdentity identity = prefab == null
                    ? null
                    : prefab.GetComponent<PlaceholderAssetIdentity>();
                if (identity != null && profile.EnsureObjectEntry(identity.AssetKey))
                {
                    addedObjects++;
                }
            }

            profile.SortEntries();
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            RebuildSerializedProfile();

            Debug.Log(
                $"[Art Replacement] Profile synchronized: {addedTiles} tile rows and " +
                $"{addedObjects} object rows added.");
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

                int sceneChanges = 0;
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
                            sceneChanges++;
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
                        sceneChanges++;
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

                if (!previewOnly && sceneChanges > 0)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                }
            }

            string mode = previewOnly ? "Preview" : "Applied";
            string message =
                $"{mode}: {tileCount} tile replacements, {objectCount} object visuals" +
                (missingVisualCount > 0
                    ? $", {missingVisualCount} objects without a SpriteRenderer."
                    : ".");
            Debug.Log("[Art Replacement] " + message);
            EditorUtility.DisplayDialog("Art Replacement", message, "OK");
            Repaint();
        }

        private int CountOpenScenePlaceholders()
        {
            int count = 0;
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.IsValid() || !scene.isLoaded)
                {
                    continue;
                }

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    count += root.GetComponentsInChildren<PlaceholderAssetIdentity>(true).Length;
                }
            }
            return count;
        }

        private static SpriteRenderer ResolveReplacementRenderer(
            AssetReplacementProfile.ObjectVisualEntry entry)
        {
            return entry.replacementPrefab == null
                ? null
                : entry.replacementPrefab.GetComponentInChildren<SpriteRenderer>(true);
        }

        private void TryLoadDefaultProfile()
        {
            if (profile == null)
            {
                profile = AssetDatabase.LoadAssetAtPath<AssetReplacementProfile>(
                    DefaultProfilePath);
            }
        }

        private static AssetReplacementProfile CreateOrLoadDefaultProfile()
        {
            AssetReplacementProfile existing =
                AssetDatabase.LoadAssetAtPath<AssetReplacementProfile>(DefaultProfilePath);
            if (existing != null)
            {
                return existing;
            }

            EnsureFolder("Assets/_Project/Art", "Replacement Profiles");
            AssetReplacementProfile created =
                CreateInstance<AssetReplacementProfile>();
            AssetDatabase.CreateAsset(created, DefaultProfilePath);
            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(created);
            return created;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private void RebuildSerializedProfile()
        {
            serializedProfile = profile == null ? null : new SerializedObject(profile);
            Repaint();
        }
    }
}
