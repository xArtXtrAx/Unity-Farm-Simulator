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
            window.minSize = new Vector2(520f, 560f);
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
                "The preview overlays the exact occupied cells used by the generated prefab, Scene gizmo, snapping and collision checks. Generated sprites use a bottom-centre pivot, so (0, 0) is normally the visual ground/base.",
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

            DrawAuthoringPreview();

            anchorOffset = EditorGUILayout.Vector2Field(
                new GUIContent(
                    "Footprint Anchor",
                    "Local ground origin relative to the normalized visual base."),
                anchorOffset);
            if (GUILayout.Button("Reset anchor to visual base (0, 0)"))
            {
                anchorOffset = Vector2.zero;
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

        private void DrawAuthoringPreview()
        {
            Rect previewRect = GUILayoutUtility.GetRect(
                240f,
                230f,
                GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(previewRect, new Color(0.10f, 0.10f, 0.10f, 1f));

            if (definition.GeneratedSprite == null)
            {
                GUI.Label(
                    previewRect,
                    "Generate the building sprite to preview it.",
                    EditorStyles.centeredGreyMiniLabel);
                return;
            }

            Sprite sprite = definition.GeneratedSprite;
            Vector2 rawSize = sprite.bounds.size;
            float visualScale = Mathf.Min(
                definition.MaximumWidth / Mathf.Max(0.01f, rawSize.x),
                definition.MaximumHeight / Mathf.Max(0.01f, rawSize.y));
            Vector2 visualSize = rawSize * visualScale;

            float minX = -visualSize.x * 0.5f;
            float maxX = visualSize.x * 0.5f;
            float minY = 0f;
            float maxY = visualSize.y;
            foreach (Vector2Int offset in occupied)
            {
                Vector2 centre = anchorOffset + (Vector2)offset;
                minX = Mathf.Min(minX, centre.x - 0.5f);
                maxX = Mathf.Max(maxX, centre.x + 0.5f);
                minY = Mathf.Min(minY, centre.y - 0.5f);
                maxY = Mathf.Max(maxY, centre.y + 0.5f);
            }

            const float worldPadding = 0.3f;
            minX -= worldPadding;
            maxX += worldPadding;
            minY -= worldPadding;
            maxY += worldPadding;
            float worldWidth = Mathf.Max(0.01f, maxX - minX);
            float worldHeight = Mathf.Max(0.01f, maxY - minY);
            float pixelsPerUnit = Mathf.Min(
                previewRect.width / worldWidth,
                previewRect.height / worldHeight);
            Vector2 worldCentre = new Vector2(
                (minX + maxX) * 0.5f,
                (minY + maxY) * 0.5f);

            Vector2 Map(Vector2 world)
            {
                return new Vector2(
                    previewRect.center.x + (world.x - worldCentre.x) * pixelsPerUnit,
                    previewRect.center.y - (world.y - worldCentre.y) * pixelsPerUnit);
            }

            Vector2 spriteTopLeft = Map(new Vector2(-visualSize.x * 0.5f, visualSize.y));
            Vector2 spriteBottomRight = Map(new Vector2(visualSize.x * 0.5f, 0f));
            Rect imageRect = Rect.MinMaxRect(
                spriteTopLeft.x,
                spriteTopLeft.y,
                spriteBottomRight.x,
                spriteBottomRight.y);

            Texture texture = sprite.texture;
            Rect source = sprite.textureRect;
            Rect uv = new Rect(
                source.x / texture.width,
                source.y / texture.height,
                source.width / texture.width,
                source.height / texture.height);
            GUI.DrawTextureWithTexCoords(imageRect, texture, uv, true);

            foreach (Vector2Int offset in occupied)
            {
                Vector2 centre = anchorOffset + (Vector2)offset;
                Vector2 topLeft = Map(centre + new Vector2(-0.5f, 0.5f));
                Vector2 bottomRight = Map(centre + new Vector2(0.5f, -0.5f));
                Rect cellRect = Rect.MinMaxRect(
                    topLeft.x,
                    topLeft.y,
                    bottomRight.x,
                    bottomRight.y);
                bool anchorCell = offset == Vector2Int.zero;
                EditorGUI.DrawRect(
                    cellRect,
                    anchorCell
                        ? new Color(0.2f, 0.55f, 1f, 0.42f)
                        : new Color(0.2f, 1f, 0.35f, 0.34f));
                DrawOutline(
                    cellRect,
                    anchorCell
                        ? new Color(0.3f, 0.75f, 1f, 1f)
                        : new Color(0.25f, 1f, 0.4f, 0.9f));
            }

            Vector2 anchorPoint = Map(anchorOffset);
            EditorGUI.DrawRect(
                new Rect(anchorPoint.x - 5f, anchorPoint.y - 1f, 10f, 2f),
                Color.cyan);
            EditorGUI.DrawRect(
                new Rect(anchorPoint.x - 1f, anchorPoint.y - 5f, 2f, 10f),
                Color.cyan);
            GUI.Label(
                new Rect(previewRect.x + 6f, previewRect.y + 5f, 220f, 18f),
                "Exact prefab footprint preview",
                EditorStyles.miniLabel);
        }

        private static void DrawOutline(Rect rect, Color color)
        {
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, 1f), color);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMax - 1f, rect.width, 1f), color);
            EditorGUI.DrawRect(new Rect(rect.xMin, rect.yMin, 1f, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1f, rect.yMin, 1f, rect.height), color);
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
                            Repaint();
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
