using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Presentation.Farming
{
    [DisallowMultipleComponent]
    public sealed class FarmTilemapLayers : MonoBehaviour
    {
        [SerializeField] private Tilemap ground;
        [SerializeField] private Tilemap paths;
        [SerializeField] private Tilemap farming;
        [SerializeField] private Tilemap decoration;

        public Tilemap Ground => ground;
        public Tilemap Paths => paths;
        public Tilemap Farming => farming;
        public Tilemap Decoration => decoration;

        public void Configure(
            Tilemap groundLayer,
            Tilemap pathLayer,
            Tilemap farmingLayer,
            Tilemap decorationLayer)
        {
            ground = groundLayer ??
                throw new ArgumentNullException(nameof(groundLayer));
            paths = pathLayer ??
                throw new ArgumentNullException(nameof(pathLayer));
            farming = farmingLayer ??
                throw new ArgumentNullException(nameof(farmingLayer));
            decoration = decorationLayer ??
                throw new ArgumentNullException(nameof(decorationLayer));
        }
    }
}
