using System;
using FarmSimulator.Domain.Farming;
using FarmSimulator.Domain.Inventory;
using FarmSimulator.Domain.Items;
using FarmSimulator.Presentation.Interaction;
using FarmSimulator.Presentation.Inventory;
using FarmSimulator.Presentation.World;
using UnityEngine;

namespace FarmSimulator.Presentation.Farming
{
    [DisallowMultipleComponent]
    public sealed class FarmPlotBehaviour : InteractableBehaviour
    {
        public const float MaximumCropWidth = 0.72f;
        public const float MaximumCropHeight = 0.86f;
        public const float CropBaseline = 0f;

        [SerializeField] private string plotId;
        [SerializeField] private SpriteRenderer soilRenderer;
        [SerializeField] private SpriteRenderer cropRenderer;
        [SerializeField] private Sprite grassSprite;
        [SerializeField] private Sprite tilledSoilSprite;
        [SerializeField] private Sprite[] turnipStages;
        [SerializeField] private Sprite[] carrotStages;
        [SerializeField] private Sprite[] cabbageStages;

        public string PlotId => plotId;
        public FarmPlotState PlotState => State;
        public SpriteRenderer SoilRenderer => soilRenderer;
        public SpriteRenderer CropRenderer => cropRenderer;
        public override string InteractionPrompt => BuildPrompt();

        private FarmPlotState State =>
            GameSessionRuntime.Instance.Farm.GetOrCreatePlot(plotId);

        private InventoryState Inventory => GameSessionRuntime.Instance.Inventory;

        private void OnEnable()
        {
            GameSessionRuntime.Instance.DayChanged += HandleDayChanged;
            Render();
        }

        private void OnDisable()
        {
            if (GameSessionRuntime.TryGetExisting(out GameSessionRuntime session))
            {
                session.DayChanged -= HandleDayChanged;
            }
        }

        public void Configure(
            string identifier,
            SpriteRenderer plotSoilRenderer,
            SpriteRenderer plotCropRenderer,
            Sprite untilledSprite,
            Sprite tilledSprite,
            Sprite[] turnipGrowthStages,
            Sprite[] carrotGrowthStages,
            Sprite[] cabbageGrowthStages)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Plot id is required.", nameof(identifier));
            }

