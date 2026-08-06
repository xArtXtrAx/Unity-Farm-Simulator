using System;
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
    /// The facade is a complete transparent house extracted from the purchased
    /// Cozy Farm full-version buildings atlas.
    /// </summary>
    public static class CozyFarmHouseExteriorUpgrader
    {
        public const string VisualRootName = "Cozy Full-Pack House v4";
        public const float MaximumHouseWidth = 5.8f;
        public const float MaximumHouseHeight = 4.45f;
        public const float HouseBaseline = -1.62f;

        [MenuItem("Tools/Farm Simulator/Apply Cozy House Exterior To Farm Scene")]
        public static void ApplyFromMenu()
        {
            HouseAndSleepScenePipeline.EnsureScenes();
            Sprite houseSprite = CozyFarmBuildingCatalog.EnsureStarterHouse();

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

                RebuildVisuals(house, houseSprite);
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
            Debug.Log(
                "Applied a complete Cozy Farm full-pack house sprite to Farm.");
        }

        private static void RebuildVisuals(Transform house, Sprite houseSprite)
        {
            if (houseSprite == null)
            {
                throw new ArgumentNullException(nameof(houseSprite));
            }

            // The house root owns only generated visual children. Functional
            // portal, collider and spawn objects live elsewhere in Farm World.
            for (int index = house.childCount - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(
                    house.GetChild(index).gameObject);
            }

            var visualRoot = new GameObject(VisualRootName);
            visualRoot.transform.SetParent(house, false);
            visualRoot.transform.localPosition = Vector3.zero;

            var facade = new GameObject("Starter Green Gable House");
            facade.transform.SetParent(visualRoot.transform, false);

            Vector2 spriteSize = houseSprite.bounds.size;
            float scale = spriteSize.x <= Mathf.Epsilon ||
                spriteSize.y <= Mathf.Epsilon
                ? 1f
                : Mathf.Min(
                    MaximumHouseWidth / spriteSize.x,
                    MaximumHouseHeight / spriteSize.y);

            facade.transform.localScale = new Vector3(scale, scale, 1f);
            facade.transform.localPosition =
                new Vector3(0f, HouseBaseline, 0f);

            SpriteRenderer renderer = facade.AddComponent<SpriteRenderer>();
            renderer.sprite = houseSprite;
            renderer.color = Color.white;
            renderer.sortingLayerName = TopDownSortingLayers.World;
            renderer.sortingOrder = 20;

            // A subtle porch shadow anchors the complete atlas sprite to the
            // gameplay floor without modifying its original artwork.
            var shadow = new GameObject("Entrance Grounding Shadow");
            shadow.transform.SetParent(visualRoot.transform, false);
            shadow.transform.localPosition = new Vector3(0f, -1.67f, 0f);
            shadow.transform.localScale = new Vector3(1.25f, 0.28f, 1f);
            SpriteRenderer shadowRenderer = shadow.AddComponent<SpriteRenderer>();
            shadowRenderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(
                "UI/Skin/UISprite.psd");
            shadowRenderer.color = new Color(0.16f, 0.12f, 0.08f, 0.22f);
            shadowRenderer.sortingLayerName = TopDownSortingLayers.World;
            shadowRenderer.sortingOrder = 19;
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
