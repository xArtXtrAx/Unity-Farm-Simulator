using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FarmSimulator.Application.Player;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Scenes;
using FarmSimulator.Presentation.Time;
using FarmSimulator.Presentation.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public static class ModernFarmSceneAuthoring
    {
        private const string MenuRoot =
            "Tools/Farm Simulator/Farm Development Kit/Scene Recovery/";
        private const string PlayerPrefabPath =
            "Assets/_Project/Resources/Prefabs/Player/Player.prefab";
        private const string BackupRoot =
            "Assets/_Project/SceneBackups";

        [MenuItem(MenuRoot + "Generate Missing Farm + HouseInterior")]
        public static void GenerateMissingScenes()
        {
            RunGeneration(replace: false, showDialog: true);
        }

        public static void GenerateMissingScenesSilently()
        {
            RunGeneration(replace: false, showDialog: false);
        }

        [MenuItem(MenuRoot + "Replace Farm + HouseInterior (with backup)")]
        public static void ReplaceScenesWithBackup()
        {
            if (!EditorUtility.DisplayDialog(
                    "Replace Farm and HouseInterior",
                    "This creates timestamped backups and rebuilds both scenes. " +
                    "Only exact references assigned in the Scene Recovery Art Profile are used.",
                    "Back up and replace",
                    "Cancel"))
            {
                return;
            }

            RunGeneration(replace: true, showDialog: true);
        }

        private static void RunGeneration(bool replace, bool showDialog)
        {
            SceneRecoveryArtProfile profile =
                SceneRecoveryArtProfile.LoadOrCreate();
            List<string> messages = new List<string>();

            try
            {
                bool farmChanged = GenerateScene(
                    ProjectSceneNames.FarmPath,
                    scene => BuildFarm(scene, profile),
                    replace,
                    messages);
                bool houseChanged = GenerateScene(
                    ProjectSceneNames.HouseInteriorPath,
                    scene => BuildHouse(scene, profile),
                    replace,
                    messages);

                EnsureBuildSettings();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (!farmChanged && !houseChanged)
                {
                    messages.Add(
                        "No scene was generated because Farm and HouseInterior already exist.");
                }

                AppendProfileWarnings(profile, messages);
                string report = string.Join("\n", messages);
                Debug.Log("Scene Recovery result:\n" + report);
                if (showDialog)
                {
                    EditorUtility.DisplayDialog(
                        "Scene Recovery",
                        report,
                        "OK");
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                if (showDialog)
                {
                    EditorUtility.DisplayDialog(
                        "Scene Recovery failed",
                        exception.Message +
                        "\n\nSee the Console for the complete stack trace.",
                        "OK");
                }

                throw;
            }
        }

        private static bool GenerateScene(
            string path,
            Action<Scene> builder,
            bool replace,
            ICollection<string> messages)
        {
            bool exists = AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null;
            if (!replace && exists)
            {
                messages.Add($"Skipped existing scene: {path}");
                return false;
            }

            if (IsOpen(path))
            {
                throw new InvalidOperationException(
                    $"Close '{path}' before regenerating it.");
            }

            if (replace && exists)
            {
                BackupIfPresent(path);
                if (!AssetDatabase.DeleteAsset(path))
                {
                    throw new InvalidOperationException(
                        $"Unity could not delete '{path}' after backing it up.");
                }
            }

            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Additive);
            try
            {
                builder(scene);
                if (!EditorSceneManager.SaveScene(scene, path))
                {
                    throw new InvalidOperationException(
                        $"Unity could not save '{path}'.");
                }
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }

            messages.Add($"Generated: {path}");
            return true;
        }

        private static void BuildFarm(
            Scene scene,
            SceneRecoveryArtProfile profile)
        {
            GameObject root = CreateRoot(scene, "Farm World");
            GameObject grid = CreateGrid(root.transform, "Farm Authoring Grid");
            Tilemap ground = CreateTilemap(grid.transform, "Ground", -100);
            Tilemap paths = CreateTilemap(grid.transform, "Paths", -90);
            CreateTilemap(grid.transform, "Soil", -80);
            CreateTilemap(grid.transform, "Decoration", 0);

            if (profile.farmGroundTile != null)
            {
                FillRectangle(
                    ground,
                    new BoundsInt(-24, -16, 0, 48, 32, 1),
                    profile.farmGroundTile);
            }

            if (profile.farmPathTile != null)
            {
                FillRectangle(
                    paths,
                    new BoundsInt(-1, -6, 0, 3, 8, 1),
                    profile.farmPathTile);
            }

            GameObject player = CreatePlayer(scene, root.transform, new Vector2(0f, -4f));
            CreateSpawn(root.transform, ProjectSpawnIds.FarmStart,
                new Vector2(0f, -4f), FacingDirection.Up);
            CreateSpawn(root.transform, ProjectSpawnIds.FarmHouseDoor,
                new Vector2(0f, 1f), FacingDirection.Down);
            CreatePortal(root.transform, "House Entrance Portal",
                new Vector2(0f, 1.5f), "Entrar a la casa",
                ProjectSceneNames.HouseInterior,
                ProjectSpawnIds.HouseEntrance);

            if (profile.farmHouseSprite != null)
            {
                CreateSpriteObject(
                    root.transform,
                    "Hero House Visual",
                    profile.farmHouseSprite,
                    new Vector2(0f, 4f),
                    10);
            }

            CreateBounds(root.transform, new Vector2(48f, 32f));
            CreateCamera(scene, player.transform, 9f,
                new Color32(90, 135, 82, 255));
            CreateAuthoringMarker(root.transform,
                "Modern Farm skeleton. All art comes only from Scene Recovery Art Profile.");
        }

        private static void BuildHouse(
            Scene scene,
            SceneRecoveryArtProfile profile)
        {
            GameObject root = CreateRoot(scene, "House Interior World");
            GameObject grid = CreateGrid(root.transform, "House Authoring Grid");
            Tilemap ground = CreateTilemap(grid.transform, "Ground", -100);
            Tilemap walls = CreateTilemap(grid.transform, "Walls", -20);
            CreateTilemap(grid.transform, "Decoration", 0);

            if (profile.houseFloorTile != null)
            {
                FillRectangle(
                    ground,
                    new BoundsInt(-8, -5, 0, 16, 10, 1),
                    profile.houseFloorTile);
            }

            if (profile.houseWallTile != null)
            {
                FillBorder(
                    walls,
                    new BoundsInt(-8, -5, 0, 16, 10, 1),
                    profile.houseWallTile);
            }

            GameObject player = CreatePlayer(scene, root.transform, new Vector2(0f, -3f));
            CreateSpawn(root.transform, ProjectSpawnIds.HouseEntrance,
                new Vector2(0f, -3f), FacingDirection.Up);
            CreateSpawn(root.transform, ProjectSpawnIds.HouseBedWake,
                new Vector2(3f, 1f), FacingDirection.Left);
            CreatePortal(root.transform, "House Exit Portal",
                new Vector2(0f, -4f), "Salir a la granja",
                ProjectSceneNames.Farm,
                ProjectSpawnIds.FarmHouseDoor);

            GameObject bed = new GameObject("Bed");
            bed.transform.SetParent(root.transform, false);
            bed.transform.localPosition = new Vector3(3f, 2f, 0f);
            BoxCollider2D bedCollider = bed.AddComponent<BoxCollider2D>();
            bedCollider.isTrigger = true;
            bedCollider.size = new Vector2(1.5f, 1f);
            bed.AddComponent<BedInteractable>()
                .Configure("Dormir hasta mañana", ProjectSpawnIds.HouseBedWake);
            if (profile.bedSprite != null)
            {
                SpriteRenderer renderer = bed.AddComponent<SpriteRenderer>();
                renderer.sprite = profile.bedSprite;
                renderer.sortingOrder = 10;
            }

            CreateBounds(root.transform, new Vector2(16f, 10f));
            CreateCamera(scene, player.transform, 6f,
                new Color32(48, 38, 32, 255));
            CreateAuthoringMarker(root.transform,
                "Modern HouseInterior skeleton. All art comes only from Scene Recovery Art Profile.");
        }

        private static void AppendProfileWarnings(
            SceneRecoveryArtProfile profile,
            ICollection<string> messages)
        {
            List<string> missing = new List<string>();
            if (profile.farmGroundTile == null) missing.Add("Farm Ground Tile");
            if (profile.farmPathTile == null) missing.Add("Farm Path Tile");
            if (profile.farmHouseSprite == null) missing.Add("Farm House Sprite");
            if (profile.houseFloorTile == null) missing.Add("House Floor Tile");
            if (profile.houseWallTile == null) missing.Add("House Wall Tile");
            if (profile.bedSprite == null) missing.Add("Bed Sprite");

            if (missing.Count == 0)
            {
                messages.Add("Art profile complete: all exact references were used.");
                return;
            }

            messages.Add(
                "Unassigned art was intentionally left empty: " +
                string.Join(", ", missing) + ".");
            messages.Add(
                "Assign them at Scene Recovery > Configure Art Profile, then use Replace with backup.");
        }

        private static GameObject CreateRoot(Scene scene, string name)
        {
            GameObject root = new GameObject(name);
            SceneManager.MoveGameObjectToScene(root, scene);
            return root;
        }

        private static GameObject CreateGrid(Transform parent, string name)
        {
            GameObject grid = new GameObject(name);
            grid.transform.SetParent(parent, false);
            grid.AddComponent<Grid>();
            return grid;
        }

        private static Tilemap CreateTilemap(Transform parent, string name, int order)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            Tilemap tilemap = child.AddComponent<Tilemap>();
            child.AddComponent<TilemapRenderer>().sortingOrder = order;
            return tilemap;
        }

        private static void FillRectangle(
            Tilemap tilemap,
            BoundsInt bounds,
            TileBase tile)
        {
            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                tilemap.SetTile(position, tile);
            }
        }

        private static void FillBorder(
            Tilemap tilemap,
            BoundsInt bounds,
            TileBase tile)
        {
            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                bool border = position.x == bounds.xMin ||
                    position.x == bounds.xMax - 1 ||
                    position.y == bounds.yMin ||
                    position.y == bounds.yMax - 1;
                if (border)
                {
                    tilemap.SetTile(position, tile);
                }
            }
        }

        private static GameObject CreatePlayer(
            Scene scene,
            Transform parent,
            Vector2 position)
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (prefab == null)
            {
                throw new InvalidOperationException(
                    $"Player prefab missing at '{PlayerPrefabPath}'.");
            }

            GameObject instance =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            instance.name = "Player";
            instance.transform.SetParent(parent, true);
            instance.transform.position = position;
            return instance;
        }

        private static void CreateSpawn(
            Transform parent,
            string id,
            Vector2 position,
            FacingDirection facing)
        {
            GameObject spawn = new GameObject($"Spawn {id}");
            spawn.transform.SetParent(parent, false);
            spawn.transform.localPosition = position;
            spawn.AddComponent<SceneSpawnPoint>().Configure(id, facing);
        }

        private static void CreatePortal(
            Transform parent,
            string name,
            Vector2 position,
            string prompt,
            string targetScene,
            string spawnId)
        {
            GameObject portal = new GameObject(name);
            portal.transform.SetParent(parent, false);
            portal.transform.localPosition = position;
            BoxCollider2D collider = portal.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1.5f, 1f);
            portal.AddComponent<ScenePortal>()
                .Configure(prompt, targetScene, spawnId);
        }

        private static void CreateSpriteObject(
            Transform parent,
            string name,
            Sprite sprite,
            Vector2 position,
            int order)
        {
            GameObject item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            SpriteRenderer renderer = item.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
        }

        private static void CreateCamera(
            Scene scene,
            Transform target,
            float size,
            Color background)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = size;
            camera.backgroundColor = background;
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            PlayerFollowCamera2D follow =
                cameraObject.AddComponent<PlayerFollowCamera2D>();
            SerializedObject serialized = new SerializedObject(follow);
            SerializedProperty targetProperty = serialized.FindProperty("target");
            targetProperty.objectReferenceValue = target;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateBounds(Transform parent, Vector2 size)
        {
            GameObject bounds = new GameObject("Scene Authoring Bounds");
            bounds.transform.SetParent(parent, false);
            BoxCollider2D box = bounds.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = size;

            GameObject movement = new GameObject("Movement Boundary");
            movement.transform.SetParent(parent, false);
            CreateBoundaryEdge(movement.transform, "Boundary Left",
                new Vector2(-size.x * 0.5f - 0.5f, 0f),
                new Vector2(1f, size.y + 2f));
            CreateBoundaryEdge(movement.transform, "Boundary Right",
                new Vector2(size.x * 0.5f + 0.5f, 0f),
                new Vector2(1f, size.y + 2f));
            CreateBoundaryEdge(movement.transform, "Boundary Bottom",
                new Vector2(0f, -size.y * 0.5f - 0.5f),
                new Vector2(size.x + 2f, 1f));
            CreateBoundaryEdge(movement.transform, "Boundary Top",
                new Vector2(0f, size.y * 0.5f + 0.5f),
                new Vector2(size.x + 2f, 1f));
        }

        private static void CreateBoundaryEdge(
            Transform parent,
            string name,
            Vector2 position,
            Vector2 size)
        {
            GameObject edge = new GameObject(name);
            edge.transform.SetParent(parent, false);
            edge.transform.localPosition = position;
            edge.AddComponent<BoxCollider2D>().size = size;
        }

        private static void CreateAuthoringMarker(Transform parent, string note)
        {
            GameObject marker = new GameObject("Modern Scene Authoring Note");
            marker.transform.SetParent(parent, false);
            marker.AddComponent<ModernSceneAuthoringMarker>().Configure(note);
        }

        private static void BackupIfPresent(string sourcePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(sourcePath) == null)
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder(BackupRoot))
            {
                AssetDatabase.CreateFolder("Assets/_Project", "SceneBackups");
            }

            string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string fileName = Path.GetFileNameWithoutExtension(sourcePath);
            string destination = $"{BackupRoot}/{fileName}-{stamp}.unity";
            if (!AssetDatabase.CopyAsset(sourcePath, destination))
            {
                throw new InvalidOperationException(
                    $"Could not back up '{sourcePath}' to '{destination}'.");
            }
        }

        private static bool IsOpen(string path)
        {
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                if (string.Equals(
                        SceneManager.GetSceneAt(index).path,
                        path,
                        StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static void EnsureBuildSettings()
        {
            string[] required =
            {
                ProjectSceneNames.BootstrapPath,
                ProjectSceneNames.FarmPath,
                ProjectSceneNames.HouseInteriorPath,
            };

            List<EditorBuildSettingsScene> scenes =
                EditorBuildSettings.scenes.ToList();
            foreach (string path in required)
            {
                int index = scenes.FindIndex(scene => scene.path == path);
                EditorBuildSettingsScene entry =
                    new EditorBuildSettingsScene(path, true);
                if (index < 0)
                {
                    scenes.Add(entry);
                }
                else
                {
                    scenes[index] = entry;
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }

    [DisallowMultipleComponent]
    public sealed class ModernSceneAuthoringMarker : MonoBehaviour
    {
        [SerializeField, TextArea(3, 8)]
        private string note;

        public string Note => note;

        public void Configure(string value)
        {
            note = value;
        }
    }
}
