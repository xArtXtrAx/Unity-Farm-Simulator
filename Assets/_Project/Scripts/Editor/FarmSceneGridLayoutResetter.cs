using System;
using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Farming;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class FarmSceneGridLayoutResetter
    {
        public const string LayoutRootName = "Farm Grid Layout v1";

        private static readonly string[] RemovedDecorNames =
        {
            "Left Orchard Tree",
            "Right Orchard Tree",
            "Back Garden Bushes",
            "Rock Border",
            "Fence Border",
        };

        static FarmSceneGridLayoutResetter()
        {
            EditorApplication.delayCall += EnsureApplied;
        }

        [MenuItem("Tools/Farm Simulator/Reset Farm To Grid Starter Layout")]
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

            if (!force && IsAlreadyApplied())
            {
                return;
            }

            HouseAndSleepScenePipeline.EnsureScenes();
            FarmSceneFarmingUpgrader.ApplyFromMenu();

            string variantId = CozyFarmHouseStyleWindow.SelectedVariantId;
            try
            {
                CozyFarmBuildingCatalog.GetHouse(variantId);
            }
            catch (ArgumentException)
            {
                variantId = CozyFarmBuildingCatalog.DefaultHouseId;
            }

            CozyFarmHouseExteriorUpgrader.ApplyVariant(variantId);

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
                Transform world = Find(scene, "Farm World");
                FarmTilemapLayers layers = FindComponent<FarmTilemapLayers>(scene);
                Transform house = Find(scene, "Hero House Exterior");
                Transform field = Find(scene, FarmSceneFarmingUpgrader.FieldRootName);
                Transform bench = Find(scene, "Garden Bench");
                Transform lamp = Find(scene, "Garden Lamp");

                if (world == null || layers == null || house == null ||
                    field == null || bench == null || lamp == null)
                {
                    throw new InvalidOperationException(
                        "Farm grid reset requires Farm World, tilemap layers, house, field, bench and lamp.");
                }

                DestroyIfPresent(scene, LayoutRootName);
                foreach (string objectName in RemovedDecorNames)
                {
                    DestroyIfPresent(scene, objectName);
                }

                layers.Paths.ClearAllTiles();
                layers.Soil.ClearAllTiles();
                layers.Decoration.ClearAllTiles();

                Grid grid = layers.GetComponent<Grid>();
                SnapToCellCenter(house, grid, new Vector3Int(0, 2, 0));
                SnapToCellCenter(lamp, grid, new Vector3Int(-4, 2, 0));
                SnapToCellCenter(bench, grid, new Vector3Int(4, 2, 0));

                RepositionPlots(field, grid);
                AlignHouseFlow(scene, house, variantId);
                SnapToCellCenter(
                    Find(scene, "Spawn farm-start"),
                    grid,
                    new Vector3Int(0, -3, 0));

                Transform marker = new GameObject(LayoutRootName).transform;
                marker.SetParent(world, false);

                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                {
                    throw new InvalidOperationException(
                        "Could not save the grid-aligned Farm scene.");
                }
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Reset Farm to grid starter layout: house, nine plots, right bench and left lamp.");
        }

        private static bool IsAlreadyApplied()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectSceneNames.FarmPath) == null)
            {
                return false;
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
                return Find(scene, LayoutRootName) != null;
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static void RepositionPlots(Transform field, Grid grid)
        {
            FarmPlotBehaviour[] plots = field
                .GetComponentsInChildren<FarmPlotBehaviour>(true)
                .OrderBy(plot => plot.PlotId, StringComparer.Ordinal)
                .ToArray();

            if (plots.Length != FarmSceneFarmingUpgrader.Columns * FarmSceneFarmingUpgrader.Rows)
            {
                throw new InvalidOperationException("Farm must contain exactly nine plots.");
            }

            int index = 0;
            for (int row = 0; row < FarmSceneFarmingUpgrader.Rows; row++)
            {
                for (int column = 0; column < FarmSceneFarmingUpgrader.Columns; column++)
                {
                    Vector3Int cell = new Vector3Int(-4 + column, -1 + row, 0);
                    plots[index++].transform.position = grid.GetCellCenterWorld(cell);
                }
            }
        }

        private static void AlignHouseFlow(
            Scene scene,
            Transform house,
            string variantId)
        {
            CozyFarmBuildingCatalog.HouseVariant variant =
                CozyFarmBuildingCatalog.GetHouse(variantId);

            Transform portal = Find(scene, "House Entrance Portal");
            if (portal != null)
            {
                portal.position = house.TransformPoint(variant.PortalOffset);
            }

            Transform doorSpawn = Find(scene, "Spawn farm-house-door");
            if (doorSpawn != null)
            {
                doorSpawn.position = house.TransformPoint(variant.SpawnOffset);
            }
        }

        private static void SnapToCellCenter(
            Transform target,
            Grid grid,
            Vector3Int cell)
        {
            if (target != null)
            {
                target.position = grid.GetCellCenterWorld(cell);
            }
        }

        private static T FindComponent<T>(Scene scene)
            where T : Component
        {
            return scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<T>(true))
                .FirstOrDefault();
        }

        private static Transform Find(Scene scene, string objectName)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform match = root.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(candidate => candidate.name == objectName);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }

        private static void DestroyIfPresent(Scene scene, string objectName)
        {
            Transform target = Find(scene, objectName);
            if (target != null)
            {
                UnityEngine.Object.DestroyImmediate(target.gameObject);
            }
        }
    }
}
