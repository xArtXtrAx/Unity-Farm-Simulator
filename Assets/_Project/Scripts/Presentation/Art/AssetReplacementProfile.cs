using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Presentation.Art
{
    [CreateAssetMenu(
        fileName = "Asset Replacement Profile",
        menuName = "Farm Simulator/Art/Asset Replacement Profile")]
    public sealed class AssetReplacementProfile : ScriptableObject
    {
        [Serializable]
        public sealed class TileEntry
        {
            public TileBase source;
            public TileBase replacement;
        }

        [Serializable]
        public sealed class ObjectVisualEntry
        {
            public string assetKey;
            public Sprite replacementSprite;
            public GameObject replacementPrefab;
            public bool copySortingFromReplacement = true;
        }

        [SerializeField]
        private List<TileEntry> tileEntries = new List<TileEntry>();

        [SerializeField]
        private List<ObjectVisualEntry> objectVisualEntries =
            new List<ObjectVisualEntry>();

        public IReadOnlyList<TileEntry> TileEntries => tileEntries;
        public IReadOnlyList<ObjectVisualEntry> ObjectVisualEntries =>
            objectVisualEntries;
    }
}
