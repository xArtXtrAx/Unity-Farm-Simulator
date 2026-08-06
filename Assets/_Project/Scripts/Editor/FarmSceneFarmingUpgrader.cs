using System;
using System.Collections.Generic;
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
        public const string UpgradeRootName = "Farming Core Loop v3";
        public const string FieldRootName = "Farm Plot Field";
        public const string GridRootName = "Farm Authoring Grid";
        public const int Columns = 3;
        public const int Rows = 3;

        private const string LegacyUpgradeRootName = "Farming Core Loop v1";
        private const string PreviousUpgradeRootName = "Farming Core Loop v2";
        private const string TileSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png";
        private const string CropSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/crops.png";

        static FarmSceneFarmingUpgrader()
        {
            EditorApplication.delayCall += EnsureApplied;
        }

        [MenuItem("Tools/Farm Simulator/Apply Farming Field To Farm Scene")]
        public static void ApplyFromMenu()
        {
            Apply(force: true);
        }

        public static void EnsureApplied()
        {
            Apply(force: false);
        }

        private static void Apply(bool force)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureApplied;
                return;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.FarmPath) == null)
            {
                EditorApplication.delayCall += EnsureApplied;
                return;
            }

            SpriteLibrary sprites = LoadSprites();
            if (!sprites.IsComplete)
            {
                Debug.LogWarning(
                    "Farming field is waiting for the curated Cozy Farm tile and crop sprites.");
                return;
            }

            Scene scene = SceneManager.GetSceneByPath(ProjectSceneNames.FarmPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(
                    ProjectSceneNames.FarmPath,
                    OpenSceneMode.Additive);
            }

            try
            {
                Transform existing = Find(scene, UpgradeRootName);
                if (existing != null && !force)
                {
                    return;
                }

                DestroyIfPresent(scene, UpgradeRootName);
                DestroyIfPresent(scene, PreviousUpgradeRootName);
                DestroyIfPresent(scene, LegacyUpgradeRootName);
                BuildField(scene, sprites);

                if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                {
                    throw new InvalidOperationException(
                        "Could not save Farm after adding the field.");
                }
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            AssetImporter importer = AssetImporter.GetAtPath(ProjectSceneNames.FarmPath);
            if (importer != null)
            {
                importer.userData = HouseAndSleepScenePipeline.FarmImportSignature;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Applied Farming Core Loop v3: separate Soil and Crops Tilemaps, transparent crop assets and a persistent 3x3 plot field.");
        }

        private static void BuildField(Scene scene, SpriteLibrary sprites)
        {
            GameObject parent = FindRoot(scene, "Farm World");
            if (parent == null)
            {
                throw new InvalidOperationException("Farm scene is missing 'Farm World'.");
            }

            DestroyIfPresent(scene, "Farm Grass");
            DestroyIfPresent(scene, "House Path");

            var upgradeRoot = new GameObject(UpgradeRootName);
            upgradeRoot.transform.SetParent(parent.transform, false);

            FarmTilemapLayers layers = CreateTilemapLayers(
                upgradeRoot.transform,
                sprites);

            var fieldRoot = new GameObject(FieldRootName);
            fieldRoot.transform.SetParent(upgradeRoot.transform, false);

            Vector3Int firstCell = new Vector3Int(-5, -2, 0);
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Vector3Int cell = firstCell + new Vector3Int(column, row, 0);
                    string plotId = $"farm-plot-{column}-{row}";
                    var plot = new GameObject($"Plot {column + 1}-{row + 1}");
                    plot.transform.SetParent(fieldRoot.transform, false);
                    plot.transform.position = layers.Ground.GetCellCenterWorld(cell);

                    SpriteRenderer soil = CreateRenderer(
                        "Soil",
                        plot.transform,
                        sprites.TilledSoil,
                        TopDownSortingLayers.Ground,
                        -72);
                    soil.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                    soil.enabled = false;

                    SpriteRenderer crop = CreateRenderer(
                        "Crop",
                        plot.transform,
                        null,
                        TopDownSortingLayers.World,
                        18);
                    crop.transform.localPosition = new Vector3(
                        0f,
                        FarmPlotBehaviour.CropBaseline,
                        0f);
                    crop.transform.localScale = Vector3.one;
                    crop.enabled = false;

                    FarmPlotBehaviour behaviour = plot.AddComponent<FarmPlotBehaviour>();
                    behaviour.Configure(
                        plotId,
                        soil,
                        crop,
                        sprites.Grass,
                        sprites.TilledSoil,
                        sprites.TurnipStages,
                        sprites.CarrotStages,
                        sprites.CabbageStages);
                }
            }

            MoveIfPresent(scene, "Garden Lamp", new Vector3(-5.8f, -0.5f, 0f));
            MoveIfPresent(scene, "Rock Border", new Vector3(-5.2f, -3.35f, 0f));
        }

        private static FarmTilemapLayers CreateTilemapLayers(
            Transform parent,
            SpriteLibrary sprites)
        {
            var gridObject = new GameObject(
                GridRootName,
                typeof(Grid),
                typeof(FarmTilemapLayers));
            gridObject.transform.SetParent(parent, false);

            Grid grid = gridObject.GetComponent<Grid>();
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSize = Vector3.one;
            grid.cellGap = Vector3.zero;

            Tilemap ground = CreateTilemap(
                "Ground",
                gridObject.transform,
                TopDownSortingLayers.Ground,
                -110);
            Tilemap paths = CreateTilemap(
                "Paths",
                gridObject.transform,
                TopDownSortingLayers.Ground,
                -100);
            Tilemap soil = CreateTilemap(
                "Soil",
                gridObject.transform,
                TopDownSortingLayers.Ground,
                -80);
            Tilemap crops = CreateTilemap(
                "Crops",
                gridObject.transform,
                TopDownSortingLayers.World,
                -20);
            Tilemap decoration = CreateTilemap(
                "Decoration",
                gridObject.transform,
                TopDownSortingLayers.World,
                0);

            for (int y = -4; y <= 4; y++)
            {
                for (int x = -7; x <= 7; x++)
                {
                    ground.SetTile(new Vector3Int(x, y, 0), sprites.GrassTile);
                }
            }

            for (int y = -4; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    paths.SetTile(new Vector3Int(x, y, 0), sprites.DirtTile);
                }
            }

            FarmTilemapLayers registry = gridObject.GetComponent<FarmTilemapLayers>();
            registry.Configure(ground, paths, soil, crops, decoration);
            return registry;
        }

        private static Tilemap CreateTilemap(
            string name,
            Transform parent,
            string sortingLayer,
            int sortingOrder)
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
            string name,
            Transform parent,
            Sprite sprite,
            string sortingLayer,
            int sortingOrder)
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
            CozyFarmTileCatalog.EnsureAssets();

            Dictionary<string, Sprite> tiles = LoadRepresentations(TileSheetPath);
            Dictionary<string, Sprite> crops = LoadRepresentations(CropSheetPath);

            return new SpriteLibrary(
                Get(tiles, "cozy_grass"),
                Get(tiles, "cozy_tilled_soil"),
                Stages(crops, "cozy_turnip_stage_"),
                Stages(crops, "cozy_carrot_stage_"),
                Stages(crops, "cozy_cabbage_stage_"),
                AssetDatabase.LoadAssetAtPath<TileBase>(CozyFarmTileCatalog.GrassTilePath),
                AssetDatabase.LoadAssetAtPath<TileBase>(CozyFarmTileCatalog.DirtTilePath));
        }

        private static Dictionary<string, Sprite> LoadRepresentations(string path)
        {
            return AssetDatabase.LoadAllAssetRepresentationsAtPath(path)
                .OfType<Sprite>()
                .ToDictionary(sprite => sprite.name, StringComparer.Ordinal);
        }

        private static Sprite Get(
            IReadOnlyDictionary<string, Sprite> sprites,
            string name)
        {
            sprites.TryGetValue(name, out Sprite sprite);
            return sprite;
        }

        private static Sprite[] Stages(
            IReadOnlyDictionary<string, Sprite> sprites,
            string prefix)
        {
            var result = new Sprite[6];
            for (int index = 0; index < result.Length; index++)
            {
                result[index] = Get(sprites, prefix + index);
            }

            return result;
        }

        private static GameObject FindRoot(Scene scene, string name)
        {
            return scene.GetRootGameObjects().FirstOrDefault(root => root.name == name);
        }

        private static Transform Find(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform result = root.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(candidate => candidate.name == name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static void DestroyIfPresent(Scene scene, string name)
        {
            Transform target = Find(scene, name);
            if (target != null)
            {
                UnityEngine.Object.DestroyImmediate(target.gameObject);
            }
        }

        private static void MoveIfPresent(
            Scene scene,
            string name,
            Vector3 localPosition)
        {
            Transform target = Find(scene, name);
            if (target != null)
            {
                target.localPosition = localPosition;
            }
        }

        private sealed class SpriteLibrary
        {
            public SpriteLibrary(
                Sprite grass,
                Sprite tilledSoil,
                Sprite[] turnipStages,
                Sprite[] carrotStages,
                Sprite[] cabbageStages,
                TileBase grassTile,
                TileBase dirtTile)
            {
                Grass = grass;
                TilledSoil = tilledSoil;
                TurnipStages = turnipStages;
                CarrotStages = carrotStages;
                CabbageStages = cabbageStages;
                GrassTile = grassTile;
                DirtTile = dirtTile;
            }

            public Sprite Grass { get; }
            public Sprite TilledSoil { get; }
            public Sprite[] TurnipStages { get; }
            public Sprite[] CarrotStages { get; }
            public Sprite[] CabbageStages { get; }
            public TileBase GrassTile { get; }
            public TileBase DirtTile { get; }

            public bool IsComplete =>
                Grass != null &&
                TilledSoil != null &&
                Complete(TurnipStages) &&
                Complete(CarrotStages) &&
                Complete(CabbageStages) &&
                GrassTile != null &&
                DirtTile != null;

            private static bool Complete(Sprite[] sprites)
            {
                return sprites != null &&
                    sprites.Length == 6 &&
                    sprites.All(sprite => sprite != null);
            }
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
            if (importedAssets.Any(path => string.Equals(
                    path,
                    ProjectSceneNames.FarmPath,
                    StringComparison.Ordinal)))
            {
                EditorApplication.delayCall += FarmSceneFarmingUpgrader.EnsureApplied;
            }
        }
    }
}
