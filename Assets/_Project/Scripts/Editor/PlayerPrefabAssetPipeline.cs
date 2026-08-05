using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Application.Player;
using FarmSimulator.Presentation.Calibration;
using FarmSimulator.Presentation.Player;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class PlayerPrefabAssetPipeline
    {
        private const string TagManagerPath =
            "ProjectSettings/TagManager.asset";

        static PlayerPrefabAssetPipeline()
        {
            EditorApplication.delayCall += EnsureAssets;
        }

        [MenuItem("Tools/Farm Simulator/Rebuild Player Prefab")]
        public static void RebuildAssets()
        {
            AssetDatabase.DeleteAsset(
                PlayerPrefabAssetCatalog.PrefabAssetPath);
            EditorApplication.delayCall += EnsureAssets;
        }

        public static void EnsureAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureAssets;
                return;
            }

            EnsureSortingLayers();

            if (!TryLoadFarmerAssets(
                    out Sprite idleDown,
                    out RuntimeAnimatorController controller))
            {
                EditorApplication.delayCall += EnsureAssets;
                return;
            }

            if (IsPrefabCurrent())
            {
                return;
            }

            CreateOrReplacePrefab(idleDown, controller);
        }

        private static bool TryLoadFarmerAssets(
            out Sprite idleDown,
            out RuntimeAnimatorController controller)
        {
            controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                FarmerSpriteAssetPipeline.ControllerAssetPath);

            string idleSpriteName = PlayerAnimationModel.SpriteName(
                PlayerAnimationModel.Frames(
                    PlayerAnimationState.IdleDown)[0]);
            idleDown = AssetDatabase
                .LoadAllAssetRepresentationsAtPath(
                    FarmerSpriteAssetPipeline.SpriteSheetAssetPath)
                .OfType<Sprite>()
                .FirstOrDefault(sprite => sprite.name == idleSpriteName);

            return idleDown != null && controller != null;
        }

        private static bool IsPrefabCurrent()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                PlayerPrefabAssetCatalog.PrefabAssetPath);
            AssetImporter importer = AssetImporter.GetAtPath(
                PlayerPrefabAssetCatalog.PrefabAssetPath);

            if (prefab == null || importer == null ||
                importer.userData != PlayerPrefabAssetCatalog.ImportSignature)
            {
                return false;
            }

            SpriteRenderer renderer =
                prefab.GetComponentInChildren<SpriteRenderer>(
                    includeInactive: true);
            Animator animator = prefab.GetComponentInChildren<Animator>(
                includeInactive: true);
            TopDownSpriteSorting sorting =
                prefab.GetComponent<TopDownSpriteSorting>();

            return prefab.GetComponent<PlayerPrefabIdentity>() != null &&
                prefab.GetComponent<TopDownPlayerMotor>() != null &&
                prefab.GetComponent<UnifiedMovementInput>() != null &&
                prefab.GetComponent<PlayerSpriteAnimator>() != null &&
                prefab.GetComponent<PlayerProxyFacingView>() != null &&
                sorting != null &&
                renderer != null &&
                animator != null &&
                animator.runtimeAnimatorController != null &&
                renderer.sortingLayerName == TopDownSortingLayers.Actors &&
                sorting.TargetRenderer == renderer &&
                sorting.Feet == prefab.transform;
        }

        private static void CreateOrReplacePrefab(
            Sprite idleDown,
            RuntimeAnimatorController controller)
        {
            EnsureFolder(PlayerPrefabAssetCatalog.AssetRoot);
            AssetDatabase.DeleteAsset(
                PlayerPrefabAssetCatalog.PrefabAssetPath);

            var root = new GameObject(
                PlayerPrefabAssetCatalog.RootObjectName);

            try
            {
                TopDownPlayerMotor motor =
                    root.AddComponent<TopDownPlayerMotor>();
                motor.Configure(
                    PlayerMovementModel.DefaultSpeedUnitsPerSecond);

                Rigidbody2D body = root.GetComponent<Rigidbody2D>();
                body.bodyType = RigidbodyType2D.Dynamic;
                body.gravityScale = 0f;
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
                body.collisionDetectionMode =
                    CollisionDetectionMode2D.Continuous;

                CapsuleCollider2D collider =
                    root.GetComponent<CapsuleCollider2D>();
                collider.direction = CapsuleDirection2D.Horizontal;
                collider.size = new Vector2(
                    LabSpatialCalibration.PlayerColliderWidth,
                    LabSpatialCalibration.PlayerColliderHeight);
                collider.offset = new Vector2(
                    0f,
                    LabSpatialCalibration.PlayerColliderOffsetY);
                collider.isTrigger = false;

                root.AddComponent<UnifiedMovementInput>();
                root.AddComponent<PlayerPrefabIdentity>();
                root.AddComponent<PlayerProxyFacingView>();

                var spriteObject = new GameObject(
                    PlayerSpriteAssetCatalog.SpriteVisualObjectName);
                spriteObject.transform.SetParent(root.transform, false);

                SpriteRenderer renderer =
                    spriteObject.AddComponent<SpriteRenderer>();
                renderer.sprite = idleDown;
                renderer.sortingLayerName = TopDownSortingLayers.Actors;

                Animator animator = spriteObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = controller;

                PlayerSpriteAnimator animationDriver =
                    root.AddComponent<PlayerSpriteAnimator>();
                animationDriver.Initialize(animator);

                TopDownSpriteSorting sorting =
                    root.AddComponent<TopDownSpriteSorting>();
                sorting.Initialize(renderer, root.transform);

                PrefabUtility.SaveAsPrefabAsset(
                    root,
                    PlayerPrefabAssetCatalog.PrefabAssetPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            AssetImporter importer = AssetImporter.GetAtPath(
                PlayerPrefabAssetCatalog.PrefabAssetPath);
            if (importer != null)
            {
                importer.userData =
                    PlayerPrefabAssetCatalog.ImportSignature;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Generated reusable Player prefab and top-down sorting layers.");
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            int separator = folderPath.LastIndexOf('/');
            if (separator <= 0)
            {
                throw new InvalidOperationException(
                    $"Cannot create Unity folder '{folderPath}'.");
            }

            string parent = folderPath[..separator];
            string name = folderPath[(separator + 1)..];
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static void EnsureSortingLayers()
        {
            UnityEngine.Object[] assets =
                AssetDatabase.LoadAllAssetsAtPath(TagManagerPath);
            if (assets.Length == 0)
            {
                Debug.LogError(
                    "Could not load ProjectSettings/TagManager.asset.");
                return;
            }

            var serialized = new SerializedObject(assets[0]);
            SerializedProperty layers =
                serialized.FindProperty("m_SortingLayers");
            if (layers == null)
            {
                Debug.LogError(
                    "TagManager does not expose m_SortingLayers.");
                return;
            }

            var existingNames = new HashSet<string>();
            var existingIds = new HashSet<uint>();
            for (int index = 0; index < layers.arraySize; index++)
            {
                SerializedProperty element =
                    layers.GetArrayElementAtIndex(index);
                existingNames.Add(
                    element.FindPropertyRelative("name").stringValue);
                existingIds.Add((uint)element
                    .FindPropertyRelative("uniqueID").longValue);
            }

            bool changed = false;
            foreach (string layerName in
                     TopDownSortingLayers.RequiredNames)
            {
                if (existingNames.Contains(layerName))
                {
                    continue;
                }

                int newIndex = layers.arraySize;
                layers.InsertArrayElementAtIndex(newIndex);
                SerializedProperty element =
                    layers.GetArrayElementAtIndex(newIndex);
                element.FindPropertyRelative("name").stringValue =
                    layerName;

                uint uniqueId = StableUniqueId(layerName);
                while (uniqueId == 0 || existingIds.Contains(uniqueId))
                {
                    uniqueId++;
                }

                element.FindPropertyRelative("uniqueID").longValue =
                    uniqueId;
                SerializedProperty locked =
                    element.FindPropertyRelative("locked");
                if (locked != null)
                {
                    locked.boolValue = false;
                }

                existingNames.Add(layerName);
                existingIds.Add(uniqueId);
                changed = true;
            }

            if (changed)
            {
                serialized.ApplyModifiedPropertiesWithoutUndo();
                AssetDatabase.SaveAssets();
            }
        }

        private static uint StableUniqueId(string value)
        {
            const uint offsetBasis = 2166136261;
            const uint prime = 16777619;
            uint hash = offsetBasis;

            foreach (char character in value)
            {
                hash ^= character;
                hash *= prime;
            }

            return hash;
        }
    }
}
