using FarmSimulator.Application.Player;
using FarmSimulator.Application.Spatial;
using UnityEngine;

namespace FarmSimulator.Presentation.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TopDownPlayerMotor))]
    public sealed class PlayerProxyFacingView : MonoBehaviour
    {
        public const float MarkerSize = 0.14f;

        private const float MarkerGap = 0.03f;
        private const float MarkerDepth = -0.08f;

        private TopDownPlayerMotor motor;
        private Transform marker;

        public static float HorizontalVisualExtent =>
            SpatialModel.ReferenceCharacterWidth * 0.5f + MarkerSize + MarkerGap;

        public static float TopVisualExtent =>
            SpatialModel.ReferenceCharacterHeight + MarkerSize + MarkerGap;

        public static float BottomVisualExtent => MarkerSize + MarkerGap;

        private void Awake()
        {
            motor = GetComponent<TopDownPlayerMotor>();
        }

        private void LateUpdate()
        {
            Refresh();
        }

        public void Initialize(Transform facingMarker)
        {
            marker = facingMarker;
            Refresh();
        }

        public void Refresh()
        {
            if (marker == null || motor == null)
            {
                return;
            }

            marker.localPosition = CalculateMarkerLocalPosition(motor.Facing);
        }

        public static Vector3 CalculateMarkerLocalPosition(FacingDirection facing)
        {
            float bodyHalfWidth = SpatialModel.ReferenceCharacterWidth * 0.5f;
            float bodyHeight = SpatialModel.ReferenceCharacterHeight;
            float bodyCenterY = bodyHeight * 0.5f;
            float markerHalfSize = MarkerSize * 0.5f;
            float horizontalOffset = bodyHalfWidth + markerHalfSize + MarkerGap;
            float upperOffset = bodyHeight + markerHalfSize + MarkerGap;
            float lowerOffset = -(markerHalfSize + MarkerGap);

            return facing switch
            {
                FacingDirection.Up => new Vector3(0f, upperOffset, MarkerDepth),
                FacingDirection.Left => new Vector3(-horizontalOffset, bodyCenterY, MarkerDepth),
                FacingDirection.Right => new Vector3(horizontalOffset, bodyCenterY, MarkerDepth),
                _ => new Vector3(0f, lowerOffset, MarkerDepth),
            };
        }
    }
}
