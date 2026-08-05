using FarmSimulator.Application.Display;
using UnityEngine;

namespace FarmSimulator.Presentation.Calibration
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class ReferenceAspectCamera : MonoBehaviour
    {
        private Camera targetCamera;
        private int lastOutputWidth = -1;
        private int lastOutputHeight = -1;

        private void OnEnable()
        {
            targetCamera = GetComponent<Camera>();
            RefreshViewport();
        }

        private void LateUpdate()
        {
            if (Screen.width != lastOutputWidth ||
                Screen.height != lastOutputHeight)
            {
                RefreshViewport();
            }
        }

        public void RefreshViewport()
        {
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }

            NormalizedViewport viewport =
                PixelArtDisplayModel.CalculateViewport(
                    Screen.width,
                    Screen.height);

            targetCamera.rect = new Rect(
                viewport.X,
                viewport.Y,
                viewport.Width,
                viewport.Height);

            lastOutputWidth = Screen.width;
            lastOutputHeight = Screen.height;
        }
    }
}
