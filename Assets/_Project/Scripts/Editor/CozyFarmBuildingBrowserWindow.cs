using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public sealed class CozyFarmBuildingBrowserWindow : EditorWindow
    {
        private CozyBuildingCategory category = CozyBuildingCategory.House;
        private Vector2 scroll;
        private string selectedId = CozyFarmBuildingCatalog.DefaultHouseId;
        private IReadOnlyList<CozyBuildingDefinition> definitions = Array.Empty<CozyBuildingDefinition>();

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Building Browser")]
        public static void Open()
        {
            var window = GetWindow<CozyFarmBuildingBrowserWindow>();
            window.titleContent = new GUIContent("Building Browser");
            window.minSize = new Vector2(560f, 420f);
            window.Show();
        }

        private void OnEnable() => Reload();

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Farm Development Kit — Buildings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Browse definitions, author logical ground footprints, generate reusable prefabs and place them directly on the active scene grid.",
                MessageType.Info);

            CozyBuildingCategory nextCategory =
                (CozyBuildingCategory)EditorGUILayout.EnumPopup("Category", category);
            if (nextCategory != category)
            {
                category = nextCategory;
                selectedId = string.Empty;
                Reload();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Rebuild definitions", GUILayout.Height(26f)))
                {
                    CozyFarmBuildingRegistry.Rebuild();
                    Reload();
                }
                if (GUILayout.Button("Generate sprites", GUILayout.Height(26f)))
                {
                    CozyFarmBuildingCatalog.EnsureAssets();
                    CozyFarmBuildingRegistry.Rebuild();
                    Reload();
                }
                if (GUILayout.Button("Generate all prefabs", GUILayout.Height(26f)))
                {
                    CozyFarmBuildingPrefabGenerator.GenerateAll(CozyFarmBuildingRegistry.Rebuild());
                    Reload();
                }
            }

            EditorGUILayout.Space();
            IReadOnlyList<CozyBuildingDefinition> visible = definitions
                .Where(definition => definition.Category == category)
                .ToArray();
            if (visible.Count == 0)
            {
                EditorGUILayout.HelpBox($"No {category} definitions are registered yet.", MessageType.None);
                return;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (CozyBuildingDefinition definition in visible) DrawDefinition(definition);
            EditorGUILayout.EndScrollView();
        }

        private void DrawDefinition(CozyBuildingDefinition definition)
        {
            bool selected = string.Equals(selectedId, definition.Id, StringComparison.Ordinal);
            using (new EditorGUILayout.VerticalScope(selected ? "SelectionRect" : "box"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    Texture preview = definition.GeneratedSprite == null
                        ? null
                        : AssetPreview.GetAssetPreview(definition.GeneratedSprite) ??
                          AssetPreview.GetMiniThumbnail(definition.GeneratedSprite);
                    GUILayout.Label(preview, GUILayout.Width(96f), GUILayout.Height(96f));
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField(definition.DisplayName, EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("ID", definition.Id);
                        EditorGUILayout.LabelField("Atlas", definition.AtlasRect.ToString());
                        EditorGUILayout.LabelField("Footprint canvas", definition.GridSize.ToString());
                        EditorGUILayout.LabelField("Occupied cells", definition.FootprintOffsets.Count.ToString());
                        EditorGUILayout.LabelField("Prefab", definition.GeneratedPrefab == null ? "Not generated" : "Ready");
                        EditorGUILayout.LabelField("Interior", definition.SupportsInterior ? "Supported" : "None");
                        if (GUILayout.Button("Select"))
                        {
                            selectedId = definition.Id;
                            Selection.activeObject = definition;
                            EditorGUIUtility.PingObject(definition);
                        }
                    }
                }

                if (!selected) return;

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Locate sprite"))
                    {
                        Selection.activeObject = definition.GeneratedSprite;
                        EditorGUIUtility.PingObject(definition.GeneratedSprite);
                    }
                    if (GUILayout.Button("Edit footprint"))
                    {
                        Selection.activeObject = definition;
                        CozyBuildingFootprintWindow.Open(definition);
                    }
                    if (GUILayout.Button("Generate prefab"))
                    {
                        CozyFarmBuildingPrefabGenerator.Generate(definition);
                        Reload();
                    }
                    using (new EditorGUI.DisabledScope(definition.GeneratedPrefab == null))
                    {
                        if (GUILayout.Button("Locate prefab"))
                        {
                            Selection.activeObject = definition.GeneratedPrefab;
                            EditorGUIUtility.PingObject(definition.GeneratedPrefab);
                        }
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(definition.GeneratedPrefab == null))
                    {
                        if (GUILayout.Button("Place on scene grid"))
                        {
                            CozyBuildingGridPlacementUtility.PlacePrefab(definition.GeneratedPrefab);
                        }
                    }
                    using (new EditorGUI.DisabledScope(definition.Category != CozyBuildingCategory.House))
                    {
                        if (GUILayout.Button("Apply as hero house"))
                            CozyFarmHouseExteriorUpgrader.ApplyVariant(definition.Id);
                    }
                }
            }
        }

        private void Reload()
        {
            definitions = CozyFarmBuildingRegistry.LoadAll();
            if (definitions.Count > 0 && !definitions.Any(definition => definition.Id == selectedId))
                selectedId = definitions[0].Id;
            Repaint();
        }
    }
}
