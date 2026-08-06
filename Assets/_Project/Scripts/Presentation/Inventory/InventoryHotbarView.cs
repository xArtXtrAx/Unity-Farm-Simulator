using System;
using System.Collections.Generic;
using FarmSimulator.Application.Inventory;
using FarmSimulator.Domain.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FarmSimulator.Presentation.Inventory
{
    [Serializable]
    public sealed class InventoryHotbarSlotView
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private Text numberText;
        [SerializeField] private Text placeholderText;
        [SerializeField] private Text quantityText;

        public RectTransform Root => root;
        public Image Background => background;
        public Image Icon => icon;
        public Text NumberText => numberText;
        public Text PlaceholderText => placeholderText;
        public Text QuantityText => quantityText;

        public void ConfigureReferences(
            RectTransform slotRoot,
            Image slotBackground,
            Image slotIcon,
            Text slotNumber,
            Text slotPlaceholder,
            Text slotQuantity)
        {
            root = slotRoot ?? throw new ArgumentNullException(nameof(slotRoot));
            background = slotBackground ?? throw new ArgumentNullException(nameof(slotBackground));
            icon = slotIcon ?? throw new ArgumentNullException(nameof(slotIcon));
            numberText = slotNumber ?? throw new ArgumentNullException(nameof(slotNumber));
            placeholderText = slotPlaceholder ?? throw new ArgumentNullException(nameof(slotPlaceholder));
            quantityText = slotQuantity ?? throw new ArgumentNullException(nameof(slotQuantity));
        }

        public void Render(
            HotbarSlotPresentation presentation,
            Sprite itemSprite,
            Color normalFill,
            Color selectedFill,
            Color normalNumber,
            Color selectedNumber)
        {
            background.color = presentation.Selected ? selectedFill : normalFill;
            numberText.color = presentation.Selected ? selectedNumber : normalNumber;
            bool hasSprite = itemSprite != null;
            icon.enabled = hasSprite;
            icon.sprite = itemSprite;
            placeholderText.gameObject.SetActive(!presentation.IsEmpty && !hasSprite);
            placeholderText.text = hasSprite ? string.Empty : presentation.ShortLabel;
            quantityText.text = presentation.Quantity > 1
                ? $"×{presentation.Quantity}"
                : string.Empty;
        }
    }

    [DisallowMultipleComponent]
    public sealed class InventoryHotbarView : MonoBehaviour
    {
        public static readonly Color NormalSlotColor = new Color32(38, 49, 38, 240);
        public static readonly Color SelectedSlotColor = new Color32(86, 99, 61, 255);
        public static readonly Color NormalNumberColor = new Color32(216, 207, 170, 255);
        public static readonly Color SelectedNumberColor = new Color32(255, 242, 168, 255);

        private const float CycleInputDebounceSeconds = 0.12f;
        private static int lastConsumedCycleFrame = -1;
        private static float lastConsumedCycleTime = float.NegativeInfinity;

        [SerializeField] private Text selectedItemText;
        [SerializeField] private InventoryHotbarSlotView[] slots;
        [SerializeField] private string[] iconItemIds;
        [SerializeField] private Sprite[] iconSprites;

        private readonly Dictionary<string, Sprite> iconsByItemId =
            new Dictionary<string, Sprite>(StringComparer.Ordinal);
        private HotbarPresentationModel model;

        public int SlotCount => slots?.Length ?? 0;
        public int SelectedIndex => model?.SelectedIndex ?? -1;
        public string SelectedItemName => model?.SelectedItemName ?? string.Empty;
        public bool IsInitialized => model != null;
        public Text SelectedItemText => selectedItemText;
        public IReadOnlyList<InventoryHotbarSlotView> Slots => slots;
        public IReadOnlyList<string> IconItemIds => iconItemIds;
        public IReadOnlyList<Sprite> IconSprites => iconSprites;

        private void Awake()
        {
            BuildIconLookup();
            if (model == null)
            {
                Initialize(InventoryState.CreateInitialPlayerInventory());
            }
        }

        private void Update()
        {
            int directIndex = ReadDirectSlotIndex();
            if (directIndex >= 0)
            {
                SelectSlot(directIndex);
                return;
            }

            int cycle = ReadCycleInput();
            if (cycle != 0 && TryConsumeCycleInput())
            {
                CycleSelection(cycle);
            }
        }

        public void ConfigureGeneratedReferences(
            Text selectedLabel,
            InventoryHotbarSlotView[] slotViews,
            string[] mappedItemIds,
            Sprite[] mappedSprites)
        {
            selectedItemText = selectedLabel ?? throw new ArgumentNullException(nameof(selectedLabel));
            slots = slotViews ?? throw new ArgumentNullException(nameof(slotViews));
            iconItemIds = mappedItemIds ?? throw new ArgumentNullException(nameof(mappedItemIds));
            iconSprites = mappedSprites ?? throw new ArgumentNullException(nameof(mappedSprites));

            if (slots.Length != InventoryHotbarAssetCatalog.SlotCount)
            {
                throw new ArgumentException(
                    $"Hotbar must contain exactly {InventoryHotbarAssetCatalog.SlotCount} slots.",
                    nameof(slotViews));
            }

            if (iconItemIds.Length != iconSprites.Length)
            {
                throw new ArgumentException(
                    "Icon item IDs and sprites must have matching lengths.");
            }

            BuildIconLookup();
        }

        public void Initialize(InventoryState inventoryState)
        {
            BuildIconLookup();
            model = new HotbarPresentationModel(
                inventoryState ?? throw new ArgumentNullException(nameof(inventoryState)));
            Refresh();
        }

        public bool SelectSlot(int index)
        {
            EnsureInitialized();
            bool changed = model.SelectSlot(index);
            if (changed) Refresh();
            return changed;
        }

        public bool CycleSelection(int delta)
        {
            EnsureInitialized();
            bool changed = model.CycleSelection(delta);
            if (changed) Refresh();
            return changed;
        }

        public bool TryRefresh()
        {
            if (!IsInitialized || !ReferencesAreReady())
            {
                return false;
            }

            Refresh();
            return true;
        }

        public void Refresh()
        {
            EnsureReferences();
            EnsureInitialized();

            IReadOnlyList<HotbarSlotPresentation> snapshot = model.Snapshot();
            for (int index = 0; index < snapshot.Count; index++)
            {
                HotbarSlotPresentation presentation = snapshot[index];
                Sprite sprite = null;
                if (presentation.ItemId != null)
                {
                    iconsByItemId.TryGetValue(presentation.ItemId, out sprite);
                }

                slots[index].Render(
                    presentation,
                    sprite,
                    NormalSlotColor,
                    SelectedSlotColor,
                    NormalNumberColor,
                    SelectedNumberColor);
            }

            selectedItemText.text =
                $"{model.SelectedItemName}  ·  1–8 / rueda / L1–R1";
        }

        public InventoryHotbarSlotView GetSlotView(int index)
        {
            if (index < 0 || index >= SlotCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return slots[index];
        }

        public bool TryGetMappedIcon(string itemId, out Sprite sprite)
        {
            if (itemId == null)
            {
                sprite = null;
                return false;
            }
            return iconsByItemId.TryGetValue(itemId, out sprite) && sprite != null;
        }

        private void BuildIconLookup()
        {
            iconsByItemId.Clear();
            if (iconItemIds == null || iconSprites == null) return;
            int count = Mathf.Min(iconItemIds.Length, iconSprites.Length);
            for (int index = 0; index < count; index++)
            {
                string itemId = iconItemIds[index];
                Sprite sprite = iconSprites[index];
                if (string.IsNullOrEmpty(itemId) || sprite == null) continue;
                iconsByItemId[itemId] = sprite;
            }
        }

        private static int ReadDirectSlotIndex()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return -1;
            if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame) return 0;
            if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame) return 1;
            if (keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame) return 2;
            if (keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame) return 3;
            if (keyboard.digit5Key.wasPressedThisFrame || keyboard.numpad5Key.wasPressedThisFrame) return 4;
            if (keyboard.digit6Key.wasPressedThisFrame || keyboard.numpad6Key.wasPressedThisFrame) return 5;
            if (keyboard.digit7Key.wasPressedThisFrame || keyboard.numpad7Key.wasPressedThisFrame) return 6;
            if (keyboard.digit8Key.wasPressedThisFrame || keyboard.numpad8Key.wasPressedThisFrame) return 7;
            return -1;
        }

        private static int ReadCycleInput()
        {
            Gamepad gamepad = Gamepad.current;
            if (gamepad == null && Gamepad.all.Count > 0) gamepad = Gamepad.all[0];
            if (gamepad != null)
            {
                if (gamepad.leftShoulder.wasPressedThisFrame) return -1;
                if (gamepad.rightShoulder.wasPressedThisFrame) return 1;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null) return 0;
            float scroll = mouse.scroll.ReadValue().y;
            if (scroll > 0.01f) return -1;
            return scroll < -0.01f ? 1 : 0;
        }

        private static bool TryConsumeCycleInput()
        {
            int frame = Time.frameCount;
            float now = Time.unscaledTime;
            if (lastConsumedCycleFrame == frame ||
                now - lastConsumedCycleTime < CycleInputDebounceSeconds)
            {
                return false;
            }

            lastConsumedCycleFrame = frame;
            lastConsumedCycleTime = now;
            return true;
        }

        private void EnsureInitialized()
        {
            if (model == null)
            {
                throw new InvalidOperationException(
                    "Inventory hotbar has not been initialized.");
            }
        }

        private bool ReferencesAreReady()
        {
            return selectedItemText != null &&
                slots != null &&
                slots.Length == InventoryHotbarAssetCatalog.SlotCount;
        }

        private void EnsureReferences()
        {
            if (!ReferencesAreReady())
            {
                throw new InvalidOperationException(
                    "Inventory hotbar prefab references are incomplete.");
            }
        }
    }
}
