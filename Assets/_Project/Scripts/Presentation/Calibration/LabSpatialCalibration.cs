using System.Collections.Generic;
using FarmSimulator.Application.Spatial;
using UnityEngine;
using UnityEngine.Rendering;

namespace FarmSimulator.Presentation.Calibration
{
    [DisallowMultipleComponent]
    public sealed class LabSpatialCalibration : MonoBehaviour
    {
        public const string GeneratedRootName = "Generated Calibration";
        public const string GroundObjectName = "Calibration Ground";
        public const string GroundVisualObjectName = "Calibration Ground Visual";
        public const string SpriteProxyObjectName = "Sprite Proxy";
        public const string DepthStackObjectName = "Depth Stack Front";

        private readonly List<Material> generatedMaterials = new();
        private Transform generatedRoot;

        private void OnEnable()
        {
            Rebuild();
        }

        private void OnDestroy()
        {
            DestroyGeneratedMaterials();
        }

        [ContextMenu("Rebuild Calibration")]
        public void Rebuild()
        {
            RemovePreviousGeneratedRoot();
            DestroyGeneratedMaterials();
            ConfigureCamera();
            ConfigureLight();

            var rootObject = new GameObject(GeneratedRootName);
            rootObject.transform.SetParent(transform, false);
            generatedRoot = rootObject.transform;

            Material groundMaterial = CreateMaterial(new Color(0.34f, 0.52f, 0.30f));
            Material gridMaterial = CreateMaterial(new Color(0.74f, 0.82f, 0.69f));
            Material xAxisMaterial = CreateMaterial(new Color(0.86f, 0.25f, 0.20f));
            Material yAxisMaterial = CreateMaterial(new Color(0.20f, 0.43f, 0.88f));
            Material pathMaterial = CreateMaterial(new Color(0.68f, 0.48f, 0.24f));
            Material waterMaterial = CreateMaterial(new Color(0.25f, 0.58f, 0.82f));
            Material plotMaterial = CreateMaterial(new Color(0.39f, 0.23f, 0.12f));
            Material spriteMaterial = CreateMaterial(new Color(0.96f, 0.72f, 0.18f));
            Material foliageMaterial = CreateMaterial(new Color(0.16f, 0.42f, 0.18f));
            Material trunkMaterial = CreateMaterial(new Color(0.38f, 0.20f, 0.08f));
            Material arcMaterial = CreateMaterial(new Color(0.72f, 0.30f, 0.88f));
            Material depthMaterial = CreateMaterial(new Color(0.20f, 0.78f, 0.78f));

            BuildGround(groundMaterial);
            BuildMapReferences(
                pathMaterial,
                waterMaterial,
                plotMaterial,
                foliageMaterial,
                trunkMaterial);
            BuildGrid(gridMaterial, xAxisMaterial, yAxisMaterial);
            BuildSpriteReferences(spriteMaterial);
            BuildDepthStack(depthMaterial);
            BuildArcReference(arcMaterial);
        }

        private static void ConfigureCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            camera.orthographic = SpatialModel.UsesOrthographicCamera;
            camera.orthographicSize = SpatialModel.CameraOrthographicSize;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.10f, 0.13f, 0.11f);
            camera.transform.position = new Vector3(0f, 0f, SpatialModel.CameraDepth);
            camera.transform.rotation = Quaternion.identity;

            ReferenceAspectCamera aspectCamera =
                camera.GetComponent<ReferenceAspectCamera>();
            if (aspectCamera == null)
            {
                aspectCamera = camera.gameObject.AddComponent<ReferenceAspectCamera>();
            }

