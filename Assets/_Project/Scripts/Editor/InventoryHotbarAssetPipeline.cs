using System;
using System.Collections.Generic;
using System.Linq;
using FarmSimulator.Domain.Items;
using FarmSimulator.Presentation.Inventory;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class InventoryHotbarAssetPipeline
    {
        private static readonly ItemId[] IconItemOrder =
        {
            ItemId.TurnipSeeds,
            ItemId.Turnip,
            ItemId.CarrotSeeds,
            ItemId.Carrot,
            ItemId.CabbageSeeds,
            ItemId.Cabbage
        };

        static InventoryHotbarAssetPipeline()
        {
            EditorApplication.delayCall += EnsureAssets;
        }

        [MenuItem("Tools/Farm Simulator/Rebuild Inventory Hotbar")]
        public static void RebuildAssets()
        {
            AssetDatabase.DeleteAsset(
                InventoryHotbarAssetCatalog.PrefabAssetPath);
            EditorApplication.delayCall += EnsureAssets;
        }

        public static void EnsureAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureAssets;
                return;
            }

            if (!TryLoadIcons(
                    out string[] itemIds,
                    out Sprite[] sprites))
            {
                EditorApplication.delayCall += EnsureAssets;
                return;
            }

            if (IsPrefabCurrent())
            {
                return;
            }

            CreateOrReplacePrefab(itemIds, sprites);
        }

        private static bool TryLoadIcons(
            out string[] itemIds,
            out Sprite[] sprites)
        {
            var available = new Dictionary<string, Sprite>(
                StringComparer.Ordinal);

            AddSprites(
                InventoryHotbarAssetCatalog.ItemsSpriteSheetPath,
                available);
            AddSprites(
                InventoryHotbarAssetCatalog.SeedsSpriteSheetPath,
                available);

            itemIds = new string[IconItemOrder.Length];
            sprites = new Sprite[IconItemOrder.Length];

            for (int index = 0; index < IconItemOrder.Length; index++)
            {
                ItemId itemId = IconItemOrder[index];
                string spriteName =
                    InventoryHotbarAssetCatalog.AvailableSpriteNames[itemId];
                if (!available.TryGetValue(spriteName, out Sprite sprite))
                {
                    return false;
                }

                itemIds[index] = itemId.Value;
                sprites[index] = sprite;
            }

            return true;
        }

        private static void AddSprites(
            string assetPath,
            IDictionary<string, Sprite> destination)
        {
            foreach (Sprite sprite in AssetDatabase
                         .LoadAllAssetRepresentationsAtPath(assetPath)
                         .OfType<Sprite>())
            {
                destination[sprite.name] = sprite;
            }
        }

        private static bool IsPrefabCurrent()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                InventoryHotbarAssetCatalog.PrefabAssetPath);
            AssetImporter importer = AssetImporter.GetAtPath(
                InventoryHotbarAssetCatalog.PrefabAssetPath);

            if (prefab == null || importer == null ||
                importer.userData !=
                InventoryHotbarAssetCatalog.ImportSignature)
            {
                return false;
            }

            InventoryHotbarView view =
                prefab.GetComponent<InventoryHotbarView>();
            Canvas canvas = prefab.GetComponent<Canvas>();
            CanvasScaler scaler = prefab.GetComponent<CanvasScaler>();

            return view != null &&
                canvas != null &&
                canvas.renderMode == RenderMode.ScreenSpaceOverlay &&
                scaler != null &&
                scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize &&
                scaler.referenceResolution == new Vector2(960f, 540f) &&
                view.SlotCount == InventoryHotbarAssetCatalog.SlotCount &&
                view.IconItemIds.Count == IconItemOrder.Length &&
                view.IconSprites.Count == IconItemOrder.Length;
        }

        private static void CreateOrReplacePrefab(
            string[] itemIds,
            Sprite[] sprites)
        {
            EnsureFolder(InventoryHotbarAssetCatalog.AssetRoot);
            AssetDatabase.DeleteAsset(
                InventoryHotbarAssetCatalog.PrefabAssetPath);

            Font font = Resources.GetBuiltinResource<Font>(
                "LegacyRuntime.ttf");
            if (font == null)
            {
                throw new InvalidOperationException(
                    "Unity built-in LegacyRuntime.ttf font is unavailable.");
            }

            var root = new GameObject(
                InventoryHotbarAssetCatalog.RootObjectName,
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(InventoryHotbarView));

            try
            {
                Canvas canvas = root.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1000;

                CanvasScaler scaler = root.GetComponent<CanvasScaler>();
                scaler.uiScaleMode =
                    CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(960f, 540f);
                scaler.screenMatchMode =
                    CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
                scaler.referencePixelsPerUnit = 100f;

                RectTransform panel = CreateRect(
                    InventoryHotbarAssetCatalog.PanelObjectName,
                    root.transform);
                panel.anchorMin = new Vector2(0.5f, 0f);
                panel.anchorMax = new Vector2(0.5f, 0f);
                panel.pivot = new Vector2(0.5f, 0f);
                panel.anchoredPosition = new Vector2(
                    0f,
                    InventoryHotbarAssetCatalog.BottomMargin);

                float totalWidth =
                    InventoryHotbarAssetCatalog.SlotCount *
                    InventoryHotbarAssetCatalog.SlotSize +
                    (InventoryHotbarAssetCatalog.SlotCount - 1) *
                    InventoryHotbarAssetCatalog.SlotGap;
                panel.sizeDelta = new Vector2(
                    totalWidth,
                    InventoryHotbarAssetCatalog.PanelHeight);

                Text selectedItemText = CreateText(
                    InventoryHotbarAssetCatalog.SelectedItemObjectName,
                    panel,
                    font,
                    fontSize: 14,
                    TextAnchor.MiddleCenter,
                    new Color32(255, 241, 199, 255));
                RectTransform selectedRect =
                    selectedItemText.rectTransform;
                selectedRect.anchorMin = new Vector2(0.5f, 1f);
                selectedRect.anchorMax = new Vector2(0.5f, 1f);
                selectedRect.pivot = new Vector2(0.5f, 0f);
                selectedRect.anchoredPosition = new Vector2(0f, 6f);
                selectedRect.sizeDelta = new Vector2(totalWidth, 24f);

                RectTransform slotsRoot = CreateRect(
                    InventoryHotbarAssetCatalog.SlotsObjectName,
                    panel);
                Stretch(slotsRoot);

                var slotViews = new InventoryHotbarSlotView[
                    InventoryHotbarAssetCatalog.SlotCount];
                float startX = -totalWidth * 0.5f +
                    InventoryHotbarAssetCatalog.SlotSize * 0.5f;

                for (int index = 0;
                     index < InventoryHotbarAssetCatalog.SlotCount;
                     index++)
                {
                    slotViews[index] = CreateSlot(
                        slotsRoot,
                        font,
                        index,
                        startX + index *
                        (InventoryHotbarAssetCatalog.SlotSize +
                         InventoryHotbarAssetCatalog.SlotGap));
                }

                InventoryHotbarView view =
                    root.GetComponent<InventoryHotbarView>();
                view.ConfigureGeneratedReferences(
                    selectedItemText,
                    slotViews,
                    itemIds,
                    sprites);

                PrefabUtility.SaveAsPrefabAsset(
                    root,
                    InventoryHotbarAssetCatalog.PrefabAssetPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
            }

            AssetImporter importer = AssetImporter.GetAtPath(
                InventoryHotbarAssetCatalog.PrefabAssetPath);
            if (importer != null)
            {
                importer.userData =
                    InventoryHotbarAssetCatalog.ImportSignature;
                importer.SaveAndReimport();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Generated inventory hotbar prefab with eight slots and curated Cozy Farm icons.");
        }

        private static InventoryHotbarSlotView CreateSlot(
            RectTransform parent,
            Font font,
            int index,
            float x)
        {
            RectTransform root = CreateRect(
                InventoryHotbarAssetCatalog.SlotObjectName(index),
                parent);
            root.anchorMin = new Vector2(0.5f, 0.5f);
            root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.anchoredPosition = new Vector2(x, 0f);
            root.sizeDelta = Vector2.one *
                InventoryHotbarAssetCatalog.SlotSize;

            Image background = root.gameObject.AddComponent<Image>();
            background.color = InventoryHotbarView.NormalSlotColor;
            background.raycastTarget = false;

            Image icon = CreateImage("Icon", root);
            RectTransform iconRect = icon.rectTransform;
            iconRect.anchorMin = new Vector2(0.5f, 0.5f);
            iconRect.anchorMax = new Vector2(0.5f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(30f, 30f);
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            Text numberText = CreateText(
                "Number",
                root,
                font,
                fontSize: 10,
                TextAnchor.UpperLeft,
                InventoryHotbarView.NormalNumberColor);
            numberText.text = (index + 1).ToString();
            RectTransform numberRect = numberText.rectTransform;
            numberRect.anchorMin = Vector2.zero;
            numberRect.anchorMax = Vector2.one;
            numberRect.offsetMin = new Vector2(4f, 2f);
            numberRect.offsetMax = new Vector2(-4f, -2f);

            Text placeholderText = CreateText(
                "Placeholder",
                root,
                font,
                fontSize: 16,
                TextAnchor.MiddleCenter,
                new Color32(255, 244, 210, 255));
            placeholderText.fontStyle = FontStyle.Bold;
            Stretch(placeholderText.rectTransform);

            Text quantityText = CreateText(
                "Quantity",
                root,
                font,
                fontSize: 11,
                TextAnchor.LowerRight,
                Color.white);
            RectTransform quantityRect = quantityText.rectTransform;
            quantityRect.anchorMin = Vector2.zero;
            quantityRect.anchorMax = Vector2.one;
            quantityRect.offsetMin = new Vector2(3f, 2f);
            quantityRect.offsetMax = new Vector2(-3f, -2f);

            var slotView = new InventoryHotbarSlotView();
            slotView.ConfigureReferences(
                root,
                background,
                icon,
                numberText,
                placeholderText,
                quantityText);
            return slotView;
        }

        private static RectTransform CreateRect(
            string name,
            Transform parent)
        {
            var gameObject = new GameObject(
                name,
                typeof(RectTransform));
            RectTransform rect =
                gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, worldPositionStays: false);
            return rect;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            Font font,
            int fontSize,
            TextAnchor alignment,
            Color color)
        {
            RectTransform rect = CreateRect(name, parent);
            Text text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static Image CreateImage(
            string name,
            Transform parent)
        {
            RectTransform rect = CreateRect(name, parent);
            return rect.gameObject.AddComponent<Image>();
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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
    }
}
