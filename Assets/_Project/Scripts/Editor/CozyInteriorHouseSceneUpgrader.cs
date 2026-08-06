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
    [InitializeOnLoad]
    public static class CozyInteriorHouseSceneUpgrader
    {
        public const string UpgradeSignature =
            "house-interior-cozy-interior-v2";

        public const string MarkerName =
            "Cozy Interior Visual Upgrade";

        private static readonly string[] RequiredSpriteNames =
        {
            "cozy_interior_wall_cream",
            "cozy_interior_floor_wood",
            "cozy_interior_door_cream",
            "cozy_interior_bed_cream",
            "cozy_interior_rug_warm",
        };

        static CozyInteriorHouseSceneUpgrader()
        {
            EditorApplication.delayCall += EnsureUpgradedScene;
        }

        [MenuItem("Tools/Farm Simulator/Apply Cozy Interior To House Scene")]
        public static void ApplyFromMenu()
        {
            ApplyToHouseScene(force: true);
        }

        public static void EnsureUpgradedScene()
        {
            ApplyToHouseScene(force: false);
        }

        public static bool ApplyToHouseScene(bool force)
        {
            if (EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureUpgradedScene;
                return false;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning(
                    "Cozy Interior art cannot be applied while entering " +
                    "or running Play Mode.");
                return false;
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    ProjectSceneNames.HouseInteriorPath) == null)
            {
                EditorApplication.delayCall += EnsureUpgradedScene;
                Debug.LogWarning(
                    "Cozy Interior is waiting for HouseInterior.unity.");
                return false;
            }

            IReadOnlyDictionary<string, Sprite> sprites =
                CozyInteriorHouseArtPipeline.LoadSprites();
            string[] missingSprites = RequiredSpriteNames
                .Where(name => !sprites.ContainsKey(name))
                .ToArray();

            if (missingSprites.Length > 0)
            {
                Debug.LogWarning(
                    "Cozy Interior is waiting for curated sprites: " +
                    string.Join(", ", missingSprites) + ".");
                CozyInteriorHouseArtPipeline.EnsureAssets();
                EditorApplication.delayCall += EnsureUpgradedScene;
                return false;
            }

            Scene scene = SceneManager.GetSceneByPath(
                ProjectSceneNames.HouseInteriorPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            Scene previousActive = SceneManager.GetActiveScene();

            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(
                    ProjectSceneNames.HouseInteriorPath,
                    OpenSceneMode.Additive);
            }

            bool saved = false;
            try
            {
                if (!force && Find(scene, MarkerName) != null)
                {
                    return true;
                }

                ApplyVisuals(scene, sprites);
                EnsureMarker(scene);
                EditorSceneManager.MarkSceneDirty(scene);

                if (openedHere || force)
                {
                    saved = EditorSceneManager.SaveScene(
                        scene,
                        ProjectSceneNames.HouseInteriorPath);
                    if (!saved)
                    {
                        throw new InvalidOperationException(
                            "Could not save upgraded HouseInterior.");
                    }
                }
                else
                {
                    Debug.Log(
                        "Cozy Interior art was applied to the open " +
                        "HouseInterior scene. Save the scene to keep it.");
                }
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }

                if (previousActive.IsValid() && previousActive.isLoaded)
                {
                    SceneManager.SetActiveScene(previousActive);
                }
            }

            if (saved)
            {
                AssetImporter sceneImporter = AssetImporter.GetAtPath(
                    ProjectSceneNames.HouseInteriorPath);
                if (sceneImporter != null)
                {
                    sceneImporter.userData =
                        HouseAndSleepScenePipeline.HouseImportSignature;
                    sceneImporter.SaveAndReimport();
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log(
                "Applied Cozy Interior v2: tiled floor and walls, " +
                "real door, rug and bed. House interaction and sleep " +
                "components were preserved.");
            return true;
        }

        private static void ApplyVisuals(
            Scene scene,
            IReadOnlyDictionary<string, Sprite> sprites)
        {
            ReplacePatch(
                scene,
                "Wood Floor",
                sprites["cozy_interior_floor_wood"],
                new Vector3(1.7f, 1.5f, 1f));
            ReplacePatch(
                scene,
                "Back Interior Wall",
                sprites["cozy_interior_wall_cream"],
                new Vector3(1.7f, 1f, 1f));
            ReplacePatch(
                scene,
                "Left Interior Wall",
                sprites["cozy_interior_wall_cream"],
                new Vector3(0.55f, 1.48f, 1f));
            ReplacePatch(
                scene,
                "Right Interior Wall",
                sprites["cozy_interior_wall_cream"],
                new Vector3(0.55f, 1.48f, 1f));

            ReplaceSingle(
                scene,
                "Interior Door",
                sprites["cozy_interior_door_cream"],
                new Vector3(0.6f, 0.6f, 1f),
                new Vector3(0f, -2.35f, 0f),
                TopDownSortingLayers.World,
                25);

            ReplaceBed(
                scene,
                sprites["cozy_interior_bed_cream"]);
            ReplaceSingle(
                scene,
                "Woven Floor Runner",
                sprites["cozy_interior_rug_warm"],
                new Vector3(0.55f, 0.55f, 1f),
                new Vector3(-0.2f, 0.25f, 0f),
                TopDownSortingLayers.Ground,
                -80);
        }

        private static void ReplacePatch(
            Scene scene,
            string objectName,
            Sprite sprite,
            Vector3 tileScale)
        {
            Transform root = FindRequired(scene, objectName);
            SpriteRenderer[] renderers =
                root.GetComponentsInChildren<SpriteRenderer>(true);

            if (renderers.Length == 0)
            {
                throw new InvalidOperationException(
                    $"'{objectName}' has no SpriteRenderer children.");
            }

            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.enabled = true;
                renderer.sprite = sprite;
                renderer.color = Color.white;
                renderer.transform.localScale = tileScale;
            }
        }

        private static void ReplaceSingle(
            Scene scene,
            string objectName,
            Sprite sprite,
            Vector3 scale,
            Vector3 position,
            string sortingLayer,
            int sortingOrder)
        {
            Transform target = FindRequired(scene, objectName);
            SpriteRenderer renderer =
                target.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = target.gameObject.AddComponent<SpriteRenderer>();
            }

            renderer.enabled = true;
            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = sortingOrder;
            target.localScale = scale;
            target.localPosition = position;
        }

        private static void ReplaceBed(Scene scene, Sprite sprite)
        {
            Transform bed = FindRequired(scene, "Hero Bed");

            foreach (SpriteRenderer renderer in
                     bed.GetComponentsInChildren<SpriteRenderer>(true))
            {
                renderer.enabled = false;
            }

            Transform visual = bed.Find("Cozy Interior Bed");
            if (visual == null)
            {
                var go = new GameObject("Cozy Interior Bed");
                visual = go.transform;
                visual.SetParent(bed, false);
            }

            SpriteRenderer bedRenderer =
                visual.GetComponent<SpriteRenderer>();
            if (bedRenderer == null)
            {
                bedRenderer =
                    visual.gameObject.AddComponent<SpriteRenderer>();
            }

            bedRenderer.enabled = true;
            bedRenderer.sprite = sprite;
            bedRenderer.color = Color.white;
            bedRenderer.sortingLayerName = TopDownSortingLayers.World;
            bedRenderer.sortingOrder = 34;
            visual.localPosition = Vector3.zero;
            visual.localScale = new Vector3(0.65f, 0.65f, 1f);

            BoxCollider2D collider = bed.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                throw new InvalidOperationException(
                    "Hero Bed lost its BoxCollider2D.");
            }

            collider.size = new Vector2(1.9f, 2.6f);
            collider.offset = new Vector2(0f, 0.15f);
        }

        private static void EnsureMarker(Scene scene)
        {
            if (Find(scene, MarkerName) != null)
            {
                return;
            }

            Transform world = FindRequired(scene, "House Interior World");
            var marker = new GameObject(MarkerName);
            marker.transform.SetParent(world, false);
        }

        private static Transform FindRequired(
            Scene scene,
            string objectName)
        {
            Transform result = Find(scene, objectName);
            if (result == null)
            {
                throw new InvalidOperationException(
                    $"HouseInterior is missing required object " +
                    $"'{objectName}'. Rebuild the house scenes first.");
            }

            return result;
        }

        private static Transform Find(Scene scene, string objectName)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform result = root
                    .GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(
                        candidate => candidate.name == objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
