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
    /// Rebuilds only the generated visual children of the farm house. Functional
    /// scene objects remain untouched unless a variant-specific metadata target
    /// can be found safely by name.
    /// </summary>
    public static class CozyFarmHouseExteriorUpgrader
    {
        public const string VisualRootName = "Cozy Full-Pack House v5";

        [MenuItem("Tools/Farm Simulator/Apply Cozy House Exterior To Farm Scene")]
        public static void ApplyFromMenu()
        {
            ApplyVariant(CoziestSelectedVariantId());
        }

        public static void ApplyVariant(string variantId)
        {
            HouseAndSleepScenePipeline.EnsureScenes();
            CozyFarmBuildingCatalog.HouseVariant variant =
                CozyFarmBuildingCatalog.GetHouse(variantId);
            Sprite houseSprite = CozyFarmBuildingCatalog.EnsureHouse(variant.Id);

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

                RebuildVisuals(house, houseSprite, variant);
                ApplyOptionalFunctionalMetadata(scene, house, variant);
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

            CozyFarmHouseStyleWindow.SelectedVariantId = variant.Id;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Applied Cozy Farm house variant '{variant.DisplayName}' to Farm.");
        }

        private static string CoziestSelectedVariantId()
        {
            string id = CozyFarmHouseStyleWindow.SelectedVariantId;
            try
            {
                CozyFarmBuildingCatalog.GetHouse(id);
                return id;
            }
            catch (ArgumentException)
            {
                return CozyFarmBuildingCatalog.DefaultHouseId;
            }
        }

        private static void RebuildVisuals(
            Transform house,
            Sprite houseSprite,
            CozyFarmBuildingCatalog.HouseVariant variant)
        {
            if (houseSprite == null)
            {
                throw new ArgumentNullException(nameof(houseSprite));
            }

            for (int index = house.childCount - 1; index >= 0; index--)
            {
                UnityEngine.Object.DestroyImmediate(
                    house.GetChild(index).gameObject);
            }

            var visualRoot = new GameObject(VisualRootName);
            visualRoot.transform.SetParent(house, false);
            visualRoot.transform.localPosition = Vector3.zero;

            var facade = new GameObject(variant.DisplayName);
            facade.transform.SetParent(visualRoot.transform, false);

            Vector2 spriteSize = houseSprite.bounds.size;
            float scale = spriteSize.x <= Mathf.Epsilon ||
                spriteSize.y <= Mathf.Epsilon
                ? 1f
                : Mathf.Min(
                    variant.MaximumWidth / spriteSize.x,
                    variant.MaximumHeight / spriteSize.y);

            facade.transform.localScale = new Vector3(scale, scale, 1f);
            facade.transform.localPosition = new Vector3(0f, variant.Baseline, 0f);

            SpriteRenderer renderer = facade.AddComponent<SpriteRenderer>();
            renderer.sprite = houseSprite;
            renderer.color = Color.white;
            renderer.sortingLayerName = TopDownSortingLayers.World;
            renderer.sortingOrder = variant.SortingOrder;

            var shadow = new GameObject("Entrance Grounding Shadow");
            shadow.transform.SetParent(visualRoot.transform, false);
            shadow.transform.localPosition = new Vector3(
                variant.ShadowOffset.x,
                variant.ShadowOffset.y,
                0f);
            shadow.transform.localScale = new Vector3(
                variant.ShadowScale.x,
                variant.ShadowScale.y,
                1f);
            SpriteRenderer shadowRenderer = shadow.AddComponent<SpriteRenderer>();
            shadowRenderer.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(
                "UI/Skin/UISprite.psd");
            shadowRenderer.color = new Color(0.16f, 0.12f, 0.08f, 0.22f);
            shadowRenderer.sortingLayerName = TopDownSortingLayers.World;
            shadowRenderer.sortingOrder = variant.SortingOrder - 1;
        }

        private static void ApplyOptionalFunctionalMetadata(
            Scene scene,
            Transform house,
            CozyFarmBuildingCatalog.HouseVariant variant)
        {
            Transform portal = Find(scene, "House Entrance Portal");
            if (portal != null)
            {
                portal.position = house.TransformPoint(variant.PortalOffset);
            }

            Transform spawn = Find(scene, "Farm Spawn Point");
            if (spawn != null)
            {
                spawn.position = house.TransformPoint(variant.SpawnOffset);
            }

            BoxCollider2D collider = house.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = variant.ColliderSize;
                collider.offset = variant.ColliderOffset;
            }
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
