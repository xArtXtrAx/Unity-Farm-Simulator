using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Presentation.Time;
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
            "house-interior-cozy-interior-v1";

        static CozyInteriorHouseSceneUpgrader()
        {
            EditorApplication.delayCall += EnsureUpgradedScene;
        }

        [MenuItem("Tools/Farm Simulator/Apply Cozy Interior To House Scene")]
        public static void ApplyFromMenu()
        {
            EnsureUpgradedScene(force: true);
        }

        public static void EnsureUpgradedScene()
        {
            EnsureUpgradedScene(force: false);
        }

        private static void EnsureUpgradedScene(bool force)
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureUpgradedScene;
                return;
            }

            if (IsOpen(ProjectSceneNames.HouseInteriorPath))
            {
                Debug.LogWarning(
                    "Close HouseInterior before applying Cozy Interior art.");
                return;
            }

            AssetImporter sceneImporter =
                AssetImporter.GetAtPath(ProjectSceneNames.HouseInteriorPath);
            if (sceneImporter == null)
            {
                EditorApplication.delayCall += EnsureUpgradedScene;
                return;
            }

            IReadOnlyDictionary<string, Sprite> sprites =
                CozyInteriorHouseArtPipeline.LoadSprites();
            string[] required =
            {
                "cozy_interior_wall_cream",
                "cozy_interior_floor_wood",
                "cozy_interior_door_cream",
                "cozy_interior_bed_cream",
                "cozy_interior_rug_warm",
            };

            if (required.Any(name => !sprites.ContainsKey(name)))
            {
                CozyInteriorHouseArtPipeline.EnsureAssets();
                return;
            }

            if (!force &&
                AssetDatabase.LoadAssetAtPath<SceneAsset>(
                    ProjectSceneNames.HouseInteriorPath) == null)
            {
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(
                ProjectSceneNames.HouseInteriorPath,
                OpenSceneMode.Additive);

            try
            {
                ReplacePatch(
                    scene,
                    "Wood Floor",
                    sprites["cozy_interior_floor_wood"]);
                ReplacePatch(
                    scene,
                    "Back Interior Wall",
                    sprites["cozy_interior_wall_cream"]);
                ReplacePatch(
                    scene,
                    "Left Interior Wall",
                    sprites["cozy_interior_wall_cream"]);
                ReplacePatch(
                    scene,
                    "Right Interior Wall",
                    sprites["cozy_interior_wall_cream"]);

                ReplaceSingle(
                    scene,
                    "Interior Door",
                    sprites["cozy_interior_door_cream"],
                    new Vector3(0.7f, 0.7f, 1f),
                    new Vector3(0f, -2.55f, 0f));

                ReplaceBed(
                    scene,
                    sprites["cozy_interior_bed_cream"]);
                ReplaceSingle(
                    scene,
                    "Woven Floor Runner",
                    sprites["cozy_interior_rug_warm"],
                    new Vector3(0.95f, 0.95f, 1f),
                    new Vector3(-0.2f, 0.25f, 0f));

                if (!EditorSceneManager.SaveScene(
                        scene,
                        ProjectSceneNames.HouseInteriorPath))
                {
                    throw new InvalidOperationException(
                        "Could not save upgraded HouseInterior.");
                }
            }
            finally
            {
                if (scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            sceneImporter =
                AssetImporter.GetAtPath(ProjectSceneNames.HouseInteriorPath);
            if (sceneImporter != null)
            {
                sceneImporter.userData =
                    HouseAndSleepScenePipeline.HouseImportSignature;
                sceneImporter.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Applied Cozy Interior floor, walls, door, rug and bed.");
        }

        private static void ReplacePatch(
            Scene scene,
            string objectName,
            Sprite sprite)
        {
            Transform root = Find(scene, objectName);
            if (root == null)
            {
                return;
            }

            foreach (SpriteRenderer renderer in
                     root.GetComponentsInChildren<SpriteRenderer>(true))
            {
                renderer.sprite = sprite;
                renderer.color = Color.white;
                renderer.transform.localScale = Vector3.one;
            }
        }

        private static void ReplaceSingle(
            Scene scene,
            string objectName,
            Sprite sprite,
            Vector3 scale,
            Vector3 position)
        {
            Transform target = Find(scene, objectName);
            if (target == null)
            {
                return;
            }

            SpriteRenderer renderer =
                target.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = target.gameObject.AddComponent<SpriteRenderer>();
            }

            renderer.sprite = sprite;
            renderer.color = Color.white;
            target.localScale = scale;
            target.localPosition = position;
        }

        private static void ReplaceBed(
            Scene scene,
            Sprite sprite)
        {
            Transform bed = Find(scene, "Hero Bed");
            if (bed == null)
            {
                return;
            }

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
            bedRenderer.sortingLayerName =
                TopDownSortingLayers.World;
            bedRenderer.sortingOrder = 34;
            visual.localPosition = Vector3.zero;
            visual.localScale = new Vector3(0.85f, 0.85f, 1f);

            BoxCollider2D collider =
                bed.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = new Vector2(1.9f, 2.6f);
                collider.offset = new Vector2(0f, 0.15f);
            }
        }

        private static Transform Find(
            Scene scene,
            string objectName)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform[] transforms =
                    root.GetComponentsInChildren<Transform>(true);
                Transform result = transforms.FirstOrDefault(
                    candidate => candidate.name == objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static bool IsOpen(string path)
        {
            Scene scene = SceneManager.GetSceneByPath(path);
            return scene.IsValid() && scene.isLoaded;
        }
    }
}
