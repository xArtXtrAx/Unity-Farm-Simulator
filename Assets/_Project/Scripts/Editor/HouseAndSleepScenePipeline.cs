using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Application.Player;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Application.Spatial;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Interaction;
using FarmSimulator.Presentation.Player;
using FarmSimulator.Presentation.Scenes;
using FarmSimulator.Presentation.Time;
using FarmSimulator.Presentation.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class HouseAndSleepScenePipeline
    {
        public const string FarmImportSignature =
            "farm-house-entry-scene-v2";
        public const string HouseImportSignature =
            "house-interior-sleep-scene-v2";

        private const float HeroVisualScale = 1.5f;

        private static readonly string[] RequiredSpriteNames =
        {
            "cozy_grass",
            "cozy_dirt",
            "cozy_wood_panel_light",
            "cozy_wood_panel_dark",
            "cozy_bench_dark",
            "cozy_bench_light",
            "cozy_flower_crates",
            "cozy_crates_dark",
            "cozy_crates_light",
            "cozy_lamp_green",
            "cozy_fence_horizontal",
            "cozy_bridge_wood",
            "cozy_bush_row",
            "cozy_tree_spring",
            "cozy_rock_row",
        };

        static HouseAndSleepScenePipeline()
        {
            EditorApplication.delayCall += EnsureScenes;
        }

        [MenuItem("Tools/Farm Simulator/Rebuild House and Sleep Scenes")]
        public static void RebuildScenes()
        {
            if (IsOpen(ProjectSceneNames.FarmPath) ||
                IsOpen(ProjectSceneNames.HouseInteriorPath))
            {
                Debug.LogWarning(
                    "Close Farm and HouseInterior before rebuilding them.");
                return;
            }

            AssetDatabase.DeleteAsset(ProjectSceneNames.FarmPath);
            AssetDatabase.DeleteAsset(ProjectSceneNames.HouseInteriorPath);
            Generate(force: true);
        }

        public static void EnsureScenes()
        {
            if (EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureScenes;
                return;
            }

            Generate(force: false);
        }

        private static void Generate(bool force)
        {
            if (!TryLoadAssets(out SceneAssets assets))
            {
                EditorApplication.delayCall += EnsureScenes;
                return;
            }

            if (force || !IsCurrent(
                    ProjectSceneNames.FarmPath,
                    FarmImportSignature))
            {
                SaveGeneratedScene(
                    ProjectSceneNames.FarmPath,
                    FarmImportSignature,
                    scene => BuildFarm(scene, assets));
            }

            if (force || !IsCurrent(
                    ProjectSceneNames.HouseInteriorPath,
                    HouseImportSignature))
            {
                SaveGeneratedScene(
                    ProjectSceneNames.HouseInteriorPath,
                    HouseImportSignature,
                    scene => BuildHouse(scene, assets));
            }

            EnsureBuildSettings();
        }

        private static bool TryLoadAssets(out SceneAssets assets)
        {
            CozyFarmHouseArtPipeline.EnsureAssets();

            Dictionary<string, Sprite> available =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(
                        CozyFarmHouseArtPipeline.TileSheetAssetPath)
                    .OfType<Sprite>()
                    .ToDictionary(
                        sprite => sprite.name,
                        StringComparer.Ordinal);

            var selected = new Dictionary<string, Sprite>(
                StringComparer.Ordinal);
            foreach (string spriteName in RequiredSpriteNames)
            {
                if (!available.TryGetValue(
                        spriteName,
                        out Sprite sprite))
                {
                    assets = null;
                    Debug.LogWarning(
                        $"House scenes are waiting for '{spriteName}'.");
                    return false;
                }

                selected[spriteName] = sprite;
            }

            GameObject player =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    PlayerPrefabAssetCatalog.PrefabAssetPath);
            if (player == null)
            {
                assets = null;
                Debug.LogWarning(
                    "House scenes are waiting for the Player prefab.");
                return false;
            }

            assets = new SceneAssets(player, selected);
            return true;
        }

        private static void BuildFarm(Scene scene, SceneAssets assets)
        {
            GameObject root = CreateRoot(scene, "Farm World");
            CreateCamera(scene, new Color32(111, 153, 96, 255));

            TilePatch(
                "Farm Grass",
                root.transform,
                assets["cozy_grass"],
                Vector2.zero,
                15,
                9,
                TopDownSortingLayers.Ground,
                -100);
            TilePatch(
                "House Path",
                root.transform,
                assets["cozy_dirt"],
                new Vector2(0f, -1.35f),
                3,
                4,
                TopDownSortingLayers.Ground,
                -90);

            CreateFarmHouse(root.transform, assets);
            CreateFarmDecor(root.transform, assets);

            Boundary(root.transform, new Vector2(14.4f, 8.2f));
            Spawn(
                root.transform,
                ProjectSpawnIds.FarmStart,
                new Vector2(0f, -2.8f),
                FacingDirection.Up);
            Spawn(
                root.transform,
                ProjectSpawnIds.FarmHouseDoor,
                new Vector2(0f, 0f),
                FacingDirection.Down);

            ScenePortal portal = Portal(
                "House Entrance Portal",
                root.transform,
                new Vector2(0f, 0.95f));
            portal.Configure(
                "Entrar a la casa",
                ProjectSceneNames.HouseInterior,
                ProjectSpawnIds.HouseEntrance);

            Player(
                scene,
                root.transform,
                assets.Player,
                ProjectSpawnIds.FarmStart,
                new Vector2(0f, -2.8f));
            Hud(root.transform);
        }

        private static void CreateFarmHouse(
            Transform parent,
            SceneAssets assets)
        {
            Transform house = Group("Hero House Exterior", parent);
            house.localPosition = new Vector3(0f, 2.45f, 0f);

            SpritePatch(
                "Cabin Wall Panels",
                house,
                assets["cozy_wood_panel_light"],
                new Vector2(0f, -0.05f),
                3,
                2,
                new Vector2(1.7f, 1.48f),
                TopDownSortingLayers.World,
                10,
                Vector2.one);
            SpritePatch(
                "Cabin Roof Panels",
                house,
                assets["cozy_wood_panel_dark"],
                new Vector2(0f, 1.28f),
                3,
                1,
                new Vector2(1.72f, 1f),
                TopDownSortingLayers.World,
                20,
                new Vector2(1f, 0.78f));

            SpriteObject(
                "Cabin Door",
                house,
                assets["cozy_wood_panel_dark"],
                new Vector2(0f, -0.92f),
                TopDownSortingLayers.World,
                30,
                new Vector2(0.42f, 0.72f));
            SpriteObject(
                "Flower Window Box",
                house,
                assets["cozy_flower_crates"],
                new Vector2(-1.45f, -0.55f),
                TopDownSortingLayers.World,
                31,
                new Vector2(0.85f, 0.85f));
            SpriteObject(
                "Cabin Eave",
                house,
                assets["cozy_bench_dark"],
                new Vector2(0f, 1.83f),
                TopDownSortingLayers.World,
                32,
                new Vector2(1.65f, 0.65f));
            SpriteObject(
                "Front Porch",
                house,
                assets["cozy_bridge_wood"],
                new Vector2(0f, -1.63f),
                TopDownSortingLayers.World,
                25,
                new Vector2(0.55f, 0.68f));

            BoxCollider2D houseCollider =
                house.gameObject.AddComponent<BoxCollider2D>();
            houseCollider.offset = new Vector2(0f, 0.18f);
            houseCollider.size = new Vector2(5.35f, 2.75f);
        }

        private static void CreateFarmDecor(
            Transform parent,
            SceneAssets assets)
        {
            SpriteObject(
                "Left Orchard Tree",
                parent,
                assets["cozy_tree_spring"],
                new Vector2(-5.45f, 0.15f),
                TopDownSortingLayers.World,
                4,
                new Vector2(1.25f, 1.25f));
            SpriteObject(
                "Right Orchard Tree",
                parent,
                assets["cozy_tree_spring"],
                new Vector2(5.35f, 0.55f),
                TopDownSortingLayers.World,
                4,
                new Vector2(1.15f, 1.15f));
            SpriteObject(
                "Back Garden Bushes",
                parent,
                assets["cozy_bush_row"],
                new Vector2(0f, 3.72f),
                TopDownSortingLayers.World,
                5,
                new Vector2(1.15f, 1.15f));
            SpriteObject(
                "Garden Bench",
                parent,
                assets["cozy_bench_light"],
                new Vector2(3.25f, 0.45f),
                TopDownSortingLayers.World,
                12,
                Vector2.one);
            SpriteObject(
                "Garden Lamp",
                parent,
                assets["cozy_lamp_green"],
                new Vector2(-3.35f, -0.35f),
                TopDownSortingLayers.World,
                12,
                new Vector2(0.9f, 0.9f));
            SpriteObject(
                "Rock Border",
                parent,
                assets["cozy_rock_row"],
                new Vector2(-3.9f, -2.55f),
                TopDownSortingLayers.World,
                6,
                new Vector2(0.85f, 0.85f));
            SpriteObject(
                "Fence Border",
                parent,
                assets["cozy_fence_horizontal"],
                new Vector2(4.4f, -2.45f),
                TopDownSortingLayers.World,
                6,
                new Vector2(1.5f, 1.15f));
        }

        private static void BuildHouse(Scene scene, SceneAssets assets)
        {
            GameObject root = CreateRoot(scene, "House Interior World");
            CreateCamera(scene, new Color32(48, 38, 32, 255));

            SpritePatch(
                "Wood Floor",
                root.transform,
                assets["cozy_wood_panel_light"],
                Vector2.zero,
                5,
                3,
                new Vector2(1.7f, 1.5f),
                TopDownSortingLayers.Ground,
                -100,
                Vector2.one);
            SpritePatch(
                "Back Interior Wall",
                root.transform,
                assets["cozy_wood_panel_dark"],
                new Vector2(0f, 2.3f),
                5,
                1,
                new Vector2(1.7f, 1f),
                TopDownSortingLayers.World,
                10,
                new Vector2(1f, 0.72f));
            SpritePatch(
                "Left Interior Wall",
                root.transform,
                assets["cozy_wood_panel_dark"],
                new Vector2(-4.12f, 0f),
                1,
                3,
                new Vector2(1f, 1.48f),
                TopDownSortingLayers.World,
                10,
                new Vector2(0.55f, 1f));
            SpritePatch(
                "Right Interior Wall",
                root.transform,
                assets["cozy_wood_panel_dark"],
                new Vector2(4.12f, 0f),
                1,
                3,
                new Vector2(1f, 1.48f),
                TopDownSortingLayers.World,
                10,
                new Vector2(0.55f, 1f));

            Boundary(root.transform, new Vector2(8.35f, 5.35f));
            Spawn(
                root.transform,
                ProjectSpawnIds.HouseEntrance,
                new Vector2(0f, -1.45f),
                FacingDirection.Up);
            Spawn(
                root.transform,
                ProjectSpawnIds.HouseBedWake,
                new Vector2(1.45f, 0.55f),
                FacingDirection.Right);

            ScenePortal exit = Portal(
                "House Exit Portal",
                root.transform,
                new Vector2(0f, -2.08f));
            exit.Configure(
                "Salir a la granja",
                ProjectSceneNames.Farm,
                ProjectSpawnIds.FarmHouseDoor);
            SpriteObject(
                "Interior Door",
                root.transform,
                assets["cozy_wood_panel_dark"],
                new Vector2(0f, -2.28f),
                TopDownSortingLayers.World,
                25,
                new Vector2(0.42f, 0.68f));

            CreateBed(
                root.transform,
                assets,
                new Vector2(2.75f, 0.55f));
            CreateInteriorFurniture(root.transform, assets);

            Player(
                scene,
                root.transform,
                assets.Player,
                ProjectSpawnIds.HouseEntrance,
                new Vector2(0f, -1.45f));
            Hud(root.transform);
        }

        private static void CreateBed(
            Transform parent,
            SceneAssets assets,
            Vector2 position)
        {
            Transform bed = Group("Hero Bed", parent);
            bed.localPosition = position;

            SpriteObject(
                "Bed Frame",
                bed,
                assets["cozy_wood_panel_dark"],
                Vector2.zero,
                TopDownSortingLayers.World,
                30,
                new Vector2(0.7f, 1.15f));
            SpriteObject(
                "Bed Mattress",
                bed,
                assets["cozy_wood_panel_light"],
                new Vector2(0f, 0.02f),
                TopDownSortingLayers.World,
                31,
                new Vector2(0.54f, 0.94f));
            SpriteObject(
                "Bed Pillow",
                bed,
                assets["cozy_bench_light"],
                new Vector2(0f, 0.62f),
                TopDownSortingLayers.World,
                32,
                new Vector2(0.38f, 0.42f));
            SpriteObject(
                "Bed Footboard",
                bed,
                assets["cozy_fence_horizontal"],
                new Vector2(0f, -0.72f),
                TopDownSortingLayers.World,
                33,
                new Vector2(0.65f, 0.8f));

            BoxCollider2D collider =
                bed.gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1.35f, 2f);

            BedInteractable sleep =
                bed.gameObject.AddComponent<BedInteractable>();
            sleep.Configure(
                "Dormir hasta mañana",
                ProjectSpawnIds.HouseBedWake);
        }

        private static void CreateInteriorFurniture(
            Transform parent,
            SceneAssets assets)
        {
            SpriteObject(
                "Flower Cabinet",
                parent,
                assets["cozy_flower_crates"],
                new Vector2(-2.85f, 1.15f),
                TopDownSortingLayers.World,
                20,
                new Vector2(0.95f, 0.95f));
            SpriteObject(
                "Storage Crates",
                parent,
                assets["cozy_crates_dark"],
                new Vector2(-2.75f, 0.05f),
                TopDownSortingLayers.World,
                20,
                Vector2.one);
            SpriteObject(
                "Reading Bench",
                parent,
                assets["cozy_bench_light"],
                new Vector2(-2.45f, -1.15f),
                TopDownSortingLayers.World,
                20,
                new Vector2(1.05f, 1.05f));
            SpriteObject(
                "Interior Lamp",
                parent,
                assets["cozy_lamp_green"],
                new Vector2(-3.45f, -0.7f),
                TopDownSortingLayers.World,
                22,
                new Vector2(0.82f, 0.82f));
            SpriteObject(
                "Woven Floor Runner",
                parent,
                assets["cozy_bridge_wood"],
                new Vector2(-0.15f, 0.85f),
                TopDownSortingLayers.Ground,
                -80,
                new Vector2(0.58f, 0.72f));
            SpriteObject(
                "Bedside Crates",
                parent,
                assets["cozy_crates_light"],
                new Vector2(2.75f, -1.05f),
                TopDownSortingLayers.World,
                20,
                new Vector2(0.75f, 0.75f));
        }

        private static void Player(
            Scene scene,
            Transform parent,
            GameObject prefab,
            string defaultSpawn,
            Vector2 position)
        {
            GameObject player =
                PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (player == null)
            {
                throw new InvalidOperationException(
                    "Could not instantiate Player prefab.");
            }

            player.name = "Player";
            player.transform.SetParent(parent, true);
            player.transform.position = position;
            player.transform.localScale = Vector3.one;

            Transform visual = player.transform.Find(
                PlayerSpriteAssetCatalog.SpriteVisualObjectName);
            if (visual == null)
            {
                throw new InvalidOperationException(
                    "Player prefab is missing its sprite visual.");
            }

            visual.localScale =
                new Vector3(HeroVisualScale, HeroVisualScale, 1f);
            player.AddComponent<PlayerInteractionController>();
            player.AddComponent<SceneSpawnResolver>()
                .Configure(defaultSpawn);
        }

        private static void Hud(Transform parent)
        {
            Font font = Resources.GetBuiltinResource<Font>(
                "LegacyRuntime.ttf");
            if (font == null)
            {
                throw new InvalidOperationException(
                    "LegacyRuntime.ttf is unavailable.");
            }

            var root = new GameObject(
                "World HUD",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            root.transform.SetParent(parent, false);

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 900;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960f, 540f);
            scaler.matchWidthOrHeight = 0.5f;

            Text day = TextObject(
                "Day Label",
                root.transform,
                font,
                18,
                TextAnchor.UpperLeft);
            SetRect(
                day.rectTransform,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(20f, -18f),
                new Vector2(320f, 40f));
            root.AddComponent<DayLabelView>().Configure(day);

            Text prompt = TextObject(
                "Interaction Prompt",
                root.transform,
                font,
                18,
                TextAnchor.MiddleCenter);
            SetRect(
                prompt.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 92f),
                new Vector2(500f, 42f));
            root.AddComponent<InteractionPromptView>()
                .Configure(prompt);
        }

        private static Text TextObject(
            string name,
            Transform parent,
            Font font,
            int size,
            TextAnchor alignment)
        {
            var go = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text),
                typeof(Outline));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = new Color32(255, 244, 213, 255);
            text.raycastTarget = false;
            Outline outline = go.GetComponent<Outline>();
            outline.effectColor = new Color32(45, 34, 29, 220);
            outline.effectDistance = new Vector2(1f, -1f);
            return text;
        }

        private static void SetRect(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 position,
            Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static ScenePortal Portal(
            string name,
            Transform parent,
            Vector2 position)
        {
            Transform portal = Group(name, parent);
            portal.localPosition = position;
            return portal.gameObject.AddComponent<ScenePortal>();
        }

        private static void Spawn(
            Transform parent,
            string id,
            Vector2 position,
            FacingDirection facing)
        {
            Transform spawn = Group($"Spawn {id}", parent);
            spawn.localPosition = position;
            spawn.gameObject.AddComponent<SceneSpawnPoint>()
                .Configure(id, facing);
        }

        private static void Boundary(Transform parent, Vector2 size)
        {
            Transform boundary = Group("Movement Boundary", parent);
            float x = size.x * 0.5f;
            float y = size.y * 0.5f;
            boundary.gameObject.AddComponent<EdgeCollider2D>().points =
                new[]
                {
                    new Vector2(-x, -y),
                    new Vector2(-x, y),
                    new Vector2(x, y),
                    new Vector2(x, -y),
                    new Vector2(-x, -y),
                };
        }

        private static void CreateCamera(Scene scene, Color background)
        {
            var go = new GameObject("Main Camera");
            SceneManager.MoveGameObjectToScene(go, scene);
            go.tag = "MainCamera";
            go.transform.position =
                new Vector3(0f, 0f, SpatialModel.CameraDepth);
            Camera camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize =
                SpatialModel.CameraOrthographicSize;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = background;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            go.AddComponent<AudioListener>();
            go.AddComponent<ReferenceAspectCamera>();
        }

        private static GameObject CreateRoot(Scene scene, string name)
        {
            var root = new GameObject(name);
            SceneManager.MoveGameObjectToScene(root, scene);
            return root;
        }

        private static Transform Group(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static void TilePatch(
            string name,
            Transform parent,
            Sprite sprite,
            Vector2 center,
            int columns,
            int rows,
            string layer,
            int order)
        {
            SpritePatch(
                name,
                parent,
                sprite,
                center,
                columns,
                rows,
                Vector2.one,
                layer,
                order,
                Vector2.one);
        }

        private static void SpritePatch(
            string name,
            Transform parent,
            Sprite sprite,
            Vector2 center,
            int columns,
            int rows,
            Vector2 spacing,
            string layer,
            int order,
            Vector2 scale)
        {
            Transform patch = Group(name, parent);
            patch.localPosition = center;
            float startX = -(columns - 1) * spacing.x * 0.5f;
            float startY = -(rows - 1) * spacing.y * 0.5f;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    SpriteObject(
                        $"{name} {x}-{y}",
                        patch,
                        sprite,
                        new Vector2(
                            startX + x * spacing.x,
                            startY + y * spacing.y),
                        layer,
                        order,
                        scale);
                }
            }
        }

        private static Transform SpriteObject(
            string name,
            Transform parent,
            Sprite sprite,
            Vector2 position,
            string layer,
            int order,
            Vector2 scale,
            float rotationDegrees = 0f)
        {
            Transform go = Group(name, parent);
            go.localPosition = position;
            go.localScale = new Vector3(scale.x, scale.y, 1f);
            go.localRotation = Quaternion.Euler(
                0f,
                0f,
                rotationDegrees);
            SpriteRenderer renderer =
                go.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingLayerName = layer;
            renderer.sortingOrder = order;
            renderer.color = Color.white;
            return go;
        }

        private static void SaveGeneratedScene(
            string path,
            string signature,
            Action<Scene> build)
        {
            if (IsOpen(path))
            {
                Debug.LogWarning($"Close '{path}' before regenerating it.");
                return;
            }

            AssetDatabase.DeleteAsset(path);
            Scene previous = SceneManager.GetActiveScene();
            Scene generated = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Additive);

            try
            {
                SceneManager.SetActiveScene(generated);
                build(generated);
                if (!EditorSceneManager.SaveScene(generated, path))
                {
                    throw new InvalidOperationException(
                        $"Could not save '{path}'.");
                }
            }
            finally
            {
                if (previous.IsValid() && previous.isLoaded)
                {
                    SceneManager.SetActiveScene(previous);
                }

                if (generated.IsValid() && generated.isLoaded)
                {
                    EditorSceneManager.CloseScene(generated, true);
                }
            }

            AssetDatabase.ImportAsset(
                path,
                ImportAssetOptions.ForceSynchronousImport);
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.userData = signature;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static bool IsCurrent(string path, string signature)
        {
            AssetImporter importer = AssetImporter.GetAtPath(path);
            return importer != null && importer.userData == signature;
        }

        private static bool IsOpen(string path)
        {
            Scene scene = SceneManager.GetSceneByPath(path);
            return scene.IsValid() && scene.isLoaded;
        }

        private static void EnsureBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            EnsureEnabled(scenes, ProjectSceneNames.FarmPath);
            EnsureEnabled(scenes, ProjectSceneNames.HouseInteriorPath);
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        private static void EnsureEnabled(
            IList<EditorBuildSettingsScene> scenes,
            string path)
        {
            for (int index = 0; index < scenes.Count; index++)
            {
                if (!string.Equals(
                        scenes[index].path,
                        path,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                if (!scenes[index].enabled)
                {
                    scenes[index] =
                        new EditorBuildSettingsScene(path, true);
                }

                return;
            }

            scenes.Add(new EditorBuildSettingsScene(path, true));
        }

        private sealed class SceneAssets
        {
            private readonly IReadOnlyDictionary<string, Sprite> sprites;

            public SceneAssets(
                GameObject player,
                IReadOnlyDictionary<string, Sprite> curatedSprites)
            {
                Player = player;
                sprites = curatedSprites;
            }

            public GameObject Player { get; }

            public Sprite this[string name] => sprites[name];
        }
    }
}
