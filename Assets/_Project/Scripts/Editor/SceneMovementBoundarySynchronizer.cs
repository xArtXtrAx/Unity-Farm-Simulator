using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    internal static class SceneMovementBoundarySynchronizer
    {
        private const string FarmScenePath = "Assets/_Project/Scenes/Farm.unity";
        private const string InteriorScenePath = "Assets/_Project/Scenes/HouseInterior.unity";
        private const string BoundsObjectName = "Scene Authoring Bounds";
        private const string MovementBoundaryName = "Movement Boundary";

        private static readonly string[] WallNames =
        {
            "Boundary Left",
            "Boundary Right",
            "Boundary Bottom",
            "Boundary Top",
        };

        static SceneMovementBoundarySynchronizer()
        {
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneSaving += OnSceneSaving;
        }

        private static void OnSceneSaving(Scene scene, string path)
        {
            if (!string.Equals(path, FarmScenePath, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(path, InteriorScenePath, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Synchronize(scene);
        }

        internal static void Synchronize(Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return;
            }

            BoxCollider2D authoredBounds = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<BoxCollider2D>(true))
                .FirstOrDefault(collider => collider.name == BoundsObjectName);
            if (authoredBounds == null)
            {
                return;
            }

            Transform boundaryRoot = FindOrCreateBoundaryRoot(scene);
            RemoveLegacyBoundaryColliders(boundaryRoot);

            Bounds worldBounds = authoredBounds.bounds;
            float thickness = DetermineWallThickness(scene);
            int layer = boundaryRoot.gameObject.layer;

            ConfigureWall(
                boundaryRoot,
                WallNames[0],
                new Vector2(worldBounds.min.x - thickness * 0.5f, worldBounds.center.y),
                new Vector2(thickness, worldBounds.size.y + thickness * 2f),
                layer);
            ConfigureWall(
                boundaryRoot,
                WallNames[1],
                new Vector2(worldBounds.max.x + thickness * 0.5f, worldBounds.center.y),
                new Vector2(thickness, worldBounds.size.y + thickness * 2f),
                layer);
            ConfigureWall(
                boundaryRoot,
                WallNames[2],
                new Vector2(worldBounds.center.x, worldBounds.min.y - thickness * 0.5f),
                new Vector2(worldBounds.size.x, thickness),
                layer);
            ConfigureWall(
                boundaryRoot,
                WallNames[3],
                new Vector2(worldBounds.center.x, worldBounds.max.y + thickness * 0.5f),
                new Vector2(worldBounds.size.x, thickness),
                layer);

            EditorUtility.SetDirty(boundaryRoot.gameObject);
        }

        private static Transform FindOrCreateBoundaryRoot(Scene scene)
        {
            Transform existing = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .FirstOrDefault(transform =>
                    string.Equals(transform.name, MovementBoundaryName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                return existing;
            }

            var gameObject = new GameObject(MovementBoundaryName);
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            return gameObject.transform;
        }

        private static void RemoveLegacyBoundaryColliders(Transform boundaryRoot)
        {
            Collider2D[] colliders = boundaryRoot.GetComponentsInChildren<Collider2D>(true);
            foreach (Collider2D collider in colliders)
            {
                if (WallNames.Contains(collider.gameObject.name) && collider is BoxCollider2D)
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(collider);
            }
        }

        private static void ConfigureWall(
            Transform boundaryRoot,
            string wallName,
            Vector2 worldPosition,
            Vector2 size,
            int layer)
        {
            Transform wall = boundaryRoot.Find(wallName);
            if (wall == null)
            {
                var gameObject = new GameObject(wallName, typeof(BoxCollider2D));
                gameObject.transform.SetParent(boundaryRoot, false);
                wall = gameObject.transform;
            }

            wall.gameObject.layer = layer;
            wall.position = new Vector3(worldPosition.x, worldPosition.y, boundaryRoot.position.z);
            wall.rotation = Quaternion.identity;
            wall.localScale = Vector3.one;

            BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = wall.gameObject.AddComponent<BoxCollider2D>();
            }

            collider.offset = Vector2.zero;
            collider.size = new Vector2(Mathf.Max(0.01f, size.x), Mathf.Max(0.01f, size.y));
            collider.isTrigger = false;
            collider.enabled = true;
            EditorUtility.SetDirty(collider);
        }

        private static float DetermineWallThickness(Scene scene)
        {
            Grid grid = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Grid>(true))
                .FirstOrDefault();
            if (grid == null)
            {
                return 1f;
            }

            return Mathf.Max(0.1f, Mathf.Max(Mathf.Abs(grid.cellSize.x), Mathf.Abs(grid.cellSize.y)));
        }
    }
}
