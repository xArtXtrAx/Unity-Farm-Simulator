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
        [SerializeField] private Tilemap soil;
        [SerializeField] private Tilemap crops;
        [SerializeField] private Tilemap decoration;

        public Tilemap Ground => ground;
        public Tilemap Paths => paths;
        public Tilemap Soil => soil;
        public Tilemap Crops => crops;
        public Tilemap Decoration => decoration;

        public void Configure(
            Tilemap groundLayer,
            Tilemap pathLayer,
            Tilemap soilLayer,
            Tilemap cropLayer,
            Tilemap decorationLayer)
        {
            ground = groundLayer ??
                throw new ArgumentNullException(nameof(groundLayer));
            paths = pathLayer ??
                throw new ArgumentNullException(nameof(pathLayer));
            soil = soilLayer ??
                throw new ArgumentNullException(nameof(soilLayer));
            crops = cropLayer ??
                throw new ArgumentNullException(nameof(cropLayer));
            decoration = decorationLayer ??
                throw new ArgumentNullException(nameof(decorationLayer));
        }
    }
}
