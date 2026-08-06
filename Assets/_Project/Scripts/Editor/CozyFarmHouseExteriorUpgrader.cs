using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Presentation.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Rebuilds only the visual children of the farm house. The functional
    /// house root, collider, spawn point and entrance portal remain untouched.
    /// </summary>
    public static class CozyFarmHouseExteriorUpgrader
    {
        public const string VisualRootName = "Cozy House Facade v3";

        private const string TileSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png";

        [MenuItem("Tools/Farm Simulator/Apply Cozy House Exterior To Farm Scene")]
        public static void ApplyFromMenu()
        {
            CozyFarmHouseArtPipeline.EnsureAssets();
            HouseAndSleepScenePipeline.EnsureScenes();

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
                Transform house = Find(scene, "Hero House Exterior");
                if (house == null)
                {
                    throw new InvalidOperationException(
                        "Farm is missing 'Hero House Exterior'. Rebuild the house scenes first.");
                }

                Dictionary<string, Sprite> sprites =
                    AssetDatabase.LoadAllAssetRepresentationsAtPath(TileSheetPath)
                        .OfType<Sprite>()
                        .ToDictionary(sprite => sprite.name, StringComparer.Ordinal);

                RebuildVisuals(house, sprites);
                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene, ProjectSceneNames.FarmPath))
                {
                    throw new InvalidOperationException(
                        "Could not save Farm after rebuilding the Cozy house exterior.");
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
            Debug.Log("Applied Cozy House Facade v3 to Farm.");
        }

        private static void RebuildVisuals(
            Transform house,
            IReadOnlyDictionary<string, Sprite> sprites)
        {
            for (int index = house.childCount - 1; index >= 0; index--)
            {
                Transform child = house.GetChild(index);
                UnityEngine.Object.DestroyImmediate(child.gameObject);
            }

            Transform visualRoot = Group(VisualRootName, house);
            visualRoot.localPosition = Vector3.zero;

            // Foundation and wall mass.
            Patch(
                "Stone-free Foundation",
                visualRoot,
                Required(sprites, "cozy_wood_panel_dark"),
                new Vector2(0f, -1.04f),
                5,
                1,
                new Vector2(0.94f, 0.48f),
                8);
            Patch(
                "Front Timber Wall",
                visualRoot,
                Required(sprites, "cozy_wood_panel_light"),
                new Vector2(0f, -0.05f),
                5,
                2,
                new Vector2(0.94f, 0.88f),
                10);

            // Dark roof body with a wider silhouette and a light fascia.
            Patch(
                "Roof Body",
                visualRoot,
                Required(sprites, "cozy_wood_panel_dark"),
                new Vector2(0f, 1.28f),
                6,
                2,
                new Vector2(0.92f, 0.66f),
                20);
            Patch(
                "Roof Fascia",
                visualRoot,
                Required(sprites, "cozy_bench_dark"),
                new Vector2(0f, 0.73f),
                3,
                1,
                new Vector2(1.02f, 0.58f),
                24);

            // Central entrance and porch.
            SpriteObject(
                "Front Door",
                visualRoot,
                Required(sprites, "cozy_wood_panel_dark"),
                new Vector2(0f, -0.67f),
                new Vector2(0.46f, 0.82f),
                31);
            SpriteObject(
                "Door Header",
                visualRoot,
                Required(sprites, "cozy_bench_light"),
                new Vector2(0f, -0.08f),
                new Vector2(0.48f, 0.34f),
                32);
            SpriteObject(
                "Front Porch",
                visualRoot,
                Required(sprites, "cozy_bridge_wood"),
                new Vector2(0f, -1.57f),
                new Vector2(0.47f, 0.62f),
                26);

            // Symmetric window boxes make the facade readable at gameplay scale.
            SpriteObject(
                "Left Flower Window",
                visualRoot,
                Required(sprites, "cozy_flower_crates"),
                new Vector2(-1.55f, -0.48f),
                new Vector2(0.82f, 0.82f),
                33);
            SpriteObject(
                "Right Flower Window",
                visualRoot,
                Required(sprites, "cozy_flower_crates"),
                new Vector2(1.55f, -0.48f),
                new Vector2(0.82f, 0.82f),
                33);

            // Small structural accents break up the repeated panels.
            SpriteObject(
                "Left Corner Post",
                visualRoot,
                Required(sprites, "cozy_fence_horizontal"),
                new Vector2(-2.48f, -0.42f),
                new Vector2(0.32f, 1.32f),
                34,
                90f);
            SpriteObject(
                "Right Corner Post",
                visualRoot,
                Required(sprites, "cozy_fence_horizontal"),
                new Vector2(2.48f, -0.42f),
                new Vector2(0.32f, 1.32f),
                34,
                90f);
        }

        private static void Patch(
            string name,
            Transform parent,
            Sprite sprite,
            Vector2 center,
            int columns,
            int rows,
            Vector2 spacing,
            int sortingOrder)
        {
            Transform root = Group(name, parent);
            float startX = center.x - ((columns - 1) * spacing.x * 0.5f);
            float startY = center.y - ((rows - 1) * spacing.y * 0.5f);
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    SpriteObject(
                        $"{name} {x + 1}-{y + 1}",
                        root,
                        sprite,
                        new Vector2(startX + x * spacing.x, startY + y * spacing.y),
                        Vector2.one,
                        sortingOrder);
                }
            }
        }

        private static SpriteRenderer SpriteObject(
            string name,
            Transform parent,
            Sprite sprite,
            Vector2 localPosition,
            Vector2 scale,
            int sortingOrder,
            float rotation = 0f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            go.transform.localScale = new Vector3(scale.x, scale.y, 1f);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);

            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingLayerName = TopDownSortingLayers.World;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static Transform Group(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static Sprite Required(
            IReadOnlyDictionary<string, Sprite> sprites,
            string name)
        {
            if (sprites.TryGetValue(name, out Sprite sprite) && sprite != null)
            {
                return sprite;
            }

            throw new InvalidOperationException(
                $"The curated Cozy house catalog is missing '{name}'.");
        }

        private static Transform Find(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform match = root.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(candidate => candidate.name == name);
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }
    }
}
