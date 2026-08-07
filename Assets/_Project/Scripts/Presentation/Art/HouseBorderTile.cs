using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Presentation.Art
{
    /// <summary>
    /// Reuses one first-party wall sprite and rotates it per Tilemap border side.
    /// This keeps HouseInterior recovery deterministic without requiring a
    /// third-party ruleset or four duplicated wall sprites.
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
            tileData.transform = ResolveTransform(position, tilemap);
        }

        private static Matrix4x4 ResolveTransform(
            Vector3Int position,
            ITilemap tilemap)
        {
            Tilemap concreteTilemap = tilemap.GetComponent<Tilemap>();
            if (concreteTilemap == null)
            {
                return Matrix4x4.identity;
            }

            BoundsInt bounds = concreteTilemap.cellBounds;
            float rotation = 0f;

            // Keep corners aligned with the horizontal runs so the top and
            // bottom trim remain continuous. Vertical edges are rotated so
            // the wooden baseboard always faces toward the room interior.
            if (position.y == bounds.yMax - 1)
            {
                rotation = 0f;
            }
            else if (position.y == bounds.yMin)
            {
                rotation = 180f;
            }
            else if (position.x == bounds.xMin)
            {
                rotation = 90f;
            }
            else if (position.x == bounds.xMax - 1)
            {
                rotation = -90f;
            }

            return Matrix4x4.Rotate(Quaternion.Euler(0f, 0f, rotation));
        }
    }
}
