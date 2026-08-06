using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Presentation.Buildings;
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
        private Vector2 colliderCentre;
        private Vector2 colliderSize = Vector2.one;
        private float colliderNudgeStep = 0.1f;
        private bool showVisualBounds = true;
        private bool showColliderBounds = true;
        private bool showFootprintOverlay = true;

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
            window.minSize = new Vector2(520f, 760f);
            window.SetDefinition(selected ?? CozyFarmBuildingRegistry.LoadAll().FirstOrDefault());
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
            EditorGUILayout.LabelField("Farm Development Kit — Building Authoring", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "The preview separates visual bounds, physical collider and logical ground footprint. Collider values are authored in prefab base space: (0, 0) is the bottom-centre visual base.",
                MessageType.Info);

            CozyBuildingDefinition next = (CozyBuildingDefinition)EditorGUILayout.ObjectField(
                "Building", definition, typeof(CozyBuildingDefinition), false);
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
                        IReadOnlyList<CozyBuildingDefinition> rebuilt = CozyFarmBuildingRegistry.Rebuild();
                        SetDefinition(rebuilt.FirstOrDefault());
                    }
                    if (GUILayout.Button("Open Building Browser", GUILayout.Height(28f)))
                    {
                        CozyFarmBuildingBrowserWindow.Open();
                    }
                }
                return;
            }

            EditorGUILayout.LabelField("Preview overlays", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                showVisualBounds = EditorGUILayout.ToggleLeft("Visual bounds", showVisualBounds, GUILayout.Width(125f));
                showColliderBounds = EditorGUILayout.ToggleLeft("Collider", showColliderBounds, GUILayout.Width(95f));
                showFootprintOverlay = EditorGUILayout.ToggleLeft("Footprint", showFootprintOverlay, GUILayout.Width(105f));
            }

            DrawAuthoringPreview();
            DrawColliderAuthoring();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Footprint authoring", EditorStyles.boldLabel);
            anchorOffset = EditorGUILayout.Vector2Field(
                new GUIContent("Footprint Anchor", "Local ground origin relative to the normalized visual base."),
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
                $"Occupied cells: {occupied.Count} · Footprint anchor: {anchorOffset} · Collider centre: {colliderCentre} · Collider size: {colliderSize}",
                EditorStyles.miniLabel);
        }

        private void DrawColliderAuthoring()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Collider authoring", EditorStyles.boldLabel);
            colliderCentre = EditorGUILayout.Vector2Field(
                new GUIContent("Collider Center", "Centre relative to the prefab bottom-centre visual base."),
                colliderCentre);
            colliderSize = SanitizeColliderSize(EditorGUILayout.Vector2Field("Collider Size", colliderSize));
            colliderNudgeStep = Mathf.Clamp(
                EditorGUILayout.FloatField("Nudge step", colliderNudgeStep),
                0.01f,
                1f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Move", GUILayout.Width(48f));
                if (GUILayout.Button("←")) colliderCentre.x -= colliderNudgeStep;
                if (GUILayout.Button("→")) colliderCentre.x += colliderNudgeStep;
                if (GUILayout.Button("↓")) colliderCentre.y -= colliderNudgeStep;
                if (GUILayout.Button("↑")) colliderCentre.y += colliderNudgeStep;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Size", GUILayout.Width(48f));
                if (GUILayout.Button("Narrower")) colliderSize.x -= colliderNudgeStep * 2f;
                if (GUILayout.Button("Wider")) colliderSize.x += colliderNudgeStep * 2f;
                if (GUILayout.Button("Shorter")) colliderSize.y -= colliderNudgeStep * 2f;
                if (GUILayout.Button("Taller")) colliderSize.y += colliderNudgeStep * 2f;
            }
            colliderSize = SanitizeColliderSize(colliderSize);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Align bottom to visual base"))
                {
                    colliderCentre.y = colliderSize.y * 0.5f;
                }
                if (GUILayout.Button("Reset collider to catalog default"))
                {
                    Undo.RecordObject(definition, "Reset building collider");
                    definition.ResetColliderToCatalogDefault();
                    EditorUtility.SetDirty(definition);
                    colliderSize = definition.ColliderSize;
                    colliderCentre = CozyFarmBuildingPrefabGenerator.ToPrefabBaseSpace(
                        definition.ColliderOffset,
                        definition.Baseline);
                    Repaint();
                }
            }

            EditorGUILayout.HelpBox(
                "The orange rectangle updates immediately. Save + regenerate prefab applies these values to the reusable BoxCollider2D. Rebuilding definitions preserves authored collider values.",
                MessageType.None);
        }

        private void DrawAuthoringPreview()
        {
            Rect previewRect = GUILayoutUtility.GetRect(260f, 265f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(previewRect, new Color(0.10f, 0.10f, 0.10f, 1f));

            if (definition.GeneratedSprite == null)
            {
                GUI.Label(previewRect, "Generate the building sprite to preview it.", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            Sprite sprite = definition.GeneratedSprite;
            Vector2 rawSize = sprite.bounds.size;
            float visualScale = Mathf.Min(
                definition.MaximumWidth / Mathf.Max(0.01f, rawSize.x),
                definition.MaximumHeight / Mathf.Max(0.01f, rawSize.y));
            Vector2 visualSize = rawSize * visualScale;

            Vector2 safeColliderSize = SanitizeColliderSize(colliderSize);
            Vector2 colliderMin = colliderCentre - safeColliderSize * 0.5f;
            Vector2 colliderMax = colliderCentre + safeColliderSize * 0.5f;

            float minX = Mathf.Min(-visualSize.x * 0.5f, colliderMin.x);
            float maxX = Mathf.Max(visualSize.x * 0.5f, colliderMax.x);
            float minY = Mathf.Min(0f, colliderMin.y);
            float maxY = Mathf.Max(visualSize.y, colliderMax.y);

            foreach (Vector2Int offset in occupied)
            {
                Vector2 centre = anchorOffset + (Vector2)offset;
                minX = Mathf.Min(minX, centre.x - 0.5f);
                maxX = Mathf.Max(maxX, centre.x + 0.5f);
                minY = Mathf.Min(minY, centre.y - 0.5f);
                maxY = Mathf.Max(maxY, centre.y + 0.5f);
            }

            const float worldPadding = 0.35f;
            minX -= worldPadding;
            maxX += worldPadding;
            minY -= worldPadding;
            maxY += worldPadding;
            float worldWidth = Mathf.Max(0.01f, maxX - minX);
            float worldHeight = Mathf.Max(0.01f, maxY - minY);
            float pixelsPerUnit = Mathf.Min(
                previewRect.width / worldWidth,
                (previewRect.height - 26f) / worldHeight);
            Vector2 worldCentre = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            Vector2 previewCentre = previewRect.center + Vector2.up * 10f;

            Vector2 Map(Vector2 world)
            {
                return new Vector2(
                    previewCentre.x + (world.x - worldCentre.x) * pixelsPerUnit,
                    previewCentre.y - (world.y - worldCentre.y) * pixelsPerUnit);
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

            if (showVisualBounds)
            {
                DrawOutline(imageRect, new Color(0.25f, 0.85f, 1f, 1f));
            }

            if (showColliderBounds)
            {
                Vector2 colliderTopLeft = Map(new Vector2(colliderMin.x, colliderMax.y));
                Vector2 colliderBottomRight = Map(new Vector2(colliderMax.x, colliderMin.y));
                Rect colliderRect = Rect.MinMaxRect(
                    colliderTopLeft.x,
                    colliderTopLeft.y,
                    colliderBottomRight.x,
                    colliderBottomRight.y);
                EditorGUI.DrawRect(colliderRect, new Color(1f, 0.65f, 0.15f, 0.14f));
                DrawOutline(colliderRect, new Color(1f, 0.65f, 0.15f, 1f));

                Vector2 centrePoint = Map(colliderCentre);
                EditorGUI.DrawRect(new Rect(centrePoint.x - 5f, centrePoint.y - 1f, 10f, 2f), new Color(1f, 0.65f, 0.15f));
                EditorGUI.DrawRect(new Rect(centrePoint.x - 1f, centrePoint.y - 5f, 2f, 10f), new Color(1f, 0.65f, 0.15f));
            }

            if (showFootprintOverlay)
            {
                foreach (Vector2Int offset in occupied)
                {
                    Vector2 centre = anchorOffset + (Vector2)offset;
                    Vector2 topLeft = Map(centre + new Vector2(-0.5f, 0.5f));
                    Vector2 bottomRight = Map(centre + new Vector2(0.5f, -0.5f));
                    Rect cellRect = Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
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
            }

            Vector2 anchorPoint = Map(anchorOffset);
            EditorGUI.DrawRect(new Rect(anchorPoint.x - 5f, anchorPoint.y - 1f, 10f, 2f), Color.cyan);
            EditorGUI.DrawRect(new Rect(anchorPoint.x - 1f, anchorPoint.y - 5f, 2f, 10f), Color.cyan);

            GUI.Label(
                new Rect(previewRect.x + 6f, previewRect.y + 5f, 250f, 18f),
                "Building authoring layers",
                EditorStyles.miniLabel);
            DrawLegend(previewRect);
        }

        private void DrawLegend(Rect previewRect)
        {
            float y = previewRect.yMax - 20f;
            float x = previewRect.x + 8f;
            if (showVisualBounds) DrawLegendItem(ref x, y, new Color(0.25f, 0.85f, 1f, 1f), "Visual");
            if (showColliderBounds) DrawLegendItem(ref x, y, new Color(1f, 0.65f, 0.15f, 1f), "Collider");
            if (showFootprintOverlay) DrawLegendItem(ref x, y, new Color(0.25f, 1f, 0.4f, 1f), "Footprint");
        }

        private static void DrawLegendItem(ref float x, float y, Color color, string label)
        {
            EditorGUI.DrawRect(new Rect(x, y + 2f, 11f, 11f), color);
            GUI.Label(new Rect(x + 15f, y, 72f, 18f), label, EditorStyles.miniLabel);
            x += 88f;
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
            int minX = GridBuildingFootprint.GetCanvasMinimumX(size.x);
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
            colliderSize = SanitizeColliderSize(colliderSize);
            Undo.RecordObject(definition, "Edit building authoring");
            definition.SetFootprint(size, occupied);
            definition.SetFootprintAnchor(anchorOffset);
            definition.SetColliderAuthoring(colliderCentre, colliderSize);
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
            colliderSize = SanitizeColliderSize(definition.ColliderSize);
            colliderCentre = CozyFarmBuildingPrefabGenerator.ToPrefabBaseSpace(
                definition.ColliderOffset,
                definition.Baseline);
        }

        private IEnumerable<Vector2Int> AllCanvasCells()
        {
            return GridBuildingFootprint.CreateRectangleOffsets(size);
        }

        private void RemoveCellsOutsideCanvas()
        {
            var allowed = new HashSet<Vector2Int>(AllCanvasCells());
            occupied.RemoveWhere(cell => !allowed.Contains(cell));
            occupied.Add(Vector2Int.zero);
        }

        private static Vector2 SanitizeColliderSize(Vector2 value)
        {
            return new Vector2(
                Mathf.Max(0.05f, Mathf.Abs(value.x)),
                Mathf.Max(0.05f, Mathf.Abs(value.y)));
        }
    }
}
