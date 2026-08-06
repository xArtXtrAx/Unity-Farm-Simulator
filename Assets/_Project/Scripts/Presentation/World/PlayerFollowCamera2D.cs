using FarmSimulator.Presentation.Player;
using UnityEngine;

namespace FarmSimulator.Presentation.World
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class PlayerFollowCamera2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField, Min(0f)] private float smoothTime = 0.12f;
        [SerializeField] private bool clampToBounds = true;
        [SerializeField] private BoxCollider2D sceneBounds;
        [SerializeField] private Vector2 offset;

        private Camera targetCamera;
        private Vector3 velocity;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            ResolveReferences();
            SnapToTarget();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                ResolveReferences();
                if (target == null)
                {
                    return;
                }
            }

            Vector3 desired = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                transform.position.z);

            desired = ClampPosition(desired);
            transform.position = smoothTime <= 0f
                ? desired
                : Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
        }

        public void Configure(BoxCollider2D bounds, float followSmoothTime, bool shouldClamp)
        {
            sceneBounds = bounds;
            smoothTime = Mathf.Max(0f, followSmoothTime);
            clampToBounds = shouldClamp;
        }

        public void SnapToTarget()
        {
            if (target == null)
            {
                ResolveReferences();
            }

            if (target == null)
            {
                return;
            }

            Vector3 desired = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                transform.position.z);
            transform.position = ClampPosition(desired);
            velocity = Vector3.zero;
        }

        private void ResolveReferences()
        {
            if (targetCamera == null)
            {
                targetCamera = GetComponent<Camera>();
            }

            if (target == null)
            {
                PlayerPrefabIdentity identity = FindFirstObjectByType<PlayerPrefabIdentity>();
                if (identity != null)
                {
                    target = identity.transform;
                }
                else
                {
                    GameObject playerObject = GameObject.Find("Player");
                    if (playerObject != null)
                    {
                        target = playerObject.transform;
                    }
                }
            }

            if (sceneBounds == null)
            {
                BoxCollider2D[] colliders = FindObjectsByType<BoxCollider2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (BoxCollider2D candidate in colliders)
                {
                    if (candidate != null && candidate.name == "Scene Authoring Bounds")
                    {
                        sceneBounds = candidate;
                        break;
                    }
                }
            }
        }

        private Vector3 ClampPosition(Vector3 desired)
        {
            if (!clampToBounds || sceneBounds == null || targetCamera == null || !targetCamera.orthographic)
            {
                return desired;
            }

            Bounds bounds = sceneBounds.bounds;
            float halfHeight = targetCamera.orthographicSize;
            float halfWidth = halfHeight * targetCamera.aspect;

            float minX = bounds.min.x + halfWidth;
            float maxX = bounds.max.x - halfWidth;
            float minY = bounds.min.y + halfHeight;
            float maxY = bounds.max.y - halfHeight;

            desired.x = minX <= maxX ? Mathf.Clamp(desired.x, minX, maxX) : bounds.center.x;
            desired.y = minY <= maxY ? Mathf.Clamp(desired.y, minY, maxY) : bounds.center.y;
            return desired;
        }
    }
}
