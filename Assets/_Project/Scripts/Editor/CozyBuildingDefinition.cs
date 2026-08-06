using System;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public enum CozyBuildingCategory
    {
        House,
        Barn,
        Windmill,
        Greenhouse,
        Shop,
        Market,
        LargeBuilding,
        OutdoorProp,
    }

    [CreateAssetMenu(
        fileName = "Cozy Building Definition",
        menuName = "Farm Simulator/Cozy Building Definition")]
    public sealed class CozyBuildingDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private CozyBuildingCategory category;
        [SerializeField] private RectInt atlasRect;
        [SerializeField] private Vector2Int gridSize = Vector2Int.one;
        [SerializeField] private Vector2 doorAnchor;
        [SerializeField] private Vector2 portalOffset;
        [SerializeField] private Vector2 spawnOffset;
        [SerializeField] private Vector2 colliderSize;
        [SerializeField] private Vector2 colliderOffset;
        [SerializeField] private float maximumWidth = 1f;
        [SerializeField] private float maximumHeight = 1f;
        [SerializeField] private float baseline;
        [SerializeField] private Vector2 shadowOffset;
        [SerializeField] private Vector2 shadowScale = Vector2.one;
        [SerializeField] private int sortingOrder;
        [SerializeField] private bool supportsInterior;
        [SerializeField] private Sprite generatedSprite;
        [SerializeField] private GameObject generatedPrefab;

        public string Id => id;
        public string DisplayName => displayName;
        public CozyBuildingCategory Category => category;
        public RectInt AtlasRect => atlasRect;
        public Vector2Int GridSize => gridSize;
        public Vector2 DoorAnchor => doorAnchor;
        public Vector2 PortalOffset => portalOffset;
        public Vector2 SpawnOffset => spawnOffset;
        public Vector2 ColliderSize => colliderSize;
        public Vector2 ColliderOffset => colliderOffset;
        public float MaximumWidth => maximumWidth;
        public float MaximumHeight => maximumHeight;
        public float Baseline => baseline;
        public Vector2 ShadowOffset => shadowOffset;
        public Vector2 ShadowScale => shadowScale;
        public int SortingOrder => sortingOrder;
        public bool SupportsInterior => supportsInterior;
        public Sprite GeneratedSprite => generatedSprite;
        public GameObject GeneratedPrefab => generatedPrefab;

        public void ConfigureFromHouse(
            CozyFarmBuildingCatalog.HouseVariant house,
            Sprite sprite)
        {
            if (house == null)
            {
                throw new ArgumentNullException(nameof(house));
            }

            id = house.Id;
            displayName = house.DisplayName;
            category = CozyBuildingCategory.House;
            atlasRect = house.SourceRect;
            gridSize = new Vector2Int(
                Mathf.Max(1, Mathf.CeilToInt(house.MaximumWidth)),
                Mathf.Max(1, Mathf.CeilToInt(house.MaximumHeight)));
            doorAnchor = house.DoorAnchor;
            portalOffset = house.PortalOffset;
            spawnOffset = house.SpawnOffset;
            colliderSize = house.ColliderSize;
            colliderOffset = house.ColliderOffset;
            maximumWidth = house.MaximumWidth;
            maximumHeight = house.MaximumHeight;
            baseline = house.Baseline;
            shadowOffset = house.ShadowOffset;
            shadowScale = house.ShadowScale;
            sortingOrder = house.SortingOrder;
            supportsInterior = true;
            generatedSprite = sprite;
        }

        public void AssignGeneratedPrefab(GameObject prefab)
        {
            generatedPrefab = prefab;
        }
    }
}
