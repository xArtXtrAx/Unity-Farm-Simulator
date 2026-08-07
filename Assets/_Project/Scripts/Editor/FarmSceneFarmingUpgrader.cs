using System;
using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Farming;
using FarmSimulator.Presentation.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class FarmSceneFarmingUpgrader
    {
        public const string UpgradeRootName = "Farming Core Loop v4";
        public const string FieldRootName = "Farm Plot Field";
        public const string GridRootName = "Farm Authoring Grid";
        public const int Columns = 3;
        public const int Rows = 3;

        private static readonly string[] PreviousRoots =
        {
            "Farming Core Loop v1",
            "Farming Core Loop v2",
            "Farming Core Loop v3",
        };

        private const string PlaceholderSourceRoot =
            "Assets/_Project/Art/Placeholder/Source";
        private const string PlaceholderTileRoot =
            "Assets/_Project/Art/Placeholder/Tiles";
        private const string CropFolder =
            "Assets/_Project/Art/Placeholder/Crops";

        private const string GrassSpritePath =
            PlaceholderSourceRoot + "/ground_grass.png";
        private const string TilledSoilSpritePath =
            PlaceholderSourceRoot + "/soil_tilled.png";
        private const string GrassTilePath =
            PlaceholderTileRoot + "/ground_grass.asset";
        private const string DirtTilePath =
            PlaceholderTileRoot + "/path_dirt.asset";

        private static bool playModeTransitionBlocked;

        static FarmSceneFarmingUpgrader()
        {
            QueueEnsureApplied();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("Tools/Farm Simulator/Apply Farming Field To Farm Scene")]
        public static void ApplyFromMenu() => Apply(force: true);

        public static void EnsureApplied()
        {
            if (ShouldAvoidSceneAuthoring())
                return;

            Apply(force: false);
        }

        /// <summary>
        /// Explicit recovery hook. Scene Recovery may rebuild Farm from an empty
        /// scene, so this reapplies the complete first-party farming field after
        /// the recovered scene has imported.
        /// </summary>
        public static void ApplyAfterSceneRecovery()
        {
            if (ShouldAvoidSceneAuthoring())
                return;

            Apply(force: true);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    playModeTransitionBlocked = true;
                    EditorApplication.delayCall -= EnsureApplied;
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    playModeTransitionBlocked = true;
                    EditorApplication.delayCall -= EnsureApplied;
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    playModeTransitionBlocked = false;
                    QueueEnsureApplied();
                    break;
            }
        }

        private static bool ShouldAvoidSceneAuthoring() =>
            playModeTransitionBlocked ||
            EditorApplication.isPlaying ||
            EditorApplication.isPlayingOrWillChangePlaymode;

        private static void QueueEnsureApplied()
        {
            if (ShouldAvoidSceneAuthoring())
                return;

            EditorApplication.delayCall -= EnsureApplied;
            EditorApplication.delayCall += EnsureApplied;
        }

        private static void Apply(bool force)
        {
            if (ShouldAvoidSceneAuthoring())
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                QueueEnsureApplied();
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.FarmPath) == null)
            {
                QueueEnsureApplied();
                return;
            }

            SpriteLibrary sprites = LoadSprites();
            if (!sprites.IsComplete)
            {
                Debug.LogWarning(
                    "Farming field is waiting for the first-party placeholder terrain and final crop sprites.");
                return;
            }

            // Re-check immediately before using EditorSceneManager. A play-mode
            // transition can begin after a delayed callback has already started.
            if (ShouldAvoidSceneAuthoring())
                return;

            Scene scene = SceneManager.GetSceneByPath(ProjectSceneNames.FarmPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                if (ShouldAvoidSceneAuthoring())
                    return;

                scene = EditorSceneManager.OpenScene(
                    ProjectSceneNames.FarmPath,
                    OpenSceneMode.Additive);
            }

            try
            {
                if (Find(scene, UpgradeRootName) != null && !force)
                    return;

                DestroyIfPresent(scene, UpgradeRootName);
                foreach (string previousRoot in PreviousRoots)
                    DestroyIfPresent(scene, previousRoot);

                BuildField(scene, sprites);
                if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                    throw new InvalidOperationException("Could not save Farm after adding the field.");
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                    EditorSceneManager.CloseScene(scene, true);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Applied Farming Core Loop v4 using first-party terrain Tilemaps plus plot-owned crop SpriteRenderers.");
        }

        private static void BuildField(Scene scene, SpriteLibrary sprites)
        {
            GameObject parent = FindRoot(scene, "Farm World");
            if (parent == null)
                throw new InvalidOperationException("Farm scene is missing 'Farm World'.");

            DestroyIfPresent(scene, "Farm Grass");
            DestroyIfPresent(scene, "House Path");

            var upgradeRoot = new GameObject(UpgradeRootName);
            upgradeRoot.transform.SetParent(parent.transform, false);
            FarmTilemapLayers layers = CreateTilemapLayers(upgradeRoot.transform, sprites);

            var fieldRoot = new GameObject(FieldRootName);
            fieldRoot.transform.SetParent(upgradeRoot.transform, false);

            Vector3Int firstCell = new Vector3Int(-5, -2, 0);
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Vector3Int cell = firstCell + new Vector3Int(column, row, 0);
                    var plot = new GameObject($"Plot {column + 1}-{row + 1}");
                    plot.transform.SetParent(fieldRoot.transform, false);
                    plot.transform.position = layers.Ground.GetCellCenterWorld(cell);

                    SpriteRenderer soil = CreateRenderer(
                        "Soil Visual", plot.transform, sprites.TilledSoil,
                        TopDownSortingLayers.Ground, -72);
                    soil.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                    soil.enabled = false;

                    SpriteRenderer crop = CreateRenderer(
                        "Crop Entity Visual", plot.transform, null,
                        TopDownSortingLayers.World, 18);
                    crop.enabled = false;

                    FarmPlotBehaviour behaviour = plot.AddComponent<FarmPlotBehaviour>();
                    behaviour.Configure(
                        $"farm-plot-{column}-{row}",
                        soil,
                        crop,
                        sprites.Grass,
                        sprites.TilledSoil,
                        sprites.TurnipStages,
                        sprites.PotatoStages,
                        sprites.RadishStages);
                }
            }

            MoveIfPresent(scene, "Garden Lamp", new Vector3(-5.8f, -0.5f, 0f));
            MoveIfPresent(scene, "Rock Border", new Vector3(-5.2f, -3.35f, 0f));
        }

        private static FarmTilemapLayers CreateTilemapLayers(Transform parent, SpriteLibrary sprites)
        {
            var gridObject = new GameObject(GridRootName, typeof(Grid), typeof(FarmTilemapLayers));
            gridObject.transform.SetParent(parent, false);
            Grid grid = gridObject.GetComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSize = Vector3.one;
            grid.cellGap = Vector3.zero;

            Tilemap ground = CreateTilemap("Ground", gridObject.transform, TopDownSortingLayers.Ground, -110);
            Tilemap paths = CreateTilemap("Paths", gridObject.transform, TopDownSortingLayers.Ground, -100);
            Tilemap soil = CreateTilemap("Soil", gridObject.transform, TopDownSortingLayers.Ground, -80);
            Tilemap decoration = CreateTilemap("Decoration", gridObject.transform, TopDownSortingLayers.World, 0);

            for (int y = -4; y <= 4; y++)
                for (int x = -7; x <= 7; x++)
                    ground.SetTile(new Vector3Int(x, y, 0), sprites.GrassTile);

            for (int y = -4; y <= 1; y++)
                for (int x = -1; x <= 1; x++)
                    paths.SetTile(new Vector3Int(x, y, 0), sprites.DirtTile);

            FarmTilemapLayers registry = gridObject.GetComponent<FarmTilemapLayers>();
            registry.Configure(ground, paths, soil, decoration);
            return registry;
        }

        private static Tilemap CreateTilemap(string name, Transform parent, string sortingLayer, int sortingOrder)
        {
            var go = new GameObject(name, typeof(Tilemap), typeof(TilemapRenderer));
            go.transform.SetParent(parent, false);
            TilemapRenderer renderer = go.GetComponent<TilemapRenderer>();
            renderer.mode = TilemapRenderer.Mode.Chunk;
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = sortingOrder;
            return go.GetComponent<Tilemap>();
        }

        private static SpriteRenderer CreateRenderer(
            string name, Transform parent, Sprite sprite, string sortingLayer, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static SpriteLibrary LoadSprites()
        {
            return new SpriteLibrary(
                AssetDatabase.LoadAssetAtPath<Sprite>(GrassSpritePath),
                AssetDatabase.LoadAssetAtPath<Sprite>(TilledSoilSpritePath),
                LoadCropStages("turnip", 5),
                LoadCropStages("potato", 6),
                LoadCropStages("radish", 5),
                AssetDatabase.LoadAssetAtPath<TileBase>(GrassTilePath),
                AssetDatabase.LoadAssetAtPath<TileBase>(DirtTilePath));
        }

        private static Sprite[] LoadCropStages(string cropName, int stageCount)
        {
            var sprites = new Sprite[stageCount];
            for (int index = 0; index < stageCount; index++)
            {
                sprites[index] = AssetDatabase.LoadAssetAtPath<Sprite>(
                    $"{CropFolder}/{cropName}_stage_{index}.png");
            }
            return sprites;
        }

        private static GameObject FindRoot(Scene scene, string name) =>
            scene.GetRootGameObjects().FirstOrDefault(root => root.name == name);

        private static Transform Find(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform result = root.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(candidate => candidate.name == name);
                if (result != null) return result;
            }
            return null;
        }

        private static void DestroyIfPresent(Scene scene, string name)
        {
            Transform target = Find(scene, name);
            if (target != null) UnityEngine.Object.DestroyImmediate(target.gameObject);
        }

        private static void MoveIfPresent(Scene scene, string name, Vector3 localPosition)
        {
            Transform target = Find(scene, name);
            if (target != null) target.localPosition = localPosition;
        }

        private sealed class SpriteLibrary
        {
            public SpriteLibrary(
                Sprite grass,
                Sprite tilledSoil,
                Sprite[] turnipStages,
                Sprite[] potatoStages,
                Sprite[] radishStages,
                TileBase grassTile,
                TileBase dirtTile)
            {
                Grass = grass;
                TilledSoil = tilledSoil;
                TurnipStages = turnipStages;
                PotatoStages = potatoStages;
                RadishStages = radishStages;
                GrassTile = grassTile;
                DirtTile = dirtTile;
            }

            public Sprite Grass { get; }
            public Sprite TilledSoil { get; }
            public Sprite[] TurnipStages { get; }
            public Sprite[] PotatoStages { get; }
            public Sprite[] RadishStages { get; }
            public TileBase GrassTile { get; }
            public TileBase DirtTile { get; }

            public bool IsComplete =>
                Grass != null &&
                TilledSoil != null &&
                Complete(TurnipStages, 5) &&
                Complete(PotatoStages, 6) &&
                Complete(RadishStages, 5) &&
                GrassTile != null &&
                DirtTile != null;

            private static bool Complete(Sprite[] sprites, int expectedCount) =>
                sprites != null &&
                sprites.Length == expectedCount &&
                sprites.All(sprite => sprite != null);
        }
    }

    internal sealed class FarmSceneFarmingPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (EditorApplication.isPlaying ||
                EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (importedAssets.Any(path => string.Equals(
                    path, ProjectSceneNames.FarmPath, StringComparison.Ordinal)))
            {
                EditorApplication.delayCall -= FarmSceneFarmingUpgrader.EnsureApplied;
                EditorApplication.delayCall += FarmSceneFarmingUpgrader.EnsureApplied;
            }
        }
    }
}
