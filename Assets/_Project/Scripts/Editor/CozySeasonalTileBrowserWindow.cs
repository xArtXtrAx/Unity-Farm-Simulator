using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    public enum CozyTileSeason
    {
        Spring,
        Summer,
        Autumn,
        Winter,
    }

    public sealed class CozySeasonalTileBrowserWindow : EditorWindow
    {
        public const string TileSourceRoot =
            CozyFarmFullPackImporter.FullAssetRoot + "/Tiles";
        public const string GeneratedRoot =
            "Assets/_Project/Art/Generated/CozyFarm/SeasonalTiles";
        public const string PaletteRoot =
            GeneratedRoot + "/Palettes";
        public const int TileSize = 16;

        private static readonly CozyTileSeason[] Seasons =
        {
            CozyTileSeason.Spring,
            CozyTileSeason.Summer,
            CozyTileSeason.Autumn,
            CozyTileSeason.Winter,
        };

        private CozyTileSeason season;
        private string paintLayer = "Ground";
        private Vector2 scroll;
        private float thumbnailSize = 58f;
        private List<SpriteEntry> entries = new List<SpriteEntry>();

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Seasonal Tile Browser")]
        public static void Open()
        {
            var window = GetWindow<CozySeasonalTileBrowserWindow>();
            window.titleContent = new GUIContent("Seasonal Tiles");
            window.minSize = new Vector2(620f, 440f);
            window.Show();
        }

        private void OnEnable()
        {
            Reload();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "Farm Development Kit — Cozy Seasonal Tiles",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Slices every non-empty 16×16 cell found under CozyFarm/Full/Tiles, " +
                "classifies it by season and builds four Unity Tile Palettes. " +
                "The purchased PNG files remain local and outside Git.",
                MessageType.Info);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Prepare / reslice all tile sheets", GUILayout.Height(28f)))
                {
                    PrepareAllSheets();
                    Reload();
                }
                if (GUILayout.Button("Rebuild all seasonal palettes", GUILayout.Height(28f)))
                {
                    RebuildAllPalettes();
                    Reload();
                }
                if (GUILayout.Button("Refresh", GUILayout.Height(28f)))
                {
                    Reload();
                }
            }

            EditorGUILayout.Space();
            CozyTileSeason next = (CozyTileSeason)GUILayout.Toolbar(
                (int)season,
                Seasons.Select(DisplayName).ToArray());
            if (next != season)
            {
                season = next;
                scroll = Vector2.zero;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                paintLayer = EditorGUILayout.Popup(
                    "Paint target",
                    Array.IndexOf(LayerNames, paintLayer),
                    LayerNames);
                int layerIndex = Mathf.Clamp(
                    EditorGUILayout.Popup(
                        Array.IndexOf(LayerNames, paintLayer),
                        LayerNames,
                        GUILayout.Width(130f)),
                    0,
                    LayerNames.Length - 1);
                paintLayer = LayerNames[layerIndex];

                if (GUILayout.Button("Open season palette", GUILayout.Width(170f)))
                {
                    OpenPalette(season, paintLayer);
                }
            }

            thumbnailSize = EditorGUILayout.Slider(
                "Thumbnail size",
                thumbnailSize,
                36f,
                96f);

            List<SpriteEntry> visible = entries
                .Where(entry => entry.Season == season)
                .ToList();
            EditorGUILayout.LabelField(
                $"{DisplayName(season)}: {visible.Count} non-empty tiles",
                EditorStyles.miniBoldLabel);

            if (visible.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    Directory.Exists(ToAbsolutePath(TileSourceRoot))
                        ? "No sliced sprites were found for this season. Run 'Prepare / reslice all tile sheets'."
                        : $"The local folder '{TileSourceRoot}' was not found. Import the Cozy Farm full pack first.",
                    MessageType.Warning);
                return;
            }

            float cellWidth = thumbnailSize + 12f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((position.width - 24f) / cellWidth));
            scroll = EditorGUILayout.BeginScrollView(scroll);
            int index = 0;
            while (index < visible.Count)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int column = 0; column < columns && index < visible.Count; column++, index++)
                    {
                        DrawSpriteButton(visible[index]);
                    }
                    GUILayout.FlexibleSpace();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private static readonly string[] LayerNames =
        {
            "Ground",
            "Paths",
            "Soil",
            "Decoration",
        };

        private void DrawSpriteButton(SpriteEntry entry)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(thumbnailSize + 8f)))
            {
                Texture preview = AssetPreview.GetAssetPreview(entry.Sprite) ??
                    AssetPreview.GetMiniThumbnail(entry.Sprite);
                if (GUILayout.Button(
                        preview,
                        GUILayout.Width(thumbnailSize),
                        GUILayout.Height(thumbnailSize)))
                {
                    Selection.activeObject = entry.Sprite;
                    EditorGUIUtility.PingObject(entry.Sprite);
                }

                GUILayout.Label(
                    entry.ShortName,
                    EditorStyles.centeredGreyMiniLabel,
                    GUILayout.Width(thumbnailSize),
                    GUILayout.Height(30f));
            }
        }

        private void Reload()
        {
            entries = DiscoverSprites();
            Repaint();
        }

        public static List<SpriteEntry> DiscoverSprites()
        {
            var result = new List<SpriteEntry>();
            if (!AssetDatabase.IsValidFolder(TileSourceRoot)) return result;

            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { TileSourceRoot }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                foreach (Sprite sprite in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>())
                {
                    CozyTileSeason spriteSeason = ResolveSeason(path, sprite.name, sprite.rect.center.x, sprite.texture.width);
                    result.Add(new SpriteEntry(sprite, path, spriteSeason));
                }
            }

            return result
                .OrderBy(entry => entry.Season)
                .ThenBy(entry => entry.AssetPath, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(entry => entry.Sprite.rect.y)
                .ThenBy(entry => entry.Sprite.rect.x)
                .ToList();
        }

        public static void PrepareAllSheets()
        {
            if (!AssetDatabase.IsValidFolder(TileSourceRoot))
            {
                EditorUtility.DisplayDialog(
                    "Seasonal tiles",
                    $"Folder not found: {TileSourceRoot}\nImport the Cozy Farm full pack first.",
                    "OK");
                return;
            }

            string[] paths = AssetDatabase.FindAssets("t:Texture2D", new[] { TileSourceRoot })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            try
            {
                for (int index = 0; index < paths.Length; index++)
                {
                    EditorUtility.DisplayProgressBar(
                        "Preparing Cozy seasonal tiles",
                        paths[index],
                        paths.Length == 0 ? 1f : (float)index / paths.Length);
                    SliceSheet(paths[index]);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log($"Prepared {paths.Length} Cozy Farm tile sheet(s) as non-empty {TileSize}×{TileSize} sprites.");
        }

        private static void SliceSheet(string path)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.isReadable = true;
            importer.SaveAndReimport();

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null) return;

            Color32[] pixels = texture.GetPixels32();
            int columns = texture.width / TileSize;
            int rows = texture.height / TileSize;
            var metadata = new List<SpriteMetaData>();
            string sheetName = Sanitize(Path.GetFileNameWithoutExtension(path));

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (!HasVisiblePixel(pixels, texture.width, column, row)) continue;

                    float centerX = column * TileSize + TileSize * 0.5f;
                    CozyTileSeason cellSeason = ResolveSeason(path, string.Empty, centerX, texture.width);
                    metadata.Add(new SpriteMetaData
                    {
                        name = $"{cellSeason.ToString().ToLowerInvariant()}_{sheetName}_{column:D3}_{row:D3}",
                        rect = new Rect(
                            column * TileSize,
                            row * TileSize,
                            TileSize,
                            TileSize),
                        alignment = (int)SpriteAlignment.Center,
                        pivot = new Vector2(0.5f, 0.5f),
                    });
                }
            }

