using System;
using System.Linq;
using FarmSimulator.Domain.Inventory;
using FarmSimulator.Domain.Items;
using FarmSimulator.Editor;
using FarmSimulator.Presentation.Inventory;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class InventoryHotbarAssetPipelineTests
    {
        [OneTimeSetUp]
        public void EnsurePrefabExists()
        {
            InventoryHotbarAssetPipeline.EnsureAssets();
        }

        [Test]
        public void HotbarPrefabIsGeneratedAndSigned()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                InventoryHotbarAssetCatalog.PrefabAssetPath);
            AssetImporter importer = AssetImporter.GetAtPath(
                InventoryHotbarAssetCatalog.PrefabAssetPath);

            Assert.That(prefab, Is.Not.Null);
            Assert.That(importer, Is.Not.Null);
            Assert.That(
                importer.userData,
                Is.EqualTo(InventoryHotbarAssetCatalog.ImportSignature));
        }

        [Test]
        public void HotbarUsesReferenceResolutionAndBottomCenterAnchor()
        {
            GameObject prefab = LoadPrefab();
            Canvas canvas = prefab.GetComponent<Canvas>();
            CanvasScaler scaler = prefab.GetComponent<CanvasScaler>();
            RectTransform panel = prefab.transform.Find(
                InventoryHotbarAssetCatalog.PanelObjectName)
                as RectTransform;

            Assert.That(canvas, Is.Not.Null);
            Assert.That(
                canvas.renderMode,
                Is.EqualTo(RenderMode.ScreenSpaceOverlay));
            Assert.That(canvas.sortingOrder, Is.EqualTo(1000));
            Assert.That(scaler, Is.Not.Null);
            Assert.That(
                scaler.uiScaleMode,
                Is.EqualTo(CanvasScaler.ScaleMode.ScaleWithScreenSize));
            Assert.That(
                scaler.referenceResolution,
                Is.EqualTo(new Vector2(960f, 540f)));
            Assert.That(panel, Is.Not.Null);
            Assert.That(panel.anchorMin, Is.EqualTo(new Vector2(0.5f, 0f)));
            Assert.That(panel.anchorMax, Is.EqualTo(new Vector2(0.5f, 0f)));
        }

        [Test]
        public void HotbarContainsEightNumberedSlots()
        {
            InventoryHotbarView view =
                LoadPrefab().GetComponent<InventoryHotbarView>();

            Assert.That(view, Is.Not.Null);
            Assert.That(
                view.SlotCount,
                Is.EqualTo(InventoryHotbarAssetCatalog.SlotCount));

            for (int index = 0; index < view.SlotCount; index++)
            {
                InventoryHotbarSlotView slot = view.GetSlotView(index);
                Assert.That(slot.Root, Is.Not.Null);
                Assert.That(
                    slot.Root.name,
                    Is.EqualTo(
                        InventoryHotbarAssetCatalog.SlotObjectName(index)));
                Assert.That(slot.NumberText.text, Is.EqualTo((index + 1).ToString()));
                Assert.That(slot.Background, Is.Not.Null);
                Assert.That(slot.Icon, Is.Not.Null);
                Assert.That(slot.PlaceholderText, Is.Not.Null);
                Assert.That(slot.QuantityText, Is.Not.Null);
            }
        }

        [Test]
        public void HotbarMapsOnlyDefinitiveFirstPartyCropIcons()
        {
            InventoryHotbarView view =
                LoadPrefab().GetComponent<InventoryHotbarView>();

            string[] expectedIds =
            {
                ItemId.TurnipSeeds.Value,
                ItemId.Turnip.Value,
                ItemId.PotatoSeeds.Value,
                ItemId.Potato.Value,
                ItemId.RadishSeeds.Value,
                ItemId.Radish.Value
            };

            CollectionAssert.AreEquivalent(expectedIds, view.IconItemIds);
            Assert.That(
                view.IconSprites.Count,
                Is.EqualTo(expectedIds.Length));
            Assert.That(view.IconSprites.All(sprite => sprite != null), Is.True);
            Assert.That(
                view.TryGetMappedIcon(ItemId.Hoe.Value, out _),
                Is.False);
            Assert.That(
                view.TryGetMappedIcon(ItemId.WateringCan.Value, out _),
                Is.False);
        }

        [Test]
        public void InitialInventoryRendersToolsAndDefinitiveSeedIcons()
        {
            GameObject instance = UnityEngine.Object.Instantiate(LoadPrefab());
            try
            {
                InventoryHotbarView view =
                    instance.GetComponent<InventoryHotbarView>();
                view.Initialize(
                    InventoryState.CreateInitialPlayerInventory());

                InventoryHotbarSlotView hoe = view.GetSlotView(0);
                InventoryHotbarSlotView wateringCan = view.GetSlotView(1);
                InventoryHotbarSlotView turnipSeeds = view.GetSlotView(2);
                InventoryHotbarSlotView potatoSeeds = view.GetSlotView(3);
                InventoryHotbarSlotView radishSeeds = view.GetSlotView(4);

                Assert.That(hoe.Icon.enabled, Is.False);
                Assert.That(hoe.PlaceholderText.gameObject.activeSelf, Is.True);
                Assert.That(hoe.PlaceholderText.text, Is.EqualTo("AZ"));
                Assert.That(wateringCan.Icon.enabled, Is.False);
                Assert.That(
                    wateringCan.PlaceholderText.text,
                    Is.EqualTo("RG"));

                Assert.That(turnipSeeds.Icon.enabled, Is.True);
                Assert.That(turnipSeeds.Icon.sprite.name, Is.EqualTo("turnip_stage_0"));
                Assert.That(turnipSeeds.QuantityText.text, Is.EqualTo("×20"));
                Assert.That(potatoSeeds.Icon.enabled, Is.True);
                Assert.That(potatoSeeds.Icon.sprite.name, Is.EqualTo("potato_stage_0"));
                Assert.That(radishSeeds.Icon.enabled, Is.True);
                Assert.That(radishSeeds.Icon.sprite.name, Is.EqualTo("radish_stage_0"));
                Assert.That(view.SelectedIndex, Is.Zero);
                Assert.That(
                    view.SelectedItemText.text,
                    Does.StartWith("Azada"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        [Test]
        public void SelectionRefreshesColorsAndSelectedItemLabel()
        {
            GameObject instance = UnityEngine.Object.Instantiate(LoadPrefab());
            try
            {
                InventoryHotbarView view =
                    instance.GetComponent<InventoryHotbarView>();
                view.Initialize(
                    InventoryState.CreateInitialPlayerInventory());

                bool changed = view.SelectSlot(2);

                Assert.That(changed, Is.True);
                Assert.That(view.SelectedIndex, Is.EqualTo(2));
                Assert.That(
                    view.GetSlotView(2).Background.color,
                    Is.EqualTo(InventoryHotbarView.SelectedSlotColor));
                Assert.That(
                    view.GetSlotView(0).Background.color,
                    Is.EqualTo(InventoryHotbarView.NormalSlotColor));
                Assert.That(
                    view.SelectedItemText.text,
                    Does.StartWith("Semillas de nabo"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static GameObject LoadPrefab()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                InventoryHotbarAssetCatalog.PrefabAssetPath);
            Assert.That(prefab, Is.Not.Null);
            return prefab;
        }
    }
}
