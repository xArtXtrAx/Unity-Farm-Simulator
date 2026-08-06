using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public sealed class CozyBuildingFootprintWindow : EditorWindow
    {
        private CozyBuildingDefinition definition;
        private Vector2Int size = new Vector2Int(4, 3);
        private HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Footprint Editor")]
        public static void OpenFromMenu()
        {
            Open(Selection.activeObject as CozyBuildingDefinition);
        }

        public static void Open(CozyBuildingDefinition selected)
        {
            var window = GetWindow<CozyBuildingFootprintWindow>();
            window.titleContent = new GUIContent("Footprint Editor");
            window.minSize = new Vector2(420f, 360f);
            window.SetDefinition(selected);
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
                "Farm Development Kit — Footprint Editor",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Paint the logical ground cells occupied by the building. The anchor cell is marked A and should normally coincide with the doorway/base of the building. Visual roof overhangs do not need occupied cells.",
                MessageType.Info);

            CozyBuildingDefinition next = (CozyBuildingDefinition)EditorGUILayout.ObjectField(
                "Building",
                definition,
                typeof(CozyBuildingDefinition),
                false);
            if (next != definition)
            {
                SetDefinition(next);
            }

            if (definition == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a CozyBuildingDefinition asset from the Building Browser.",
                    MessageType.Warning);
                return;
            }

            Vector2Int nextSize = EditorGUILayout.Vector2IntField("Canvas size", size);
            nextSize.x = Mathf.Clamp(nextSize.x, 1, 12);
            nextSize.y = Mathf.Clamp(nextSize.y, 1, 12);
            if (nextSize != size)
            {
                size = nextSize;
                RemoveCellsOutsideCanvas();
            }

            EditorGUILayout.Space();
            DrawGrid();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Fill rectangle"))
                {
                    occupied.Clear();
                    foreach (Vector2Int cell in AllCanvasCells()) occupied.Add(cell);
                }
                if (GUILayout.Button("Clear"))
                {
                    occupied.Clear();
                    occupied.Add(Vector2Int.zero);
                }
                if (GUILayout.Button("Reset category default"))
                {
                    Undo.RecordObject(definition, "Reset building footprint");
                    definition.ResetFootprintToCategoryDefault();
                    EditorUtility.SetDirty(definition);
                    LoadFromDefinition();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save footprint", GUILayout.Height(30f)))
                {
                    Save();
                }
                using (new EditorGUI.DisabledScope(definition.GeneratedPrefab == null))
                {
                    if (GUILayout.Button("Save + regenerate prefab", GUILayout.Height(30f)))
                    {
                        Save();
                        CozyFarmBuildingPrefabGenerator.Generate(definition);
                    }
                }
            }

            EditorGUILayout.LabelField(
                $"Occupied cells: {occupied.Count} · Anchor: (0, 0)",
                EditorStyles.miniLabel);
        }

        private void DrawGrid()
        {
            float cellSize = Mathf.Clamp((position.width - 60f) / size.x, 28f, 56f);
            int minX = -((size.x - 1) / 2);
            int maxY = size.y - 1;

            for (int y = maxY; y >= 0; y--)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    for (int column = 0; column < size.x; column++)
                    {
                        Vector2Int cell = new Vector2Int(minX + column, y);
                        bool active = occupied.Contains(cell);
                        string label = cell == Vector2Int.zero ? "A" : active ? "■" : "";
                        Color previous = GUI.backgroundColor;
                        GUI.backgroundColor = cell == Vector2Int.zero
                            ? new Color(0.35f, 0.65f, 1f)
                            : active
                                ? new Color(0.35f, 0.9f, 0.45f)
                                : new Color(0.35f, 0.35f, 0.35f);
                        if (GUILayout.Button(label, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                        {
                            if (active && cell != Vector2Int.zero) occupied.Remove(cell);
                            else occupied.Add(cell);
                        }
                        GUI.backgroundColor = previous;
                    }
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private void Save()
        {
            occupied.Add(Vector2Int.zero);
            Undo.RecordObject(definition, "Edit building footprint");
            definition.SetFootprint(size, occupied);
            EditorUtility.SetDirty(definition);
            AssetDatabase.SaveAssets();
        }

        private void SetDefinition(CozyBuildingDefinition selected)
        {
            definition = selected;
            LoadFromDefinition();
        }

        private void LoadFromDefinition()
        {
            if (definition == null)
            {
                occupied.Clear();
                return;
            }

            size = definition.GridSize;
            occupied = new HashSet<Vector2Int>(definition.FootprintOffsets);
            occupied.Add(Vector2Int.zero);
        }

        private IEnumerable<Vector2Int> AllCanvasCells()
        {
            int minX = -((size.x - 1) / 2);
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    yield return new Vector2Int(minX + x, y);
                }
            }
        }

        private void RemoveCellsOutsideCanvas()
        {
            var allowed = new HashSet<Vector2Int>(AllCanvasCells());
            occupied.RemoveWhere(cell => !allowed.Contains(cell));
            occupied.Add(Vector2Int.zero);
        }
    }
}