#pragma warning disable CS0618
            importer.spritesheet = metadata.ToArray();
#pragma warning restore CS0618
            importer.isReadable = false;
            importer.SaveAndReimport();
        }

        private static bool HasVisiblePixel(
            Color32[] pixels,
            int textureWidth,
            int column,
            int row)
        {
            int startX = column * TileSize;
            int startY = row * TileSize;
            for (int y = 0; y < TileSize; y++)
            {
                int offset = (startY + y) * textureWidth + startX;
                for (int x = 0; x < TileSize; x++)
                {
                    if (pixels[offset + x].a != 0) return true;
                }
            }
            return false;
        }

        public static void RebuildAllPalettes()
        {
            EnsureFolder(GeneratedRoot);
            EnsureFolder(PaletteRoot);
            List<SpriteEntry> allEntries = DiscoverSprites();
            foreach (CozyTileSeason value in Seasons)
            {
                BuildPalette(value, allEntries.Where(entry => entry.Season == value).ToArray());
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Rebuilt Cozy Farm seasonal palettes: Spring, Summer, Autumn and Winter.");
        }

        private static void BuildPalette(CozyTileSeason value, IReadOnlyList<SpriteEntry> seasonEntries)
        {
            string seasonName = value.ToString();
            string tileFolder = GeneratedRoot + "/" + seasonName + "/Tiles";
            EnsureFolder(GeneratedRoot + "/" + seasonName);
            EnsureFolder(tileFolder);

            var tiles = new List<Tile>();
            foreach (SpriteEntry entry in seasonEntries)
            {
                string sourceGuid = AssetDatabase.AssetPathToGUID(entry.AssetPath);
                string fileName = Sanitize(sourceGuid + "_" + entry.Sprite.name) + ".asset";
                string tilePath = tileFolder + "/" + fileName;
                Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
                if (tile == null)
                {
                    tile = CreateInstance<Tile>();
                    AssetDatabase.CreateAsset(tile, tilePath);
                }
                tile.name = entry.Sprite.name;
                tile.sprite = entry.Sprite;
                tile.colliderType = Tile.ColliderType.None;
                EditorUtility.SetDirty(tile);
                tiles.Add(tile);
            }

            GameObject palette = UnityTilePaletteBridge.CreateOrReplacePalette(
                PaletteRoot,
                "Cozy Farm - " + seasonName,
                tilemap => PopulatePalette(tilemap, tiles));
            if (palette != null) EditorUtility.SetDirty(palette);
        }

        private static void PopulatePalette(Tilemap tilemap, IReadOnlyList<Tile> tiles)
        {
            const int paletteColumns = 24;
            for (int index = 0; index < tiles.Count; index++)
            {
                int x = index % paletteColumns;
                int y = -(index / paletteColumns);
                tilemap.SetTile(new Vector3Int(x, y, 0), tiles[index]);
            }
            tilemap.CompressBounds();
        }

        private static void OpenPalette(CozyTileSeason value, string layerName)
        {
            string palettePath = PaletteRoot + "/Cozy Farm - " + value + ".prefab";
            GameObject palette = AssetDatabase.LoadAssetAtPath<GameObject>(palettePath);
            if (palette == null)
            {
                RebuildAllPalettes();
                palette = AssetDatabase.LoadAssetAtPath<GameObject>(palettePath);
            }

            Tilemap target = UnityEngine.Object.FindObjectsByType<Tilemap>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .FirstOrDefault(tilemap => string.Equals(
                    tilemap.name,
                    layerName,
                    StringComparison.OrdinalIgnoreCase));
            if (target == null)
            {
                EditorUtility.DisplayDialog(
                    "Seasonal tiles",
                    $"No Tilemap named '{layerName}' is loaded in the active scene.",
                    "OK");
                return;
            }

            if (palette == null || !UnityTilePaletteBridge.OpenAndActivate(palette, target))
            {
                EditorUtility.DisplayDialog(
                    "Seasonal tiles",
                    "Unity could not open or activate the requested Tile Palette.",
                    "OK");
            }
        }

        public static CozyTileSeason ResolveSeason(
            string assetPath,
            string spriteName,
            float horizontalCenter,
            float textureWidth)
        {
            string token = ((assetPath ?? string.Empty) + " " + (spriteName ?? string.Empty))
                .ToLowerInvariant();
            if (token.Contains("spring") || token.Contains("primavera")) return CozyTileSeason.Spring;
            if (token.Contains("summer") || token.Contains("verano")) return CozyTileSeason.Summer;
            if (token.Contains("autumn") || token.Contains("fall") || token.Contains("otono") || token.Contains("otoño"))
                return CozyTileSeason.Autumn;
            if (token.Contains("winter") || token.Contains("invierno") || token.Contains("snow"))
                return CozyTileSeason.Winter;

            float normalized = textureWidth <= 0f ? 0f : Mathf.Clamp01(horizontalCenter / textureWidth);
            if (normalized < 0.25f) return CozyTileSeason.Spring;
            if (normalized < 0.5f) return CozyTileSeason.Summer;
            if (normalized < 0.75f) return CozyTileSeason.Autumn;
            return CozyTileSeason.Winter;
        }

        private static string DisplayName(CozyTileSeason value)
        {
            switch (value)
            {
                case CozyTileSeason.Spring: return "Primavera";
                case CozyTileSeason.Summer: return "Verano";
                case CozyTileSeason.Autumn: return "Otoño";
                case CozyTileSeason.Winter: return "Invierno";
                default: return value.ToString();
            }
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }
                current = next;
            }
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value)) return "tile";
            char[] invalid = Path.GetInvalidFileNameChars();
            return new string(value.Select(character =>
                    invalid.Contains(character) || character == '/' || character == '\\'
                        ? '_'
                        : character)
                .ToArray());
        }

        private static string ToAbsolutePath(string assetPath)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty;
            return Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }

        public readonly struct SpriteEntry
        {
            public SpriteEntry(Sprite sprite, string assetPath, CozyTileSeason season)
            {
                Sprite = sprite;
                AssetPath = assetPath;
                Season = season;
            }

            public Sprite Sprite { get; }
            public string AssetPath { get; }
            public CozyTileSeason Season { get; }
            public string ShortName => Sprite == null ? "Missing" : Sprite.name;
        }
    }
}
