using System;
using FarmSimulator.Presentation.Art;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public sealed class SceneRecoveryArtProfile : ScriptableObject
    {
        public const string AssetPath =
            "Assets/_Project/Editor/Scene Recovery Art Profile.asset";

        private const string GroundTilePath =
            "Assets/_Project/Art/Placeholder/Tiles/ground_grass.asset";
        private const string PathTilePath =
            "Assets/_Project/Art/Placeholder/Tiles/path_dirt.asset";
        private const string HouseSpritePath =
            "Assets/_Project/Art/Placeholder/Source/house_small_4x5.png";
        private const string BedSpritePath =
            "Assets/_Project/Art/Placeholder/Source/bed_single.png";
        private const string HouseFloorSpritePath =
            "Assets/_Project/Art/Placeholder/Source/house_floor.png";
        private const string HouseWallSpritePath =
            "Assets/_Project/Art/Placeholder/Source/house_wall.png";
        private const string HouseFloorTilePath =
            "Assets/_Project/Art/Placeholder/Tiles/house_floor.asset";
        private const string HouseWallTilePath =
            "Assets/_Project/Art/Placeholder/Tiles/house_wall_oriented.asset";

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

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Scene Recovery/Prepare First-Party Art Profile")]
        public static void PrepareFirstPartyArtProfile()
        {
            try
            {
                SceneRecoveryArtProfile profile = LoadOrCreate();

                TileBase ground = LoadRequired<TileBase>(GroundTilePath);
                TileBase path = LoadRequired<TileBase>(PathTilePath);
                Sprite house = LoadRequired<Sprite>(HouseSpritePath);
                Sprite bed = LoadRequired<Sprite>(BedSpritePath);
                Tile floor = EnsureTile(
                    HouseFloorSpritePath,
                    HouseFloorTilePath,
                    "house_floor");
                HouseBorderTile wall = EnsureHouseBorderTile(
                    HouseWallSpritePath,
                    HouseWallTilePath);

                profile.farmGroundTile = ground;
                profile.farmPathTile = path;
                profile.farmHouseSprite = house;
                profile.houseFloorTile = floor;
                profile.houseWallTile = wall;
                profile.bedSprite = bed;

                EditorUtility.SetDirty(profile);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Selection.activeObject = profile;
                EditorGUIUtility.PingObject(profile);

                const string message =
                    "First-party Scene Recovery art is ready.\n\n" +
                    "Assigned:\n" +
                    "- Farm Ground Tile\n" +
                    "- Farm Path Tile\n" +
                    "- Farm House Sprite\n" +
                    "- House Floor Tile\n" +
                    "- House Wall Tile (oriented border)\n" +
                    "- Bed Sprite\n\n" +
                    "No third-party art was used.";

                Debug.Log("[Scene Recovery] " + message.Replace("\n", " | "));
                EditorUtility.DisplayDialog("Scene Recovery Art", message, "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "Scene Recovery Art failed",
                    exception.Message + "\n\nSee Console for the complete stack trace.",
                    "OK");
            }
        }

        private static Tile EnsureTile(
            string spritePath,
            string tilePath,
            string tileName)
        {
            Sprite sprite = LoadRequired<Sprite>(spritePath);
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile == null)
            {
                EnsureFolder(System.IO.Path.GetDirectoryName(tilePath)?.Replace('\\', '/'));
                tile = CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tilePath);
            }

            tile.name = tileName;
            tile.sprite = sprite;
            tile.color = Color.white;
            tile.colliderType = Tile.ColliderType.None;
            EditorUtility.SetDirty(tile);
            return tile;
        }

        private static HouseBorderTile EnsureHouseBorderTile(
            string spritePath,
            string tilePath)
        {
            Sprite sprite = LoadRequired<Sprite>(spritePath);
            HouseBorderTile tile =
                AssetDatabase.LoadAssetAtPath<HouseBorderTile>(tilePath);
            if (tile == null)
            {
                UnityEngine.Object existing =
                    AssetDatabase.LoadMainAssetAtPath(tilePath);
                if (existing != null && !AssetDatabase.DeleteAsset(tilePath))
                {
                    throw new InvalidOperationException(
                        $"Could not replace incompatible wall tile at '{tilePath}'.");
                }

                EnsureFolder(System.IO.Path.GetDirectoryName(tilePath)?.Replace('\\', '/'));
                tile = CreateInstance<HouseBorderTile>();
                AssetDatabase.CreateAsset(tile, tilePath);
            }

            tile.name = "house_wall_oriented";
            tile.Configure(sprite);
            EditorUtility.SetDirty(tile);
            return tile;
        }

        private static T LoadRequired<T>(string path)
            where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                throw new InvalidOperationException(
                    $"Required first-party asset is missing or has not imported yet: '{path}'.");
            }

            return asset;
        }

        private static void EnsureFolder(string path)
        {
            if (string.IsNullOrEmpty(path) || AssetDatabase.IsValidFolder(path))
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
                "Scene Recovery uses exact first-party assets from Assets/_Project/Art/Placeholder. " +
                "It never searches by partial names and never substitutes third-party art.",
                MessageType.Info);

            if (GUILayout.Button("Prepare / Repair First-Party References"))
            {
                SceneRecoveryArtProfile.PrepareFirstPartyArtProfile();
                profile = SceneRecoveryArtProfile.LoadOrCreate();
                serializedProfile = new SerializedObject(profile);
            }

            EditorGUILayout.Space();
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
