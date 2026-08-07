using System;
using System.Collections.Generic;
using FarmSimulator.Domain.Items;

namespace FarmSimulator.Presentation.Inventory
{
    public static class InventoryHotbarAssetCatalog
    {
        public const string AssetRoot =
            "Assets/_Project/Resources/Prefabs/UI";
        public const string PrefabAssetPath =
            AssetRoot + "/InventoryHotbar.prefab";
        public const string ResourcePath =
            "Prefabs/UI/InventoryHotbar";
        public const string ImportSignature =
            "inventory-hotbar-prefab-v2-first-party";

        public const string RootObjectName = "Inventory Hotbar";
        public const string PanelObjectName = "Hotbar Panel";
        public const string SlotsObjectName = "Slots";
        public const string SelectedItemObjectName = "Selected Item";
        public const string SlotNamePrefix = "Slot ";

        public const int SlotCount = 8;
        public const float SlotSize = 46f;
        public const float SlotGap = 6f;
        public const float BottomMargin = 18f;
        public const float PanelHeight = 58f;

        private const string CropRoot =
            "Assets/_Project/Art/Placeholder/Crops";

        private static readonly IReadOnlyDictionary<ItemId, string>
            SpritePaths = new Dictionary<ItemId, string>
            {
                { ItemId.TurnipSeeds, CropRoot + "/turnip_stage_0.png" },
                { ItemId.Turnip, CropRoot + "/turnip_stage_4.png" },
                { ItemId.PotatoSeeds, CropRoot + "/potato_stage_0.png" },
                { ItemId.Potato, CropRoot + "/potato_stage_5.png" },
                { ItemId.RadishSeeds, CropRoot + "/radish_stage_0.png" },
                { ItemId.Radish, CropRoot + "/radish_stage_4.png" }
            };

        public static IReadOnlyDictionary<ItemId, string>
            AvailableSpritePaths => SpritePaths;

        public static string SlotObjectName(int index)
        {
            if (index < 0 || index >= SlotCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index),
                    index,
                    "Hotbar slot index is outside the supported range.");
            }

            return SlotNamePrefix + (index + 1);
        }
    }
}
