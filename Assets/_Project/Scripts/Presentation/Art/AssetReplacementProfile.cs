using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool EnsureTileEntry(TileBase source)
        {
            if (source == null || tileEntries.Any(entry => entry != null && entry.source == source))
            {
                return false;
            }

            tileEntries.Add(new TileEntry { source = source });
            return true;
        }

        public bool EnsureObjectEntry(string assetKey)
        {
            if (string.IsNullOrWhiteSpace(assetKey) ||
                objectVisualEntries.Any(entry =>
                    entry != null &&
                    string.Equals(entry.assetKey, assetKey, StringComparison.Ordinal)))
            {
                return false;
            }

            objectVisualEntries.Add(new ObjectVisualEntry { assetKey = assetKey });
            return true;
        }

        public void SortEntries()
        {
            tileEntries = tileEntries
                .Where(entry => entry != null)
                .OrderBy(entry => entry.source != null ? entry.source.name : string.Empty)
                .ToList();

            objectVisualEntries = objectVisualEntries
                .Where(entry => entry != null)
                .OrderBy(entry => entry.assetKey ?? string.Empty)
                .ToList();
        }
    }
}
