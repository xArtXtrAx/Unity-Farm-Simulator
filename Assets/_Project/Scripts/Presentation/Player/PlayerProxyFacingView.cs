using FarmSimulator.Application.Player;
using UnityEngine;

namespace FarmSimulator.Presentation.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TopDownPlayerMotor))]
    public sealed class PlayerProxyFacingView : MonoBehaviour
    {
        private TopDownPlayerMotor motor;
        private Transform marker;

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

            marker.localPosition = motor.Facing switch
            {
                FacingDirection.Up => new Vector3(0f, 0.48f, -0.08f),
                FacingDirection.Left => new Vector3(-0.42f, 0f, -0.08f),
                FacingDirection.Right => new Vector3(0.42f, 0f, -0.08f),
                _ => new Vector3(0f, -0.48f, -0.08f),
            };
        }
    }
}
