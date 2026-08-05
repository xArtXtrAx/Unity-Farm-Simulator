using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class CozyFarmShowcaseScenePipeline
    {
        public const string SceneAssetPath =
            "Assets/_Project/Scenes/CozyFarmShowcase.unity";
        public const string ImportSignature =
            "cozy-farm-showcase-scene-v1";
        public const string RootObjectName = "Cozy Farm Showcase";
        public const string CameraObjectName = "Showcase Camera";
        public const string TerrainGroupName = "Terrain Samples";
        public const string CatalogGroupName = "Catalog Icons";
        public const string CropGroupName = "Crop Growth Stages";
        public const string HeroObjectName = "Current Hero";

        private const string SourceRoot =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/";

        private static readonly string[] ItemNames =
        {
            "cozy_turnip",
            "cozy_carrot",
            "cozy_cabbage"
        };

        private static readonly string[] SeedNames =
        {
            "cozy_turnip_seeds",
            "cozy_carrot_seeds",
            "cozy_cabbage_seeds"
        };

        private static readonly string[] CropNames =
        {
            "turnip",
            "carrot",
            "cabbage"
        };

        private static readonly string[] TerrainNames =
        {
            "cozy_grass",
            "cozy_dirt",
            "cozy_water",
            "cozy_tilled_soil"
        };

        static CozyFarmShowcaseScenePipeline()
        {
            EditorApplication.delayCall += EnsureScene;
        }

        [MenuItem("Tools/Farm Simulator/Rebuild Cozy Farm Showcase")]
        public static void RebuildScene()
        {
            Scene loadedScene = SceneManager.GetSceneByPath(SceneAssetPath);
            if (loadedScene.IsValid() && loadedScene.isLoaded)
            {
                Debug.LogWarning(
                    "Close CozyFarmShowcase before rebuilding it.");
                return;
            }

            CreateOrReplaceScene();
        }

        public static void EnsureScene()
        {
            if (EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureScene;
                return;
            }

            if (IsSceneCurrent())
            {
                return;
            }

            CreateOrReplaceScene();
        }

        private static bool IsSceneCurrent()
        {
            AssetImporter importer =
                AssetImporter.GetAtPath(SceneAssetPath);
            return importer != null &&
                importer.userData == ImportSignature;
        }

        private static void CreateOrReplaceScene()
        {
            if (!TryLoadAssets(out ShowcaseAssets assets))
            {
                Debug.LogError(
                    "Cozy Farm showcase assets are incomplete. " +
                    "Confirm the curated sprites and Player prefab exist.");
                return;
            }

            Scene previousActiveScene = SceneManager.GetActiveScene();
            Scene showcaseScene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Additive);

            try
            {
                SceneManager.SetActiveScene(showcaseScene);
                BuildShowcase(showcaseScene, assets);

                if (!EditorSceneManager.SaveScene(
                        showcaseScene,
                        SceneAssetPath))
                {
                    throw new InvalidOperationException(
                        "Unity could not save the Cozy Farm showcase scene.");
                }
            }
            finally
            {
                if (previousActiveScene.IsValid() &&
                    previousActiveScene.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActiveScene);
                }

                if (showcaseScene.IsValid() &&
                    showcaseScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(
                        showcaseScene,
                        removeScene: true);
                }
            }

            AssetDatabase.ImportAsset(
                SceneAssetPath,
                ImportAssetOptions.ForceSynchronousImport);

            AssetImporter importer =
                AssetImporter.GetAtPath(SceneAssetPath);
            if (importer != null)
            {
                importer.userData = ImportSignature;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            Debug.Log(
                "Generated CozyFarmShowcase with the current hero and " +
                "curated Cozy Farm pilot sprites.");
        }

        private static bool TryLoadAssets(
            out ShowcaseAssets assets)
        {
            var sprites = new Dictionary<string, Sprite>(
                StringComparer.Ordinal);

            bool loaded =
                LoadNamedSprites(
                    SourceRoot + "items.png",
                    ItemNames,
                    sprites) &&
                LoadNamedSprites(
                    SourceRoot + "seeds.png",
                    SeedNames,
                    sprites) &&
                LoadNamedSprites(
                    SourceRoot + "tiles.png",
                    TerrainNames,
                    sprites);

            foreach (string cropName in CropNames)
            {
                string[] stages = Enumerable.Range(0, 6)
                    .Select(stage =>
                        $"cozy_{cropName}_stage_{stage}")
                    .ToArray();

                loaded &= LoadNamedSprites(
                    SourceRoot + "crops.png",
                    stages,
                    sprites);
            }

            GameObject playerPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    PlayerPrefabAssetCatalog.PrefabAssetPath);

            assets = loaded && playerPrefab != null
                ? new ShowcaseAssets(playerPrefab, sprites)
                : null;
            return assets != null;
        }

        private static bool LoadNamedSprites(
            string assetPath,
            IEnumerable<string> names,
            IDictionary<string, Sprite> destination)
        {
            Dictionary<string, Sprite> available =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
                    .OfType<Sprite>()
                    .ToDictionary(
                        sprite => sprite.name,
                        StringComparer.Ordinal);

            bool loaded = true;
            foreach (string name in names)
            {
                if (!available.TryGetValue(name, out Sprite sprite))
                {
                    loaded = false;
                    continue;
                }

                destination[name] = sprite;
            }

            return loaded;
        }

        private static void BuildShowcase(
            Scene scene,
            ShowcaseAssets assets)
        {
            CreateCamera(scene);

            var root = new GameObject(RootObjectName);
            SceneManager.MoveGameObjectToScene(root, scene);

            CreateTiledSprite(
                "Grass Backdrop",
                assets["cozy_grass"],
                root.transform,
                Vector2.zero,
                new Vector2(20f, 11f),
                TopDownSortingLayers.Ground,
                -100);

            CreateTerrainSamples(root.transform, assets);
            CreateCatalogIcons(root.transform, assets);
            CreateCropStages(root.transform, assets);
            CreateHero(scene, root.transform, assets);
        }

        private static void CreateCamera(Scene scene)
        {
            var cameraObject = new GameObject(CameraObjectName);
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position =
                new Vector3(0f, 0f, -10f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5.625f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor =
                new Color32(25, 34, 37, 255);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;

            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<ReferenceAspectCamera>();
        }

        private static void CreateTerrainSamples(
            Transform parent,
            ShowcaseAssets assets)
        {
            Transform group = CreateGroup(
                TerrainGroupName,
                parent);

            Vector2[] positions =
            {
                new Vector2(5.1f, 3.2f),
                new Vector2(8f, 3.2f),
                new Vector2(5.1f, 0.35f),
                new Vector2(8f, 0.35f)
            };

            for (int index = 0;
                 index < TerrainNames.Length;
                 index++)
            {
                string name = TerrainNames[index];
                CreateTiledSprite(
                    name,
                    assets[name],
                    group,
                    positions[index],
                    new Vector2(2.5f, 2.25f),
                    TopDownSortingLayers.World,
                    0);
            }
        }

        private static void CreateCatalogIcons(
            Transform parent,
            ShowcaseAssets assets)
        {
            Transform group = CreateGroup(
                CatalogGroupName,
                parent);
            Transform items = CreateGroup("Harvested Items", group);
            Transform seeds = CreateGroup("Seed Bags", group);

            for (int index = 0; index < ItemNames.Length; index++)
            {
                float x = -7.6f + (index * 1.7f);
                CreateSprite(
                    ItemNames[index],
                    assets[ItemNames[index]],
                    items,
                    new Vector2(x, 3.55f),
                    TopDownSortingLayers.Actors,
                    100);

                CreateSprite(
                    SeedNames[index],
                    assets[SeedNames[index]],
                    seeds,
                    new Vector2(x, 2.05f),
                    TopDownSortingLayers.Actors,
                    100);
            }
        }

        private static void CreateCropStages(
            Transform parent,
            ShowcaseAssets assets)
        {
            Transform group = CreateGroup(
                CropGroupName,
                parent);
            float[] rowY = { 0.45f, -1.25f, -2.95f };

            for (int cropIndex = 0;
                 cropIndex < CropNames.Length;
                 cropIndex++)
            {
                string cropName = CropNames[cropIndex];
                Transform row = CreateGroup(
                    $"{cropName} stages",
                    group);

                for (int stage = 0; stage < 6; stage++)
                {
                    float x = -8f + (stage * 1.35f);
                    string spriteName =
                        $"cozy_{cropName}_stage_{stage}";

                    CreateSprite(
                        $"soil_for_{spriteName}",
                        assets["cozy_tilled_soil"],
                        row,
                        new Vector2(x, rowY[cropIndex] - 0.5f),
                        TopDownSortingLayers.Ground,
                        10);

                    CreateSprite(
                        spriteName,
                        assets[spriteName],
                        row,
                        new Vector2(x, rowY[cropIndex]),
                        TopDownSortingLayers.Actors,
                        100);
                }
            }
        }

        private static void CreateHero(
            Scene scene,
            Transform parent,
            ShowcaseAssets assets)
        {
            CreateTiledSprite(
                "Hero Scale Reference",
                assets["cozy_dirt"],
                parent,
                new Vector2(3f, -3.15f),
                new Vector2(3f, 2.5f),
                TopDownSortingLayers.World,
                0);

            GameObject hero =
                PrefabUtility.InstantiatePrefab(
                    assets.PlayerPrefab,
                    scene) as GameObject;

            if (hero == null)
            {
                throw new InvalidOperationException(
                    "Unity could not instantiate the Player prefab.");
            }

            hero.name = HeroObjectName;
            hero.transform.SetParent(parent, worldPositionStays: true);
            hero.transform.position =
                new Vector3(3f, -3.65f, 0f);
        }

        private static Transform CreateGroup(
            string name,
            Transform parent)
        {
            var group = new GameObject(name);
            group.transform.SetParent(parent, worldPositionStays: false);
            return group.transform;
        }

        private static SpriteRenderer CreateSprite(
            string objectName,
            Sprite sprite,
            Transform parent,
            Vector2 position,
            string sortingLayer,
            int sortingOrder)
        {
            var spriteObject = new GameObject(objectName);
            spriteObject.transform.SetParent(
                parent,
                worldPositionStays: false);
            spriteObject.transform.localPosition =
                new Vector3(position.x, position.y, 0f);

            SpriteRenderer renderer =
                spriteObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static SpriteRenderer CreateTiledSprite(
            string objectName,
            Sprite sprite,
            Transform parent,
            Vector2 position,
            Vector2 size,
            string sortingLayer,
            int sortingOrder)
        {
            SpriteRenderer renderer = CreateSprite(
                objectName,
                sprite,
                parent,
                position,
                sortingLayer,
                sortingOrder);
            renderer.drawMode = SpriteDrawMode.Tiled;
            renderer.size = size;
            return renderer;
        }

        private sealed class ShowcaseAssets
        {
            private readonly IReadOnlyDictionary<string, Sprite>
                sprites;

            public ShowcaseAssets(
                GameObject playerPrefab,
                IReadOnlyDictionary<string, Sprite> sprites)
            {
                PlayerPrefab = playerPrefab ??
                    throw new ArgumentNullException(
                        nameof(playerPrefab));
                this.sprites = sprites ??
                    throw new ArgumentNullException(nameof(sprites));
            }

            public GameObject PlayerPrefab { get; }

            public Sprite this[string name] => sprites[name];
        }
    }
}
