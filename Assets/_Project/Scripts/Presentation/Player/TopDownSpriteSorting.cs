using FarmSimulator.Application.Spatial;
using UnityEngine;

namespace FarmSimulator.Presentation.Player
{
    [DisallowMultipleComponent]
    public sealed class TopDownSpriteSorting : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer targetRenderer;

        [SerializeField]
        private Transform feet;

        [SerializeField]
        private int baseOrder = TopDownSortingModel.DefaultBaseOrder;

        [SerializeField]
        [Min(1)]
        private int ordersPerUnit = TopDownSortingModel.DefaultOrdersPerUnit;

        public SpriteRenderer TargetRenderer => targetRenderer;

        public Transform Feet => feet;

        public int CurrentOrder =>
            targetRenderer == null ? 0 : targetRenderer.sortingOrder;

        private void Awake()
        {
            ResolveReferences();
            Refresh();
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void LateUpdate()
        {
            Refresh();
        }

        public void Initialize(
            SpriteRenderer renderer,
            Transform feetTransform,
            int sortingBaseOrder = TopDownSortingModel.DefaultBaseOrder,
            int sortingOrdersPerUnit = TopDownSortingModel.DefaultOrdersPerUnit)
        {
            targetRenderer = renderer;
            feet = feetTransform;
            baseOrder = sortingBaseOrder;
            ordersPerUnit = Mathf.Max(1, sortingOrdersPerUnit);

            if (targetRenderer != null)
            {
                targetRenderer.sortingLayerName = TopDownSortingLayers.Actors;
            }

            Refresh();
        }

        public void Refresh()
        {
            ResolveReferences();
            if (targetRenderer == null || feet == null)
            {
                return;
            }

            targetRenderer.sortingLayerName = TopDownSortingLayers.Actors;
            targetRenderer.sortingOrder = TopDownSortingModel.OrderForFeetY(
                feet.position.y,
                baseOrder,
                ordersPerUnit);
        }

        private void ResolveReferences()
        {
            if (feet == null)
            {
                feet = transform;
            }

            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<SpriteRenderer>(
                    includeInactive: true);
            }
        }
    }
}
