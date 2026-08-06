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
        private Vector2 anchorOffset;
        private HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Footprint Editor")]
        public static void OpenFromMenu()
        {
            CozyBuildingDefinition selected =
                Selection.activeObject as CozyBuildingDefinition ??
                CozyFarmBuildingRegistry.LoadAll().FirstOrDefault();
            Open(selected);
        }

        public static void Open(CozyBuildingDefinition selected)
        {
            var window = GetWindow<CozyBuildingFootprintWindow>();
            window.titleContent = new GUIContent("Footprint Editor");
            window.minSize = new Vector2(520f, 500f);
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
                "Farm Development Kit — Building Authoring",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The Footprint Anchor is the single origin used by the prefab, Scene preview, snapping and collision checks. Place it at the doorway/base, then paint only the ground cells that the building truly occupies.",
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
                    "No building definitions are available. Rebuild them or open a building from the Building Browser.",
                    MessageType.Warning);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Rebuild definitions", GUILayout.Height(28f)))
                    {
                        IReadOnlyList<CozyBuildingDefinition> rebuilt =
                            CozyFarmBuildingRegistry.Rebuild();
                        SetDefinition(rebuilt.FirstOrDefault());
                    }
                    if (GUILayout.Button("Open Building Browser", GUILayout.Height(28f)))
                    {
                        CozyFarmBuildingBrowserWindow.Open();
                    }
                }
                return;
            }

            DrawSpritePreview();

            anchorOffset = EditorGUILayout.Vector2Field(
                new GUIContent(
                    "Footprint Anchor",
                    "Local position of the grid origin relative to the prefab root."),
                anchorOffset);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use portal/door position"))
                {
                    anchorOffset = definition.PortalOffset;
                }
                if (GUILayout.Button("Reset anchor to root"))
                {
                    anchorOffset = Vector2.zero;
                }
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
                if (GUILayout.Button("Save authoring", GUILayout.Height(30f))) Save();
                if (GUILayout.Button("Save + regenerate prefab", GUILayout.Height(30f)))
                {
                    Save();
                    CozyFarmBuildingPrefabGenerator.Generate(definition);
                }
            }

            EditorGUILayout.LabelField(
                $"Occupied cells: {occupied.Count} · Logical anchor cell: (0, 0) · Local anchor: {anchorOffset}",
                EditorStyles.miniLabel);
        }

        private void DrawSpritePreview()
        {
            Rect rect = GUILayoutUtility.GetRect(200f, 180f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.12f, 1f));
            if (definition.GeneratedSprite == null)
            {
                GUI.Label(rect, "Generate the building sprite to preview it.", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            Texture texture = definition.GeneratedSprite.texture;
            Rect source = definition.GeneratedSprite.textureRect;
            Rect uv = new Rect(
                source.x / texture.width,
                source.y / texture.height,
                source.width / texture.width,
                source.height / texture.height);
            float aspect = source.width / source.height;
            Rect imageRect = rect;
            if (rect.width / rect.height > aspect)
            {
                imageRect.width = rect.height * aspect;
                imageRect.x += (rect.width - imageRect.width) * 0.5f;
            }
            else
            {
                imageRect.height = rect.width / aspect;
                imageRect.y += (rect.height - imageRect.height) * 0.5f;
            }
            GUI.DrawTextureWithTexCoords(imageRect, texture, uv, true);
            EditorGUI.DrawRect(
                new Rect(rect.center.x - 1f, rect.yMax - 18f, 2f, 18f),
                Color.cyan);
            GUI.Label(
                new Rect(rect.center.x + 4f, rect.yMax - 20f, 130f, 18f),
                "Footprint anchor",
                EditorStyles.miniLabel);
        }

        private void DrawGrid()
        {
            float cellSize = Mathf.Clamp((position.width - 80f) / size.x, 28f, 54f);
            int minX = -((size.x - 1) / 2);
            for (int y = size.y - 1; y >= 0; y--)
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
            Undo.RecordObject(definition, "Edit building authoring");
            definition.SetFootprint(size, occupied);
            definition.SetFootprintAnchor(anchorOffset);
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
            anchorOffset = definition.FootprintAnchorOffset;
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