            plotId = identifier;
            soilRenderer = plotSoilRenderer ??
                throw new ArgumentNullException(nameof(plotSoilRenderer));
            cropRenderer = plotCropRenderer ??
                throw new ArgumentNullException(nameof(plotCropRenderer));
            grassSprite = untilledSprite ??
                throw new ArgumentNullException(nameof(untilledSprite));
            tilledSoilSprite = tilledSprite ??
                throw new ArgumentNullException(nameof(tilledSprite));
            turnipStages = ValidateStages(turnipGrowthStages, nameof(turnipGrowthStages));
            carrotStages = ValidateStages(carrotGrowthStages, nameof(carrotGrowthStages));
            cabbageStages = ValidateStages(cabbageGrowthStages, nameof(cabbageGrowthStages));
            ConfigureInteraction("Preparar parcela", interactionPriority: 5);
        }

        public override bool CanInteract(GameObject interactor) =>
            base.CanInteract(interactor) && !string.IsNullOrWhiteSpace(plotId);

        public override void Interact(GameObject interactor)
        {
            FarmPlotState state = State;
            if (state.IsMature)
            {
                Harvest(state);
                return;
            }

            InventorySlot selected = Inventory.GetSelectedSlot();
            if (selected.IsEmpty)
            {
                Debug.Log("[Farming] Select a tool or seeds.");
                return;
            }

            ItemId selectedItem = selected.ItemId.Value;
            if (selectedItem == ItemId.Hoe)
            {
                if (state.Till()) CommitVisualChange();
                return;
            }

            if (selectedItem == ItemId.WateringCan)
            {
                if (state.Water()) CommitVisualChange();
                return;
            }

            if (CropCatalog.TryGetBySeed(selectedItem, out _))
            {
                Plant(state, selectedItem);
                return;
            }

            Debug.Log($"[Farming] '{selectedItem.Value}' cannot be used on a plot.");
        }

        public void Render()
        {
            if (soilRenderer == null || cropRenderer == null ||
                grassSprite == null || tilledSoilSprite == null ||
                string.IsNullOrWhiteSpace(plotId))
            {
                return;
            }

            FarmPlotState state = State;
            soilRenderer.sprite = tilledSoilSprite;
            soilRenderer.enabled = state.IsTilled;
            soilRenderer.color = state.IsWatered
                ? new Color32(116, 92, 72, 255)
                : Color.white;

            if (!state.HasCrop)
            {
                cropRenderer.enabled = false;
                cropRenderer.sprite = null;
                return;
            }

            Sprite[] stages = StagesFor(state.SeedItemId.Value);
            int index = Mathf.Clamp(state.VisualStage, 0, stages.Length - 1);
            cropRenderer.sprite = stages[index];
            cropRenderer.color = Color.white;
            cropRenderer.enabled = cropRenderer.sprite != null;
            NormalizeCropVisual(cropRenderer.sprite);
        }

        private void NormalizeCropVisual(Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            Vector2 size = sprite.bounds.size;
            float scale = size.x <= Mathf.Epsilon || size.y <= Mathf.Epsilon
                ? 1f
                : Mathf.Min(
                    MaximumCropWidth / size.x,
                    MaximumCropHeight / size.y);
            cropRenderer.transform.localScale = new Vector3(scale, scale, 1f);
            cropRenderer.transform.localPosition = new Vector3(0f, CropBaseline, 0f);
        }

        private void Plant(FarmPlotState state, ItemId seedItemId)
        {
            if (!state.Plant(seedItemId)) return;
            if (!Inventory.ConsumeSelected())
            {
                throw new InvalidOperationException("A planted seed could not be consumed.");
            }
            CommitVisualChange();
        }

        private void Harvest(FarmPlotState state)
        {
            ItemId cropItemId = state.Crop.HarvestItemId;
            if (!Inventory.CanAddItem(cropItemId))
            {
                Debug.Log("[Farming] Inventory is full; crop was not harvested.");
                return;
            }

            if (!state.TryHarvest(out ItemId harvestedItemId)) return;
            AddItemResult result = Inventory.AddItem(harvestedItemId);
            if (!result.Changed)
            {
                throw new InvalidOperationException(
                    "Harvested crop could not be added to inventory.");
            }
            CommitVisualChange();
        }

        private void CommitVisualChange()
        {
            Render();
            FindFirstObjectByType<InventoryHotbarView>()?.Refresh();
        }

        private void HandleDayChanged(FarmSimulator.Domain.Time.GameDate date) => Render();

        private string BuildPrompt()
        {
            if (string.IsNullOrWhiteSpace(plotId)) return "Parcela";
            FarmPlotState state = State;
            if (state.IsMature) return $"Cosechar {state.Crop.Name}";

            InventorySlot selected = Inventory.GetSelectedSlot();
            if (selected.IsEmpty) return "Selecciona azada, regadera o semillas";

            ItemId selectedItem = selected.ItemId.Value;
            if (selectedItem == ItemId.Hoe)
                return state.IsTilled ? "La parcela ya está arada" : "Arar parcela";
            if (selectedItem == ItemId.WateringCan)
            {
                if (!state.IsTilled) return "Primero ara la parcela";
                return state.IsWatered
                    ? "La parcela ya está regada"
                    : "Regar parcela";
            }

            if (CropCatalog.TryGetBySeed(selectedItem, out CropDefinition crop))
            {
                if (!state.IsTilled) return "Primero ara la parcela";
                return state.HasCrop
                    ? "La parcela ya tiene un cultivo"
                    : $"Sembrar {crop.Name}";
            }

            return "Este objeto no sirve en la parcela";
        }

        private Sprite[] StagesFor(ItemId seedItemId)
        {
            if (seedItemId == ItemId.TurnipSeeds) return turnipStages;
            if (seedItemId == ItemId.CarrotSeeds) return carrotStages;
            if (seedItemId == ItemId.CabbageSeeds) return cabbageStages;
            throw new ArgumentOutOfRangeException(
                nameof(seedItemId),
                seedItemId.Value,
                "Crop sprites are not configured.");
        }

        private static Sprite[] ValidateStages(Sprite[] stages, string parameterName)
        {
            if (stages == null || stages.Length != CropCatalog.FinalVisualStage + 1)
            {
                throw new ArgumentException(
                    "A crop must provide exactly six visual stages.",
                    parameterName);
            }

            var copy = (Sprite[])stages.Clone();
            for (int index = 0; index < copy.Length; index++)
            {
                if (copy[index] == null)
                {
                    throw new ArgumentException(
                        "Crop visual stages cannot contain null sprites.",
                        parameterName);
                }
            }

            return copy;
        }
    }
}
