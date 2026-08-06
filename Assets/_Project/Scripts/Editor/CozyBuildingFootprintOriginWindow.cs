using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public sealed class CozyBuildingFootprintOriginWindow : EditorWindow
    {
        private CozyBuildingDefinition definition;
        private Vector2 anchorOffset;
        private float nudgeStep = 0.5f;

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Footprint Origin")]
        public static void OpenFromMenu()
        {
            CozyBuildingDefinition selected =
                Selection.activeObject as CozyBuildingDefinition ??
                CozyFarmBuildingRegistry.LoadAll().FirstOrDefault();
            Open(selected);
        }

        public static void Open(CozyBuildingDefinition selected)
        {
            var window = GetWindow<CozyBuildingFootprintOriginWindow>();
            window.titleContent = new GUIContent("Footprint Origin");
            window.minSize = new Vector2(390f, 330f);
            window.SetDefinition(
                selected ?? CozyFarmBuildingRegistry.LoadAll().FirstOrDefault());
            window.Show();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is CozyBuildingDefinition selected)
            {
                SetDefinition(selected);
                Repaint();
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "Farm Development Kit — Footprint Origin",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Choose where the logical footprint begins relative to the building visual. " +
                "The anchor is the centre of cell (0, 0). Therefore the footprint's lower edge " +
                "is Anchor Y minus 0.5. Use a lower edge of 0 to align the footprint exactly " +
                "with the bottom of a bottom-centre-pivot sprite.",
                MessageType.Info);

            CozyBuildingDefinition next = (CozyBuildingDefinition)EditorGUILayout.ObjectField(
                "Building",
                definition,
                typeof(CozyBuildingDefinition),
                false);
            if (next != definition) SetDefinition(next);

            if (definition == null)
            {
                EditorGUILayout.HelpBox(
                    "No CozyBuildingDefinition is available. Rebuild definitions or select one in the Building Browser.",
                    MessageType.Warning);
                if (GUILayout.Button("Rebuild definitions", GUILayout.Height(28f)))
                {
                    IReadOnlyList<CozyBuildingDefinition> rebuilt =
                        CozyFarmBuildingRegistry.Rebuild();
                    SetDefinition(rebuilt.FirstOrDefault());
                }
                return;
            }

            EditorGUILayout.Space();
            anchorOffset = EditorGUILayout.Vector2Field("Anchor cell centre", anchorOffset);

            float lowerEdge = EditorGUILayout.FloatField(
                new GUIContent(
                    "Footprint lower edge Y",
                    "World-local Y coordinate of the lower border of the first footprint row."),
                anchorOffset.y - 0.5f);
            anchorOffset.y = lowerEdge + 0.5f;

            EditorGUILayout.LabelField(
                $"Current lower edge: {anchorOffset.y - 0.5f:0.###} · Anchor: ({anchorOffset.x:0.###}, {anchorOffset.y:0.###})",
                EditorStyles.miniLabel);

            EditorGUILayout.Space();
            nudgeStep = EditorGUILayout.FloatField("Nudge step", nudgeStep);
            nudgeStep = Mathf.Max(0.05f, nudgeStep);

            DrawNudgePad();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Align lower edge to visual base"))
                {
                    anchorOffset.y = 0.5f;
                }
                if (GUILayout.Button("Centre anchor on root"))
                {
                    anchorOffset = Vector2.zero;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save origin", GUILayout.Height(30f)))
                {
                    Save();
                }
                if (GUILayout.Button("Save + regenerate prefab", GUILayout.Height(30f)))
                {
                    Save();
                    CozyFarmBuildingPrefabGenerator.Generate(definition);
                }
            }
        }

        private void DrawNudgePad()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField("Move footprint origin", EditorStyles.boldLabel);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("▲", GUILayout.Width(56f), GUILayout.Height(28f)))
                    {
                        anchorOffset.y += nudgeStep;
                    }
                    GUILayout.FlexibleSpace();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("◀", GUILayout.Width(56f), GUILayout.Height(28f)))
                    {
                        anchorOffset.x -= nudgeStep;
                    }
                    if (GUILayout.Button("●", GUILayout.Width(56f), GUILayout.Height(28f)))
                    {
                        anchorOffset = new Vector2(0f, 0.5f);
                    }
                    if (GUILayout.Button("▶", GUILayout.Width(56f), GUILayout.Height(28f)))
                    {
                        anchorOffset.x += nudgeStep;
                    }
                    GUILayout.FlexibleSpace();
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("▼", GUILayout.Width(56f), GUILayout.Height(28f)))
                    {
                        anchorOffset.y -= nudgeStep;
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private void Save()
        {
            Undo.RecordObject(definition, "Move building footprint origin");
            definition.SetFootprintAnchor(anchorOffset);
            EditorUtility.SetDirty(definition);
            AssetDatabase.SaveAssets();
        }

        private void SetDefinition(CozyBuildingDefinition selected)
        {
            definition = selected;
            anchorOffset = definition == null
                ? Vector2.zero
                : definition.FootprintAnchorOffset;
        }
    }
}
