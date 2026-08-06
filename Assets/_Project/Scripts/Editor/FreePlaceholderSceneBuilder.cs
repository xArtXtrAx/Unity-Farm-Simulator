using System;
using System.IO;
using FarmSimulator.Presentation.Art;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Configures Scene Recovery with the redistributable placeholder package and
    /// rebuilds Farm + HouseInterior without any licensed art dependency.
    /// </summary>
    public static class FreePlaceholderSceneBuilder
    {
        private const string MenuPath =
            "Tools/Farm Simulator/Farm Development Kit/Free Placeholder Scenes/Replace Farm + HouseInterior";

        private const string Root = "Assets/_Project/Art/Placeholder";
        private const string SourceRoot = Root + "/Source";
        private const string TileRoot = Root + "/Tiles";
        private const int PixelsPerCell = 16;

        private const string FarmScenePath = "Assets/_Project/Scenes/Farm.unity";
        private const string HouseScenePath = "Assets/_Project/Scenes/HouseInterior.unity";

        [MenuItem(MenuPath)]
        public static void ReplaceScenes()
        {
            if (!EditorUtility.DisplayDialog(
                    "Build free placeholder scenes",
                    "This generates any missing free placeholder assets, configures the scene profile, " +
                    "backs up Farm and HouseInterior, and rebuilds both scenes with redistributable art.",
                    "Build scenes",
                    "Cancel"))
            {
                return;
            }

            try
            {
                FreePlaceholderArtGenerator.GenerateMissingAssets();
                EnsureInteriorTiles();
                ConfigureProfile();

                // The modern authoring command owns backup, scene construction and build settings.
                ModernFarmSceneAuthoring.ReplaceScenesWithBackup();

                EditorApplication.delayCall += PostProcessGeneratedScenes;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(
                    "Free placeholder scenes failed",
                    exception.Message + "\n\nSee Console for the complete stack trace.",
                    "OK");
            }
        }

        private static void ConfigureProfile()
        {
            SceneRecoveryArtProfile profile = SceneRecoveryArtProfile.LoadOrCreate();
            profile.farmGroundTile = LoadRequired<TileBase>(TileRoot + "/ground_grass.asset");
            profile.farmPathTile = LoadRequired<TileBase>(TileRoot + "/path_dirt.asset");
            profile.farmHouseSprite = LoadRequired<Sprite>(SourceRoot + "/house_small_4x5.png");
            profile.houseFloorTile = LoadRequired<TileBase>(TileRoot + "/floor_wood.asset");
            profile.houseWallTile = LoadRequired<TileBase>(TileRoot + "/wall_cream.asset");
            profile.bedSprite = LoadRequired<Sprite>(SourceRoot + "/bed_single.png");
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
        }

        private static T LoadRequired<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                throw new InvalidOperationException($"Required placeholder asset missing: {path}");
            }

            return asset;
        }

        private static void EnsureInteriorTiles()
        {
            EnsureFolder(Root, "Source");
            EnsureFolder(Root, "Tiles");

            CreateTileIfMissing(
                "floor_wood",
                DrawWoodFloor,
                new Color32(176, 112, 58, 255));
            CreateTileIfMissing(
                "wall_cream",
                DrawCreamWall,
                new Color32(230, 205, 151, 255));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateTileIfMissing(
            string fileName,
            Action<Color32[], int, int, Color32> painter,
            Color32 baseColor)
        {
            string pngPath = SourceRoot + "/" + fileName + ".png";
            string tilePath = TileRoot + "/" + fileName + ".asset";

            if (AssetDatabase.LoadAssetAtPath<Tile>(tilePath) != null &&
                AssetDatabase.LoadAssetAtPath<Sprite>(pngPath) != null)
            {
                return;
            }

            var pixels = new Color32[PixelsPerCell * PixelsPerCell];
            painter(pixels, PixelsPerCell, PixelsPerCell, baseColor);

            var texture = new Texture2D(
                PixelsPerCell,
                PixelsPerCell,
                TextureFormat.RGBA32,
                false);
            texture.SetPixels32(pixels);
            texture.Apply(false, false);

            string absolute = Path.GetFullPath(pngPath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolute) ?? SourceRoot);
            File.WriteAllBytes(absolute, texture.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceSynchronousImport);
            var importer = AssetImporter.GetAtPath(pngPath) as TextureImporter;
            if (importer == null)
            {
                throw new InvalidOperationException("Could not configure " + pngPath);
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerCell;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();

            Sprite sprite = LoadRequired<Sprite>(pngPath);
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tilePath);
            }

            tile.name = fileName;
            tile.sprite = sprite;
            tile.color = Color.white;
            tile.colliderType = Tile.ColliderType.None;
            EditorUtility.SetDirty(tile);
            AssetDatabase.SetLabels(tile, new[] { "placeholder", "tile.house." + fileName });
        }

        private static void DrawWoodFloor(
            Color32[] pixels,
            int width,
            int height,
            Color32 baseColor)
        {
            Fill(pixels, width, height, baseColor);
            Color32 seam = new Color32(126, 76, 43, 255);
            Color32 highlight = new Color32(208, 144, 75, 255);
            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x++)
                {
                    Set(pixels, width, height, x, y, seam);
                }
            }

            for (int y = 2; y < height; y += 4)
            {
                Set(pixels, width, height, 3, y, highlight);
                Set(pixels, width, height, 11, y, highlight);
            }
        }

        private static void DrawCreamWall(
            Color32[] pixels,
            int width,
            int height,
            Color32 baseColor)
        {
            Fill(pixels, width, height, baseColor);
            Color32 edge = new Color32(176, 139, 92, 255);
            Color32 light = new Color32(245, 225, 180, 255);
            for (int x = 0; x < width; x++)
            {
                Set(pixels, width, height, x, 0, edge);
                Set(pixels, width, height, x, height - 1, edge);
            }

            for (int y = 0; y < height; y++)
            {
                Set(pixels, width, height, 0, y, edge);
                Set(pixels, width, height, width - 1, y, edge);
            }

            Set(pixels, width, height, 4, 10, light);
            Set(pixels, width, height, 10, 5, light);
        }

        private static void Fill(
            Color32[] pixels,
            int width,
            int height,
            Color32 color)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[y * width + x] = color;
                }
            }
        }

        private static void Set(
            Color32[] pixels,
            int width,
            int height,
            int x,
            int y,
            Color32 color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                pixels[y * width + x] = color;
            }
        }

        private static void PostProcessGeneratedScenes()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += PostProcessGeneratedScenes;
                return;
            }

            PostProcessScene(FarmScenePath, scene =>
            {
                GameObject house = Find(scene, "Hero House Visual");
                if (house == null)
                {
                    throw new InvalidOperationException("Farm is missing Hero House Visual.");
                }

                PlaceholderAssetIdentity identity =
                    house.GetComponent<PlaceholderAssetIdentity>() ??
                    house.AddComponent<PlaceholderAssetIdentity>();
                identity.Configure("building.house.small.4x5", new Vector2Int(4, 5));

                BoxCollider2D collider =
                    house.GetComponent<BoxCollider2D>() ??
                    house.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(3.8f, 1.8f);
                collider.offset = new Vector2(0f, 0.9f);
            });

            PostProcessScene(HouseScenePath, scene =>
            {
                GameObject bed = Find(scene, "Bed");
                if (bed == null)
                {
                    throw new InvalidOperationException("HouseInterior is missing Bed.");
                }

                PlaceholderAssetIdentity identity =
                    bed.GetComponent<PlaceholderAssetIdentity>() ??
                    bed.AddComponent<PlaceholderAssetIdentity>();
                identity.Configure("furniture.bed.single", new Vector2Int(1, 2));
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "Free placeholder scenes",
                "Farm and HouseInterior were rebuilt with redistributable placeholder art.\n\n" +
                "Review both scenes, then commit their .unity and .meta files plus the two new interior tiles.",
                "OK");
        }

        private static void PostProcessScene(string path, Action<Scene> action)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            try
            {
                action(scene);
                EditorSceneManager.MarkSceneDirty(scene);
                if (!EditorSceneManager.SaveScene(scene, path))
                {
                    throw new InvalidOperationException("Could not save " + path);
                }
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        private static GameObject Find(Scene scene, string objectName)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (Transform candidate in transforms)
                {
                    if (candidate.name == objectName)
                    {
                        return candidate.gameObject;
                    }
                }
            }

            return null;
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