            aspectCamera.RefreshViewport();
        }

        private static void ConfigureLight()
        {
            Light sceneLight = Object.FindFirstObjectByType<Light>();
            if (sceneLight != null)
            {
                sceneLight.enabled = false;
            }
        }

        private void BuildGround(Material material)
        {
            float width = SpatialModel.GridColumns * SpatialModel.GridCellSize;
            float height = SpatialModel.GridRows * SpatialModel.GridCellSize;
            Vector3 groundPosition = new(0f, 0f, 0.2f);

            var ground = new GameObject(GroundObjectName);
            ground.transform.SetParent(generatedRoot, false);
            ground.transform.position = groundPosition;
            ground.transform.localScale = new Vector3(width, height, 1f);

            BoxCollider2D groundCollider = ground.AddComponent<BoxCollider2D>();
            groundCollider.size = Vector2.one;

            CreatePrimitive(
                PrimitiveType.Cube,
                GroundVisualObjectName,
                groundPosition,
                new Vector3(width, height, 0.1f),
                material);
        }

        private void BuildMapReferences(
            Material pathMaterial,
            Material waterMaterial,
            Material plotMaterial,
            Material foliageMaterial,
            Material trunkMaterial)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "Horizontal Path",
                new Vector3(0f, -1.25f, 0.10f),
                new Vector3(13.5f, 1.5f, 0.04f),
                pathMaterial);

            CreatePrimitive(
                PrimitiveType.Cube,
                "Vertical Path",
                new Vector3(-2.25f, 1f, 0.09f),
                new Vector3(1.5f, 5.6f, 0.04f),
                pathMaterial);

            CreatePrimitive(
                PrimitiveType.Sphere,
                "Pond Proxy",
                new Vector3(4.4f, 2.15f, 0.02f),
                new Vector3(2.2f, 1.3f, 0.08f),
                waterMaterial);

            for (int column = 0; column < 3; column++)
            {
                for (int row = 0; row < 2; row++)
                {
                    CreatePrimitive(
                        PrimitiveType.Cube,
                        $"Crop Plot {column}-{row}",
                        new Vector3(
                            -5.5f + column * 1.05f,
                            -3.3f + row * 0.95f,
                            0.03f),
                        new Vector3(0.8f, 0.8f, 0.04f),
                        plotMaterial);
                }
            }

            BuildTreeProxy(
                new Vector2(-5.2f, 0.9f),
                foliageMaterial,
                trunkMaterial);
            BuildTreeProxy(
                new Vector2(5.2f, -3.2f),
                foliageMaterial,
                trunkMaterial);
        }

        private void BuildTreeProxy(
            Vector2 groundPosition,
            Material foliageMaterial,
            Material trunkMaterial)
        {
            const float trunkHeight = 1.05f;
            const float canopyHeight = 1.9f;

            CreatePrimitive(
                PrimitiveType.Cube,
                $"Tree Trunk {groundPosition.x:0.0},{groundPosition.y:0.0}",
                new Vector3(
                    groundPosition.x,
                    groundPosition.y + trunkHeight * 0.5f,
                    VisualDepthForY(groundPosition.y) - 0.01f),
                new Vector3(0.5f, trunkHeight, 0.05f),
                trunkMaterial);

            CreatePrimitive(
                PrimitiveType.Sphere,
                $"Tree Canopy {groundPosition.x:0.0},{groundPosition.y:0.0}",
                new Vector3(
                    groundPosition.x,
                    groundPosition.y + trunkHeight + canopyHeight * 0.35f,
                    VisualDepthForY(groundPosition.y) - 0.02f),
                new Vector3(1.9f, canopyHeight, 0.08f),
                foliageMaterial);
        }

        private void BuildGrid(
            Material gridMaterial,
            Material xAxisMaterial,
            Material yAxisMaterial)
        {
            float width = SpatialModel.GridColumns * SpatialModel.GridCellSize;
            float height = SpatialModel.GridRows * SpatialModel.GridCellSize;
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            const float lineThickness = 0.025f;

            for (int column = 0; column <= SpatialModel.GridColumns; column++)
            {
                float x = -halfWidth + column * SpatialModel.GridCellSize;
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"Grid X {column:00}",
                    new Vector3(x, 0f, -0.01f),
                    new Vector3(lineThickness, height, 0.02f),
                    gridMaterial);
            }

            for (int row = 0; row <= SpatialModel.GridRows; row++)
            {
                float y = -halfHeight + row * SpatialModel.GridCellSize;
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"Grid Y {row:00}",
                    new Vector3(0f, y, -0.01f),
                    new Vector3(width, lineThickness, 0.02f),
                    gridMaterial);
            }

            CreatePrimitive(
                PrimitiveType.Cube,
                "X Axis",
                new Vector3(0f, 0f, -0.03f),
                new Vector3(width, 0.07f, 0.025f),
                xAxisMaterial);

            CreatePrimitive(
                PrimitiveType.Cube,
                "Y Axis",
                new Vector3(0f, 0f, -0.04f),
                new Vector3(0.07f, height, 0.025f),
                yAxisMaterial);
        }

        private void BuildSpriteReferences(Material material)
        {
            CreateSpriteProxy(
                SpriteProxyObjectName,
                new Vector2(0f, -0.6f),
                SpatialModel.ReferenceCharacterWidth,
                SpatialModel.ReferenceCharacterHeight,
                material);

            CreateSpriteProxy(
                "Sprite North",
                new Vector2(1.4f, 2.1f),
                SpatialModel.ReferenceCharacterWidth,
                SpatialModel.ReferenceCharacterHeight,
                material);
            CreateSpriteProxy(
                "Sprite Center",
                new Vector2(1.4f, 0.2f),
                SpatialModel.ReferenceCharacterWidth,
                SpatialModel.ReferenceCharacterHeight,
                material);
            CreateSpriteProxy(
                "Sprite South",
                new Vector2(1.4f, -2.2f),
                SpatialModel.ReferenceCharacterWidth,
                SpatialModel.ReferenceCharacterHeight,
                material);
        }

        private void CreateSpriteProxy(
            string objectName,
            Vector2 feetPosition,
            float width,
            float height,
            Material material)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                objectName,
                new Vector3(
                    feetPosition.x,
                    feetPosition.y + height * 0.5f,
                    VisualDepthForY(feetPosition.y)),
                new Vector3(width, height, 0.05f),
                material);
        }

        private void BuildDepthStack(Material material)
        {
            for (int layer = 0; layer < 3; layer++)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    layer == 2
                        ? DepthStackObjectName
                        : $"Depth Stack {layer + 1}",
                    new Vector3(
                        -4f + layer * 0.25f,
                        0.35f - layer * 0.2f,
                        -0.18f - layer * 0.08f),
                    new Vector3(1f, 1f, 0.04f),
                    material);
            }
        }

        private void BuildArcReference(Material material)
        {
            const int points = 13;
            Vector3 start = new(-4.5f, 2.55f, -0.35f);
            Vector3 end = new(4.5f, 2.55f, -0.35f);

            for (int index = 0; index < points; index++)
            {
                float t = index / (float)(points - 1);
                Vector3 position = Vector3.Lerp(start, end, t);
                position.y += 1.35f * 4f * t * (1f - t);
                position.z -= 0.15f * Mathf.Sin(Mathf.PI * t);

                CreatePrimitive(
                    PrimitiveType.Sphere,
                    $"Arc Point {index:00}",
                    position,
                    Vector3.one * 0.14f,
                    material);
            }
        }

        private static float VisualDepthForY(float feetY)
        {
            return -0.20f + feetY * 0.01f;
        }

        private GameObject CreatePrimitive(
            PrimitiveType primitiveType,
            string objectName,
            Vector3 position,
            Vector3 scale,
            Material material)
        {
            GameObject instance = GameObject.CreatePrimitive(primitiveType);
            instance.name = objectName;
            instance.transform.SetParent(generatedRoot, false);
            instance.transform.position = position;
            instance.transform.localScale = scale;

            Renderer primitiveRenderer = instance.GetComponent<Renderer>();
            primitiveRenderer.sharedMaterial = material;
            primitiveRenderer.shadowCastingMode = ShadowCastingMode.Off;
            primitiveRenderer.receiveShadows = false;

            Collider primitiveCollider = instance.GetComponent<Collider>();
            if (primitiveCollider != null)
            {
                Destroy(primitiveCollider);
            }

            return instance;
        }

        private Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Color")
                ?? Shader.Find("Standard");

            var material = new Material(shader)
            {
                color = color,
                hideFlags = HideFlags.DontSave,
            };

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            generatedMaterials.Add(material);
            return material;
        }

        private void RemovePreviousGeneratedRoot()
        {
            Transform previousRoot = transform.Find(GeneratedRootName);
            if (previousRoot != null)
            {
                Destroy(previousRoot.gameObject);
            }

            generatedRoot = null;
        }

        private void DestroyGeneratedMaterials()
        {
            foreach (Material material in generatedMaterials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }

            generatedMaterials.Clear();
        }
    }
}
