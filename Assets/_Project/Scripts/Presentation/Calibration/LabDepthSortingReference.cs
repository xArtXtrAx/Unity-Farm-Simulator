using FarmSimulator.Presentation.Player;
using UnityEngine;

namespace FarmSimulator.Presentation.Calibration
{
    [DisallowMultipleComponent]
    public sealed class LabDepthSortingReference : MonoBehaviour
    {
        public const string ObjectName = "Depth Sorting Reference";
        public const float Width = 1.5f;
        public const float Height = 2.25f;

        public static readonly Vector2 FeetPosition = new(0f, 0.65f);

        private static readonly Color ReferenceColor =
            new(0.72f, 0.24f, 0.86f, 1f);

        private GameObject referenceObject;
        private Texture2D generatedTexture;
        private Sprite generatedSprite;

        public GameObject ReferenceObject => referenceObject;

        private void OnEnable()
        {
            EnsureCreated();
        }

        private void LateUpdate()
        {
            if (referenceObject == null)
            {
                EnsureCreated();
            }
        }

        private void OnDestroy()
        {
            DestroyGeneratedAssets();
        }

        public GameObject EnsureCreated()
        {
            Transform generatedRoot = transform.Find(
                LabSpatialCalibration.GeneratedRootName);
            if (generatedRoot == null)
            {
                return null;
            }

            Transform existing = generatedRoot.Find(ObjectName);
            if (existing != null)
            {
                referenceObject = existing.gameObject;
                return referenceObject;
            }

            EnsureSprite();

            var instance = new GameObject(ObjectName);
            instance.transform.SetParent(generatedRoot, false);
            instance.transform.position = new Vector3(
                FeetPosition.x,
                FeetPosition.y,
                -0.55f);
            instance.transform.localScale = new Vector3(
                Width,
                Height,
                1f);

            SpriteRenderer renderer = instance.AddComponent<SpriteRenderer>();
            renderer.sprite = generatedSprite;
            renderer.color = ReferenceColor;

            TopDownSpriteSorting sorting =
                instance.AddComponent<TopDownSpriteSorting>();
            sorting.Initialize(renderer, instance.transform);

            referenceObject = instance;
            return referenceObject;
        }

        private void EnsureSprite()
        {
            if (generatedSprite != null)
            {
                return;
            }

            generatedTexture = new Texture2D(
                1,
                1,
                TextureFormat.RGBA32,
                mipChain: false)
            {
                name = "Depth Sorting Reference Texture",
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };
            generatedTexture.SetPixel(0, 0, Color.white);
            generatedTexture.Apply(
                updateMipmaps: false,
                makeNoLongerReadable: true);

            generatedSprite = Sprite.Create(
                generatedTexture,
                new Rect(0f, 0f, 1f, 1f),
                new Vector2(0.5f, 0f),
                pixelsPerUnit: 1f);
            generatedSprite.name = "Depth Sorting Reference Sprite";
            generatedSprite.hideFlags = HideFlags.DontSave;
        }

        private void DestroyGeneratedAssets()
        {
            if (generatedSprite != null)
            {
                Destroy(generatedSprite);
                generatedSprite = null;
            }

            if (generatedTexture != null)
            {
                Destroy(generatedTexture);
                generatedTexture = null;
            }
        }
    }
}
