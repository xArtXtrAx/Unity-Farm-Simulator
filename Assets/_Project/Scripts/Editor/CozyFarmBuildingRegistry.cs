using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public static class CozyFarmBuildingRegistry
    {
        public const string DefinitionRoot =
            "Assets/_Project/Buildings/CozyFarm/Definitions";

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Rebuild Building Definitions")]
        public static void RebuildFromMenu()
        {
            IReadOnlyList<CozyBuildingDefinition> definitions = Rebuild();
            EditorUtility.DisplayDialog(
                "Farm Development Kit",
                $"Generated {definitions.Count} categorized building definitions.",
                "OK");
        }

        public static IReadOnlyList<CozyBuildingDefinition> Rebuild()
        {
            CozyFarmBuildingCatalog.EnsureAssets();
            EnsureFolder(DefinitionRoot);

            var definitions = new List<CozyBuildingDefinition>();
            foreach (CozyFarmBuildingCatalog.HouseVariant house in
                     CozyFarmBuildingCatalog.Houses)
            {
                string path = GetDefinitionPath(house.Id);
                CozyBuildingDefinition definition =
                    AssetDatabase.LoadAssetAtPath<CozyBuildingDefinition>(path);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<CozyBuildingDefinition>();
                    definition.name = house.DisplayName;
                    AssetDatabase.CreateAsset(definition, path);
                }

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(house.GeneratedPath);
                definition.ConfigureFromHouse(house, sprite);
                EditorUtility.SetDirty(definition);
                definitions.Add(definition);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return definitions;
        }

        public static IReadOnlyList<CozyBuildingDefinition> LoadAll()
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:CozyBuildingDefinition",
                new[] { DefinitionRoot });
            if (guids.Length == 0)
            {
                return Rebuild();
            }

            return guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<CozyBuildingDefinition>)
                .Where(definition => definition != null)
                .OrderBy(definition => definition.Category)
                .ThenBy(definition => definition.DisplayName, StringComparer.Ordinal)
                .ToArray();
        }

        public static IReadOnlyList<CozyBuildingDefinition> LoadCategory(
            CozyBuildingCategory category)
        {
            return LoadAll()
                .Where(definition => definition.Category == category)
                .ToArray();
        }

        public static CozyBuildingDefinition Get(string id)
        {
            CozyBuildingDefinition result = LoadAll()
                .FirstOrDefault(definition =>
                    string.Equals(definition.Id, id, StringComparison.Ordinal));
            if (result == null)
            {
                throw new ArgumentException(
                    $"Unknown Cozy building definition '{id}'.",
                    nameof(id));
            }

            return result;
        }

        public static string GetDefinitionPath(string id) =>
            DefinitionRoot + "/" + id + ".asset";

        private static void EnsureFolder(string assetPath)
        {
            string[] parts = assetPath.Replace('\\', '/').Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }
    }
}
