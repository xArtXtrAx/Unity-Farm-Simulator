using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public sealed class SceneRecoveryArtProfile : ScriptableObject
    {
        public const string AssetPath =
            "Assets/_Project/Editor/Scene Recovery Art Profile.asset";

        [Header("Farm")]
        public TileBase farmGroundTile;
        public TileBase farmPathTile;
        public Sprite farmHouseSprite;

        [Header("House Interior")]
        public TileBase houseFloorTile;
        public TileBase houseWallTile;
        public Sprite bedSprite;

        public static SceneRecoveryArtProfile LoadOrCreate()
        {
            SceneRecoveryArtProfile profile =
                AssetDatabase.LoadAssetAtPath<SceneRecoveryArtProfile>(AssetPath);
            if (profile != null)
            {
                return profile;
            }

            EnsureFolder("Assets/_Project/Editor");
            profile = CreateInstance<SceneRecoveryArtProfile>();
            AssetDatabase.CreateAsset(profile, AssetPath);
            AssetDatabase.SaveAssets();
            return profile;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(path)
                ?.Replace('\\', '/');
            string name = System.IO.Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) &&
                !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, name);
        }
    }

    public sealed class SceneRecoveryArtProfileWindow : EditorWindow
    {
        private SceneRecoveryArtProfile profile;
        private SerializedObject serializedProfile;

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Scene Recovery/Configure Art Profile")]
        public static void Open()
        {
            GetWindow<SceneRecoveryArtProfileWindow>(
                "Scene Recovery Art");
        }

        private void OnEnable()
        {
            profile = SceneRecoveryArtProfile.LoadOrCreate();
            serializedProfile = new SerializedObject(profile);
        }

        private void OnGUI()
        {
            if (profile == null)
            {
                OnEnable();
            }

            EditorGUILayout.HelpBox(
                "Assign exact tiles and sprites from the current Cozy Farm / Cozy Interior libraries. " +
                "Scene Recovery never searches by partial names and never substitutes another asset.",
                MessageType.Info);

            serializedProfile.Update();
            EditorGUILayout.PropertyField(
                serializedProfile.FindProperty("farmGroundTile"));
            EditorGUILayout.PropertyField(
                serializedProfile.FindProperty("farmPathTile"));
            EditorGUILayout.PropertyField(
                serializedProfile.FindProperty("farmHouseSprite"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(
                serializedProfile.FindProperty("houseFloorTile"));
            EditorGUILayout.PropertyField(
                serializedProfile.FindProperty("houseWallTile"));
            EditorGUILayout.PropertyField(
                serializedProfile.FindProperty("bedSprite"));

            if (serializedProfile.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(profile);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Select Profile Asset"))
            {
                Selection.activeObject = profile;
                EditorGUIUtility.PingObject(profile);
            }
        }
    }
}
