using System;
using System.Collections.Generic;
using System.IO;
using FarmSimulator.Presentation.Buildings;
using FarmSimulator.Presentation.Player;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public static class CozyFarmBuildingPrefabGenerator
    {
        public const string PrefabRoot =
            "Assets/_Project/Buildings/CozyFarm/Prefabs";
        public const string CompositionRootName = "Building Composition";
        public const string VisualName = "Building Visual";
        public const string DoorAnchorName = "Door Anchor";
        public const string PortalAnchorName = "Portal Anchor";
        public const string SpawnAnchorName = "Spawn Anchor";

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Generate Building Prefabs")]
        public static void GenerateAllFromMenu()
        {
            IReadOnlyList<CozyBuildingDefinition> definitions =
                CozyFarmBuildingRegistry.Rebuild();
            int generated = GenerateAll(definitions);
            EditorUtility.DisplayDialog(
                "Farm Development Kit",
                $"Generated {generated} reusable building prefabs.",
                "OK");
        }

        public static int GenerateAll(IReadOnlyList<CozyBuildingDefinition> definitions)
        {
            if (definitions == null) throw new ArgumentNullException(nameof(definitions));
            EnsureFolder(PrefabRoot);
            int generated = 0;
            foreach (CozyBuildingDefinition definition in definitions)
            {
                Generate(definition);
                generated++;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return generated;
        }

        public static GameObject Generate(CozyBuildingDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (definition.GeneratedSprite == null)
                throw new InvalidOperationException($"Building '{definition.Id}' has no generated sprite.");

            EnsureFolder(PrefabRoot);
            string path = GetPrefabPath(definition);
            var root = new GameObject(definition.DisplayName);
            try
            {
                root.transform.position = Vector3.zero;
                root.AddComponent<GridBuildingFootprint>()
                    .Configure(
                        definition.Id,
                        definition.GridSize,
                        definition.FootprintOffsets);

                Transform composition = Child(CompositionRootName, root.transform);
                Transform visual = Child(VisualName, composition);
                SpriteRenderer renderer = visual.gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = definition.GeneratedSprite;
                renderer.color = Color.white;
                renderer.sortingLayerName = TopDownSortingLayers.World;
                renderer.sortingOrder = definition.SortingOrder;

                Vector2 spriteSize = definition.GeneratedSprite.bounds.size;
                float scale = Mathf.Min(
                    definition.MaximumWidth / Mathf.Max(0.01f, spriteSize.x),
                    definition.MaximumHeight / Mathf.Max(0.01f, spriteSize.y));
                visual.localScale = new Vector3(scale, scale, 1f);
                visual.localPosition = new Vector3(0f, -definition.Baseline, 0f);

                BoxCollider2D collider = root.AddComponent<BoxCollider2D>();
                collider.size = definition.ColliderSize;
                collider.offset = definition.ColliderOffset;

                Child(DoorAnchorName, root.transform).localPosition = definition.PortalOffset;
                Child(PortalAnchorName, root.transform).localPosition = definition.PortalOffset;
                Child(SpawnAnchorName, root.transform).localPosition = definition.SpawnOffset;

                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
                if (prefab == null)
                    throw new InvalidOperationException($"Could not save generated building prefab '{path}'.");

                definition.AssignGeneratedPrefab(prefab);
                EditorUtility.SetDirty(definition);
                return prefab;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        public static string GetPrefabPath(CozyBuildingDefinition definition) =>
            PrefabRoot + "/" + definition.Id + ".prefab";

        private static Transform Child(string name, Transform parent)
        {
            var child = new GameObject(name).transform;
            child.SetParent(parent, false);
            return child;
        }

        private static void EnsureFolder(string assetPath)
        {
            string[] parts = assetPath.Replace('\\', '/').Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[index]);
                current = next;
            }
        }
    }
}
