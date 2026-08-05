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
            "inventory-hotbar-prefab-v1";

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

        public const string ItemsSpriteSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/items.png";
        public const string SeedsSpriteSheetPath =
            "Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/seeds.png";

        private static readonly IReadOnlyDictionary<ItemId, string>
            SpriteNames = new Dictionary<ItemId, string>
            {
                { ItemId.TurnipSeeds, "cozy_turnip_seeds" },
                { ItemId.Turnip, "cozy_turnip" },
                { ItemId.CarrotSeeds, "cozy_carrot_seeds" },
                { ItemId.Carrot, "cozy_carrot" },
                { ItemId.CabbageSeeds, "cozy_cabbage_seeds" },
                { ItemId.Cabbage, "cozy_cabbage" }
            };

        public static IReadOnlyDictionary<ItemId, string>
            AvailableSpriteNames => SpriteNames;

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
