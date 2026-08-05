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
        public const string SpriteProxyObjectName = "Sprite Proxy";

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

            Material groundMaterial = CreateMaterial(new Color(0.31f, 0.48f, 0.28f));
            Material gridMaterial = CreateMaterial(new Color(0.72f, 0.78f, 0.68f));
            Material xAxisMaterial = CreateMaterial(new Color(0.84f, 0.24f, 0.20f));
            Material zAxisMaterial = CreateMaterial(new Color(0.20f, 0.42f, 0.86f));
            Material spriteMaterial = CreateMaterial(new Color(0.96f, 0.72f, 0.18f));
            Material arcMaterial = CreateMaterial(new Color(0.70f, 0.28f, 0.86f));

            BuildGround(groundMaterial);
            BuildGrid(gridMaterial, xAxisMaterial, zAxisMaterial);
            BuildSpriteReferences(spriteMaterial);
            BuildHeightReference(xAxisMaterial);
            BuildArcReference(arcMaterial);
        }

        private void ConfigureCamera()
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
            camera.backgroundColor = new Color(0.61f, 0.77f, 0.88f);

            camera.transform.position = new Vector3(10f, 10f, -10f);
            camera.transform.rotation = Quaternion.LookRotation(
                new Vector3(0f, 0.5f, 0f) - camera.transform.position,
                Vector3.up);
        }

        private static void ConfigureLight()
        {
            Light sceneLight = Object.FindFirstObjectByType<Light>();
            if (sceneLight == null)
            {
                return;
            }

            sceneLight.type = LightType.Directional;
            sceneLight.intensity = 1.35f;
            sceneLight.shadows = LightShadows.Soft;
            sceneLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private void BuildGround(Material material)
        {
            float width = SpatialModel.GridColumns * SpatialModel.GridCellSize;
            float depth = SpatialModel.GridRows * SpatialModel.GridCellSize;

            CreatePrimitive(
                PrimitiveType.Cube,
                GroundObjectName,
                new Vector3(0f, -0.05f, 0f),
                new Vector3(width, 0.1f, depth),
                material,
                keepCollider: true);
        }

        private void BuildGrid(Material gridMaterial, Material xAxisMaterial, Material zAxisMaterial)
        {
            float width = SpatialModel.GridColumns * SpatialModel.GridCellSize;
            float depth = SpatialModel.GridRows * SpatialModel.GridCellSize;
            float halfWidth = width * 0.5f;
            float halfDepth = depth * 0.5f;
            const float lineThickness = 0.025f;

            for (int column = 0; column <= SpatialModel.GridColumns; column++)
            {
                float x = -halfWidth + column * SpatialModel.GridCellSize;
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"Grid X {column:00}",
                    new Vector3(x, 0.0125f, 0f),
                    new Vector3(lineThickness, 0.025f, depth),
                    gridMaterial,
                    keepCollider: false);
            }

            for (int row = 0; row <= SpatialModel.GridRows; row++)
            {
                float z = -halfDepth + row * SpatialModel.GridCellSize;
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"Grid Z {row:00}",
                    new Vector3(0f, 0.0125f, z),
                    new Vector3(width, 0.025f, lineThickness),
                    gridMaterial,
                    keepCollider: false);
            }

            CreatePrimitive(
                PrimitiveType.Cube,
                "X Axis",
                new Vector3(0f, 0.035f, 0f),
                new Vector3(width, 0.05f, 0.07f),
                xAxisMaterial,
                keepCollider: false);

            CreatePrimitive(
                PrimitiveType.Cube,
                "Z Axis",
                new Vector3(0f, 0.04f, 0f),
                new Vector3(0.07f, 0.06f, depth),
                zAxisMaterial,
                keepCollider: false);
        }

        private void BuildSpriteReferences(Material material)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                SpriteProxyObjectName,
                new Vector3(0f, SpatialModel.ReferenceCharacterHeight * 0.5f, 0f),
                new Vector3(0.8f, SpatialModel.ReferenceCharacterHeight, 0.08f),
                material,
                keepCollider: false,
                faceCamera: true);

            float[] depths = { -3f, 0f, 3f };
            for (int index = 0; index < depths.Length; index++)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"Sprite Depth {index + 1}",
                    new Vector3(3.5f, 0.75f, depths[index]),
                    new Vector3(0.65f, 1.5f, 0.06f),
                    material,
                    keepCollider: false,
                    faceCamera: true);
            }
        }

        private void BuildHeightReference(Material material)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "Y Height Reference",
                new Vector3(-4.5f, 1f, -3.5f),
                new Vector3(0.1f, 2f, 0.1f),
                material,
                keepCollider: false);

            for (int tick = 0; tick <= 4; tick++)
            {
                float y = tick * 0.5f;
                CreatePrimitive(
                    PrimitiveType.Cube,
                    $"Height Tick {tick}",
                    new Vector3(-4.35f, y, -3.5f),
                    new Vector3(0.3f, 0.035f, 0.08f),
                    material,
                    keepCollider: false);
            }
        }

        private void BuildArcReference(Material material)
        {
            const int points = 13;
            Vector3 start = new(-4f, 0.2f, 2.75f);
            Vector3 end = new(4f, 0.2f, 2.75f);

            for (int index = 0; index < points; index++)
            {
                float t = index / (float)(points - 1);
                Vector3 position = Vector3.Lerp(start, end, t);
                position.y += 2.5f * 4f * t * (1f - t);

                CreatePrimitive(
                    PrimitiveType.Sphere,
                    $"Arc Point {index:00}",
                    position,
                    Vector3.one * 0.16f,
                    material,
                    keepCollider: false);
            }
        }

        private GameObject CreatePrimitive(
            PrimitiveType primitiveType,
            string objectName,
            Vector3 position,
            Vector3 scale,
            Material material,
            bool keepCollider,
            bool faceCamera = false)
        {
            GameObject instance = GameObject.CreatePrimitive(primitiveType);
            instance.name = objectName;
            instance.transform.SetParent(generatedRoot, false);
            instance.transform.position = position;
            instance.transform.localScale = scale;

            if (faceCamera && Camera.main != null)
            {
                Vector3 direction = Camera.main.transform.position - instance.transform.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.001f)
                {
                    instance.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                }
            }

            Renderer primitiveRenderer = instance.GetComponent<Renderer>();
            primitiveRenderer.sharedMaterial = material;
            primitiveRenderer.shadowCastingMode = ShadowCastingMode.On;
            primitiveRenderer.receiveShadows = true;

            if (!keepCollider)
            {
                Collider primitiveCollider = instance.GetComponent<Collider>();
                if (primitiveCollider != null)
                {
                    Destroy(primitiveCollider);
                }
            }

            return instance;
        }

        private Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
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
