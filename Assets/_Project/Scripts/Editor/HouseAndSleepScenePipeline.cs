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
            "farm-house-entry-scene-v1";
        public const string HouseImportSignature =
            "house-interior-sleep-scene-v1";

        private const string TileSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png";
        private const float HeroVisualScale = 1.5f;

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
            Dictionary<string, Sprite> sprites =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(
                        TileSheetPath)
                    .OfType<Sprite>()
                    .ToDictionary(
                        sprite => sprite.name,
                        StringComparer.Ordinal);

            sprites.TryGetValue("cozy_grass", out Sprite grass);
            sprites.TryGetValue("cozy_dirt", out Sprite dirt);
            GameObject player =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    PlayerPrefabAssetCatalog.PrefabAssetPath);

            assets = grass != null && dirt != null && player != null
                ? new SceneAssets(grass, dirt, player)
                : null;

            if (assets == null)
            {
                Debug.LogWarning(
                    "House scenes are waiting for cozy_grass, cozy_dirt " +
                    "and the Player prefab.");
            }

            return assets != null;
        }

        private static void BuildFarm(Scene scene, SceneAssets assets)
        {
            GameObject root = CreateRoot(scene, "Farm World");
            CreateCamera(scene, new Color32(111, 153, 96, 255));
            TilePatch(
                "Farm Grass",
                root.transform,
                assets.Grass,
                Vector2.zero,
                15,
                9,
                TopDownSortingLayers.Ground,
                -100,
                Color.white);

            Transform house = Group("Hero House Exterior", root.transform);
            house.localPosition = new Vector3(0f, 2.65f, 0f);
            TilePatch(
                "House Body",
                house,
                assets.Dirt,
                Vector2.zero,
                5,
                3,
                TopDownSortingLayers.World,
                10,
                new Color32(198, 151, 105, 255));
            TilePatch(
                "Roof",
                house,
                assets.Grass,
                new Vector2(0f, 1.35f),
                5,
                1,
                TopDownSortingLayers.World,
                20,
                new Color32(105, 76, 61, 255));
            SpriteObject(
                "Door",
                house,
                assets.Dirt,
                new Vector2(0f, -1.2f),
                TopDownSortingLayers.World,
                30,
                0.8f,
                new Color32(91, 61, 45, 255));

            BoxCollider2D houseCollider =
                house.gameObject.AddComponent<BoxCollider2D>();
            houseCollider.offset = new Vector2(0f, 0.1f);
            houseCollider.size = new Vector2(4.8f, 2.2f);

            Boundary(root.transform, new Vector2(14.4f, 8.2f));
            Spawn(
                root.transform,
                ProjectSpawnIds.FarmStart,
                new Vector2(0f, -2.8f),
                FacingDirection.Up);
            Spawn(
                root.transform,
                ProjectSpawnIds.FarmHouseDoor,
                new Vector2(0f, 0.25f),
                FacingDirection.Down);

            ScenePortal portal = Portal(
                "House Entrance Portal",
                root.transform,
                new Vector2(0f, 1.35f));
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

        private static void BuildHouse(Scene scene, SceneAssets assets)
        {
            GameObject root = CreateRoot(scene, "House Interior World");
            CreateCamera(scene, new Color32(55, 42, 36, 255));
            TilePatch(
                "Interior Floor",
                root.transform,
                assets.Dirt,
                Vector2.zero,
                9,
                6,
                TopDownSortingLayers.Ground,
                -100,
                new Color32(211, 174, 124, 255));
            TilePatch(
                "Back Wall",
                root.transform,
                assets.Grass,
                new Vector2(0f, 2.5f),
                9,
                1,
                TopDownSortingLayers.World,
                20,
                new Color32(128, 91, 68, 255));
            TilePatch(
                "Left Wall",
                root.transform,
                assets.Grass,
                new Vector2(-4f, 0f),
                1,
                6,
                TopDownSortingLayers.World,
                20,
                new Color32(128, 91, 68, 255));
            TilePatch(
                "Right Wall",
                root.transform,
                assets.Grass,
                new Vector2(4f, 0f),
                1,
                6,
                TopDownSortingLayers.World,
                20,
                new Color32(128, 91, 68, 255));

            Boundary(root.transform, new Vector2(8.2f, 5.3f));
            Spawn(
                root.transform,
                ProjectSpawnIds.HouseEntrance,
                new Vector2(0f, -1.55f),
                FacingDirection.Up);
            Spawn(
                root.transform,
                ProjectSpawnIds.HouseBedWake,
                new Vector2(1.55f, 0.8f),
                FacingDirection.Right);

            ScenePortal exit = Portal(
                "House Exit Portal",
                root.transform,
                new Vector2(0f, -2.25f));
            exit.Configure(
                "Salir a la granja",
                ProjectSceneNames.Farm,
                ProjectSpawnIds.FarmHouseDoor);
            SpriteObject(
                "Interior Door",
                root.transform,
                assets.Dirt,
                new Vector2(0f, -2.45f),
                TopDownSortingLayers.World,
                25,
                0.8f,
                new Color32(91, 61, 45, 255));

            CreateBed(
                root.transform,
                assets,
                new Vector2(2.75f, 0.8f));
            TilePatch(
                "Small Table",
                root.transform,
                assets.Dirt,
                new Vector2(-2.65f, 1.15f),
                2,
                1,
                TopDownSortingLayers.World,
                15,
                new Color32(113, 76, 54, 255));

            Player(
                scene,
                root.transform,
                assets.Player,
                ProjectSpawnIds.HouseEntrance,
                new Vector2(0f, -1.55f));
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
                assets.Dirt,
                Vector2.zero,
                TopDownSortingLayers.World,
                30,
                1.45f,
                new Color32(113, 76, 54, 255));
            SpriteObject(
                "Bed Blanket",
                bed,
                assets.Grass,
                new Vector2(0f, 0.12f),
                TopDownSortingLayers.World,
                31,
                1.15f,
                new Color32(111, 146, 174, 255));

            BoxCollider2D collider =
                bed.gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1.35f, 1.1f);

            BedInteractable sleep =
                bed.gameObject.AddComponent<BedInteractable>();
            sleep.Configure(
                "Dormir hasta mañana",
                ProjectSpawnIds.HouseBedWake);
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
                    new Vector2(-x, -y)
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
            int order,
            Color tint)
        {
            Transform patch = Group(name, parent);
            patch.localPosition = center;
            float startX = -(columns - 1) * 0.5f;
            float startY = -(rows - 1) * 0.5f;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    SpriteObject(
                        $"{name} {x}-{y}",
                        patch,
                        sprite,
                        new Vector2(startX + x, startY + y),
                        layer,
                        order,
                        1f,
                        tint);
                }
            }
        }

        private static void SpriteObject(
            string name,
            Transform parent,
            Sprite sprite,
            Vector2 position,
            string layer,
            int order,
            float scale,
            Color tint)
        {
            Transform go = Group(name, parent);
            go.localPosition = position;
            go.localScale = new Vector3(scale, scale, 1f);
            SpriteRenderer renderer =
                go.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingLayerName = layer;
            renderer.sortingOrder = order;
            renderer.color = tint;
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
            public SceneAssets(
                Sprite grass,
                Sprite dirt,
                GameObject player)
            {
                Grass = grass;
                Dirt = dirt;
                Player = player;
            }

            public Sprite Grass { get; }
            public Sprite Dirt { get; }
            public GameObject Player { get; }
        }
    }
}
