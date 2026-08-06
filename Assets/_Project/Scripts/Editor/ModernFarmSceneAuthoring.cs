using System;
using System.IO;
using System.Linq;
using FarmSimulator.Application.Player;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Player;
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
            GenerateScene(ProjectSceneNames.FarmPath, BuildFarm, false);
            GenerateScene(ProjectSceneNames.HouseInteriorPath, BuildHouse, false);
            EnsureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem(MenuRoot + "Replace Farm + HouseInterior (with backup)")]
        public static void ReplaceScenesWithBackup()
        {
            if (!EditorUtility.DisplayDialog(
                    "Replace Farm and HouseInterior",
                    "This creates timestamped backups of existing scenes and then " +
                    "rebuilds both scenes with the modern non-Legacy authoring layout. " +
                    "The old reset pipeline is not used.",
                    "Back up and replace",
                    "Cancel"))
            {
                return;
            }

            BackupIfPresent(ProjectSceneNames.FarmPath);
            BackupIfPresent(ProjectSceneNames.HouseInteriorPath);
            GenerateScene(ProjectSceneNames.FarmPath, BuildFarm, true);
            GenerateScene(ProjectSceneNames.HouseInteriorPath, BuildHouse, true);
            EnsureBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void GenerateScene(
            string path,
            Action<Scene> builder,
            bool replace)
        {
            if (!replace && AssetDatabase.LoadAssetAtPath<SceneAsset>(path) != null)
            {
                return;
            }

            if (IsOpen(path))
            {
                throw new InvalidOperationException(
                    $"Close '{path}' before regenerating it.");
            }

            if (replace)
            {
                AssetDatabase.DeleteAsset(path);
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
        }

        private static void BuildFarm(Scene scene)
        {
            GameObject root = CreateRoot(scene, "Farm World");
            GameObject gridObject = new GameObject("Farm Authoring Grid");
            SceneManager.MoveGameObjectToScene(gridObject, scene);
            gridObject.transform.SetParent(root.transform, false);
            gridObject.AddComponent<Grid>();

            Tilemap ground = CreateTilemap(gridObject.transform, "Ground", -100);
            CreateTilemap(gridObject.transform, "Paths", -90);
            CreateTilemap(gridObject.transform, "Soil", -80);
            CreateTilemap(gridObject.transform, "Decoration", 0);

            TileBase grass = FindTile("grass");
            if (grass != null)
            {
                FillRectangle(ground, new BoundsInt(-24, -16, 0, 48, 32, 1), grass);
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

            CreateCamera(scene, player.transform, 9f, new Color32(90, 135, 82, 255));
            CreateBounds(root.transform, new Vector2(48f, 32f));
            CreateAuthoringMarker(root.transform,
                "Farm scene rebuilt with modern Tilemap layers. " +
                "Paint using the current seasonal Tile Browser / Tile Palette.");
        }

        private static void BuildHouse(Scene scene)
        {
            GameObject root = CreateRoot(scene, "House Interior World");
            GameObject gridObject = new GameObject("House Authoring Grid");
            SceneManager.MoveGameObjectToScene(gridObject, scene);
            gridObject.transform.SetParent(root.transform, false);
            gridObject.AddComponent<Grid>();

            Tilemap ground = CreateTilemap(gridObject.transform, "Ground", -100);
            CreateTilemap(gridObject.transform, "Walls", -20);
            CreateTilemap(gridObject.transform, "Decoration", 0);

            TileBase floor = FindTile("wood") ?? FindTile("floor");
            if (floor != null)
            {
                FillRectangle(ground, new BoundsInt(-8, -5, 0, 16, 10, 1), floor);
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
            BedInteractable interactable = bed.AddComponent<BedInteractable>();
            interactable.Configure("Dormir hasta mañana", ProjectSpawnIds.HouseBedWake);

            Sprite bedSprite = FindSprite("bed");
            if (bedSprite != null)
            {
                bed.AddComponent<SpriteRenderer>().sprite = bedSprite;
            }

            CreateCamera(scene, player.transform, 6f, new Color32(48, 38, 32, 255));
            CreateBounds(root.transform, new Vector2(16f, 10f));
            CreateAuthoringMarker(root.transform,
                "HouseInterior rebuilt with modern authoring layers. " +
                "Decorate from the current Cozy Interior library.");
        }

        private static GameObject CreateRoot(Scene scene, string name)
        {
            GameObject root = new GameObject(name);
            SceneManager.MoveGameObjectToScene(root, scene);
            return root;
        }

        private static Tilemap CreateTilemap(Transform parent, string name, int order)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(parent, false);
            Tilemap tilemap = child.AddComponent<Tilemap>();
            TilemapRenderer renderer = child.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = order;
            return tilemap;
        }

        private static void FillRectangle(Tilemap tilemap, BoundsInt bounds, TileBase tile)
        {
            foreach (Vector3Int position in bounds.allPositionsWithin)
            {
                tilemap.SetTile(position, tile);
            }
        }

        private static TileBase FindTile(string token)
        {
            string[] guids = AssetDatabase.FindAssets("t:TileBase");
            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.IndexOf("SeasonalTiles", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    path.IndexOf("Tile", StringComparison.OrdinalIgnoreCase) >= 0)
                .Where(path => path.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(path => AssetDatabase.LoadAssetAtPath<TileBase>(path))
                .FirstOrDefault(tile => tile != null);
        }

        private static Sprite FindSprite(string token)
        {
            string[] guids = AssetDatabase.FindAssets("t:Sprite");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.IndexOf("CozyInterior", StringComparison.OrdinalIgnoreCase) < 0 ||
                    path.IndexOf(token, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                Sprite sprite = AssetDatabase.LoadAllAssetsAtPath(path)
                    .OfType<Sprite>()
                    .FirstOrDefault(candidate =>
                        candidate.name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0);
                if (sprite != null)
                {
                    return sprite;
                }
            }

            return null;
        }

        private static GameObject CreatePlayer(
            Scene scene,
            Transform parent,
            Vector2 position)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (prefab == null)
            {
                throw new InvalidOperationException(
                    $"Player prefab missing at '{PlayerPrefabPath}'.");
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
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
            string scene,
            string spawnId)
        {
            GameObject portalObject = new GameObject(name);
            portalObject.transform.SetParent(parent, false);
            portalObject.transform.localPosition = position;
            BoxCollider2D collider = portalObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1.5f, 1f);
            portalObject.AddComponent<ScenePortal>()
                .Configure(prompt, scene, spawnId);
        }

        private static void CreateCamera(
            Scene scene,
            Transform target,
            float orthographicSize,
            Color background)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = orthographicSize;
            camera.backgroundColor = background;
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            PlayerFollowCamera2D follow = cameraObject.AddComponent<PlayerFollowCamera2D>();
            SerializedObject serialized = new SerializedObject(follow);
            SerializedProperty targetProperty = serialized.FindProperty("target");
            if (targetProperty != null)
            {
                targetProperty.objectReferenceValue = target;
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
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
                new Vector2(-size.x * 0.5f - 0.5f, 0f), new Vector2(1f, size.y + 2f));
            CreateBoundaryEdge(movement.transform, "Boundary Right",
                new Vector2(size.x * 0.5f + 0.5f, 0f), new Vector2(1f, size.y + 2f));
            CreateBoundaryEdge(movement.transform, "Boundary Bottom",
                new Vector2(0f, -size.y * 0.5f - 0.5f), new Vector2(size.x + 2f, 1f));
            CreateBoundaryEdge(movement.transform, "Boundary Top",
                new Vector2(0f, size.y * 0.5f + 0.5f), new Vector2(size.x + 2f, 1f));
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
                Scene scene = SceneManager.GetSceneAt(index);
                if (string.Equals(scene.path, path, StringComparison.Ordinal))
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

            var scenes = EditorBuildSettings.scenes.ToList();
            foreach (string path in required)
            {
                int index = scenes.FindIndex(scene => scene.path == path);
                if (index < 0)
                {
                    scenes.Add(new EditorBuildSettingsScene(path, true));
                }
                else
                {
                    scenes[index] = new EditorBuildSettingsScene(path, true);
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }

    [DisallowMultipleComponent]
    public sealed class ModernSceneAuthoringMarker : MonoBehaviour
    {
        [SerializeField]
        [TextArea(3, 8)]
        private string note;

        public string Note => note;

        public void Configure(string value)
        {
            note = value;
        }
    }
}
