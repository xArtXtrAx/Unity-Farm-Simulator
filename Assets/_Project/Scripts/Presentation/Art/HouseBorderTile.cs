using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Presentation.Art
{
    /// <summary>
    /// Reuses one first-party wall sprite and rotates it per Tilemap border side.
    /// This keeps HouseInterior recovery deterministic without requiring a
    /// third-party ruleset or duplicated wall sprites.
    /// </summary>
    [CreateAssetMenu(fileName = "HouseBorderTile", menuName = "Farm Simulator/Tiles/House Border Tile")]
    public sealed class HouseBorderTile : TileBase
    {
        [SerializeField]
        private Sprite sprite;

        [SerializeField]
        private Color color = Color.white;

        public Sprite Sprite => sprite;

        public void Configure(Sprite value)
        {
            sprite = value;
            color = Color.white;
        }

        public override void GetTileData(
            Vector3Int position,
            ITilemap tilemap,
            ref TileData tileData)
        {
            tileData.sprite = sprite;
            tileData.color = color;
            tileData.colliderType = Tile.ColliderType.None;
            tileData.flags = TileFlags.LockColor;

            Tilemap concreteTilemap = tilemap.GetComponent<Tilemap>();
            BoundsInt bounds = concreteTilemap != null
                ? concreteTilemap.cellBounds
                : new BoundsInt(position, Vector3Int.one);
            tileData.transform = Matrix4x4.Rotate(
                Quaternion.Euler(0f, 0f, ResolveRotationDegrees(position, bounds)));
        }

        public static float ResolveRotationDegrees(
            Vector3Int position,
            BoundsInt bounds)
        {
            // Corners follow the horizontal run so the trim remains continuous.
            if (position.y == bounds.yMax - 1)
            {
                return 0f;
            }

            if (position.y == bounds.yMin)
            {
                return 180f;
            }

            if (position.x == bounds.xMin)
            {
                return 90f;
            }

            if (position.x == bounds.xMax - 1)
            {
                return -90f;
            }

            return 0f;
        }
    }
}
