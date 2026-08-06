using System;
using System.Collections.Generic;
using System.Linq;
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
        public static readonly Vector2 DefaultHouseFootprintAnchor =
            new Vector2(0f, 0.5f);

        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private CozyBuildingCategory category;
        [SerializeField] private RectInt atlasRect;
        [SerializeField] private Vector2Int gridSize = Vector2Int.one;
        [SerializeField] private Vector2Int[] footprintOffsets = { Vector2Int.zero };
        [SerializeField] private bool footprintAuthored;
        [SerializeField] private Vector2 footprintAnchorOffset;
        [SerializeField] private bool footprintAnchorAuthored;
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
        public IReadOnlyList<Vector2Int> FootprintOffsets => footprintOffsets;
        public bool FootprintAuthored => footprintAuthored;
        public Vector2 FootprintAnchorOffset => footprintAnchorOffset;
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
            if (house == null) throw new ArgumentNullException(nameof(house));

            id = house.Id;
            displayName = house.DisplayName;
            category = CozyBuildingCategory.House;
            atlasRect = house.SourceRect;
            if (!footprintAuthored || footprintOffsets == null || footprintOffsets.Length == 0)
            {
                SetFootprintInternal(
                    new Vector2Int(4, 3),
                    CreateDefaultHouseFootprint(),
                    authored: false);
            }

            bool legacyPortalAnchor =
                (footprintAnchorOffset - house.PortalOffset).sqrMagnitude < 0.0001f;
            bool legacyCentreAnchor = footprintAnchorOffset.sqrMagnitude < 0.0001f;
            if (!footprintAnchorAuthored || legacyPortalAnchor || legacyCentreAnchor)
            {
                // Grid offsets identify cell centres. Moving the footprint origin half
                // a cell upward makes the lower edge of row zero coincide exactly with
                // the sprite's bottom-centre visual base. The next cell below remains
                // free for paths, steps and other entrance decoration.
                footprintAnchorOffset = DefaultHouseFootprintAnchor;
                footprintAnchorAuthored = false;
            }

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

        public void SetFootprint(Vector2Int size, IEnumerable<Vector2Int> offsets)
        {
            SetFootprintInternal(size, offsets, authored: true);
        }

        public void SetFootprintAnchor(Vector2 localOffset)
        {
            footprintAnchorOffset =
                category == CozyBuildingCategory.House && localOffset.sqrMagnitude < 0.0001f
                    ? DefaultHouseFootprintAnchor
                    : localOffset;
            footprintAnchorAuthored = true;
        }

        public void ResetFootprintToCategoryDefault()
        {
            footprintAnchorAuthored = false;
            footprintAnchorOffset = category == CozyBuildingCategory.House
                ? DefaultHouseFootprintAnchor
                : Vector2.zero;
            if (category == CozyBuildingCategory.House)
            {
                SetFootprintInternal(
                    new Vector2Int(4, 3),
                    CreateDefaultHouseFootprint(),
                    authored: false);
                return;
            }

            SetFootprintInternal(
                Vector2Int.one,
                new[] { Vector2Int.zero },
                authored: false);
        }

        public void AssignGeneratedPrefab(GameObject prefab)
        {
            generatedPrefab = prefab;
        }

        private void SetFootprintInternal(
            Vector2Int size,
            IEnumerable<Vector2Int> offsets,
            bool authored)
        {
            gridSize = new Vector2Int(Mathf.Max(1, size.x), Mathf.Max(1, size.y));
            footprintOffsets = offsets?
                .Distinct()
                .OrderBy(value => value.y)
                .ThenBy(value => value.x)
                .ToArray() ?? Array.Empty<Vector2Int>();
            if (footprintOffsets.Length == 0)
            {
                footprintOffsets = new[] { Vector2Int.zero };
            }
            footprintAuthored = authored;
        }

        private static IEnumerable<Vector2Int> CreateDefaultHouseFootprint()
        {
            for (int x = -2; x <= 1; x++)
            {
                yield return new Vector2Int(x, 0);
                yield return new Vector2Int(x, 1);
            }

            yield return new Vector2Int(-1, 2);
            yield return new Vector2Int(0, 2);
        }
    }
}
