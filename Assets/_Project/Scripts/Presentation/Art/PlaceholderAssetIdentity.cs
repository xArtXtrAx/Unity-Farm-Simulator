using UnityEngine;

namespace FarmSimulator.Presentation.Art
{
    /// <summary>
    /// Stable semantic identity used by editor replacement tools.
    /// The key describes the role of an object, never its final art source.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlaceholderAssetIdentity : MonoBehaviour
    {
        [SerializeField]
        private string assetKey;

        [SerializeField]
        private Vector2Int footprintCells = Vector2Int.one;

        public string AssetKey => assetKey;
        public Vector2Int FootprintCells => footprintCells;

        public void Configure(string key, Vector2Int footprint)
        {
            assetKey = key;
            footprintCells = new Vector2Int(
                Mathf.Max(1, footprint.x),
                Mathf.Max(1, footprint.y));
        }
    }
}
