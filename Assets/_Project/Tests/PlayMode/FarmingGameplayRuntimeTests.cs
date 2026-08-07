using System.Collections;
using FarmSimulator.Domain.Inventory;
using FarmSimulator.Domain.Items;
using FarmSimulator.Presentation.Farming;
using FarmSimulator.Presentation.Interaction;
using FarmSimulator.Presentation.Player;
using FarmSimulator.Presentation.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace FarmSimulator.Tests.PlayMode
{
    public sealed class FarmingGameplayRuntimeTests
    {
        [UnityTest]
        public IEnumerator TurnipLoopRunsFromHotbarThroughDayAdvanceAndHarvest()
        {
            GameSessionRuntime session = GameSessionRuntime.Instance;
            session.ResetSession();

            GameObject player = new GameObject("Farming Test Player");
            GameObject plotObject = null;
            Texture2D texture = null;
            try
            {
                FarmPlotBehaviour plot = CreatePlot("runtime-loop-plot", new Vector3(0.5f, -0.5f), out plotObject, out texture);
                InventoryState inventory = session.Inventory;

                inventory.SelectSlot(0);
                plot.Interact(player);
                Assert.That(plot.PlotState.IsTilled, Is.True);

                inventory.SelectSlot(1);
                plot.Interact(player);
                Assert.That(plot.PlotState.IsWatered, Is.True);

                inventory.SelectSlot(2);
                int seedsBefore = inventory.GetSelectedSlot().Quantity;
                plot.Interact(player);
                Assert.That(plot.PlotState.SeedItemId, Is.EqualTo(ItemId.TurnipSeeds));
                Assert.That(inventory.GetSelectedSlot().Quantity, Is.EqualTo(seedsBefore - 1));
                Assert.That(plot.CropRenderer.enabled, Is.True);

                session.AdvanceDay();
                Assert.That(plot.PlotState.GrowthDays, Is.EqualTo(1));
                Assert.That(plot.PlotState.IsWatered, Is.False);

                inventory.SelectSlot(1);
                plot.Interact(player);
                session.AdvanceDay();
                Assert.That(plot.PlotState.IsMature, Is.True);
                Assert.That(plot.PlotState.VisualStage, Is.EqualTo(4));

                plot.Interact(player);
                Assert.That(plot.PlotState.HasCrop, Is.False);
                Assert.That(CountItem(inventory, ItemId.Turnip), Is.EqualTo(1));
                Assert.That(plot.CropRenderer.enabled, Is.False);

                yield return null;
            }
            finally
            {
                if (plotObject != null) Object.DestroyImmediate(plotObject);
                Object.DestroyImmediate(player);
                if (texture != null) Object.DestroyImmediate(texture);
                session.ResetSession();
            }
        }

        [UnityTest]
        public IEnumerator InteractionControllerSelectsOnlyFarmPlotInFrontCell()
        {
            GameSessionRuntime.Instance.ResetSession();
            GameObject player = new GameObject("Grid Interaction Player");
            GameObject frontObject = null;
            GameObject sideObject = null;
            Texture2D frontTexture = null;
            Texture2D sideTexture = null;
            try
            {
                player.transform.position = new Vector3(0.5f, 0.5f, 0f);
                player.AddComponent<Rigidbody2D>();
                player.AddComponent<CapsuleCollider2D>();
                TopDownPlayerMotor motor = player.AddComponent<TopDownPlayerMotor>();
                motor.SetDesiredInput(Vector2.down);
                PlayerInteractionController controller = player.AddComponent<PlayerInteractionController>();

                FarmPlotBehaviour front = CreatePlot("front-cell", new Vector3(0.5f, -0.5f), out frontObject, out frontTexture);
                CreatePlot("side-cell", new Vector3(1.5f, 0.5f), out sideObject, out sideTexture);

                controller.RefreshSelection();
                Assert.That(controller.SelectedInteractable, Is.SameAs(front));

                yield return null;
            }
            finally
            {
                if (frontObject != null) Object.DestroyImmediate(frontObject);
                if (sideObject != null) Object.DestroyImmediate(sideObject);
                Object.DestroyImmediate(player);
                if (frontTexture != null) Object.DestroyImmediate(frontTexture);
                if (sideTexture != null) Object.DestroyImmediate(sideTexture);
                GameSessionRuntime.Instance.ResetSession();
            }
        }

        private static FarmPlotBehaviour CreatePlot(
            string id,
            Vector3 position,
            out GameObject plotObject,
            out Texture2D texture)
        {
            plotObject = new GameObject(id);
            plotObject.transform.position = position;

            var soilObject = new GameObject("Soil Visual");
            soilObject.transform.SetParent(plotObject.transform, false);
            SpriteRenderer soil = soilObject.AddComponent<SpriteRenderer>();

            var cropObject = new GameObject("Crop Entity Visual");
            cropObject.transform.SetParent(plotObject.transform, false);
            SpriteRenderer crop = cropObject.AddComponent<SpriteRenderer>();

            texture = new Texture2D(16, 16);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);

            FarmPlotBehaviour behaviour = plotObject.AddComponent<FarmPlotBehaviour>();
            behaviour.Configure(
                id,
                soil,
                crop,
                sprite,
                sprite,
                Repeat(sprite, 5),
                Repeat(sprite, 6),
                Repeat(sprite, 5));
            return behaviour;
        }

        private static Sprite[] Repeat(Sprite sprite, int count)
        {
            var sprites = new Sprite[count];
            for (int index = 0; index < count; index++) sprites[index] = sprite;
            return sprites;
        }

        private static int CountItem(InventoryState inventory, ItemId itemId)
        {
            int total = 0;
            foreach (InventorySlot slot in inventory.GetSlots())
            {
                if (!slot.IsEmpty && slot.ItemId.Value == itemId)
                    total += slot.Quantity;
            }
            return total;
        }
    }
}
