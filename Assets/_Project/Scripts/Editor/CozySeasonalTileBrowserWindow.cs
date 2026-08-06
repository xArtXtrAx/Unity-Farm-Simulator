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
        public const string PaletteRoot = GeneratedRoot + "/Palettes";
        public const int TileSize = 16;

        private static readonly CozyTileSeason[] Seasons =
        {
            CozyTileSeason.Spring,
            CozyTileSeason.Summer,
            CozyTileSeason.Autumn,
            CozyTileSeason.Winter,
        };

        private static readonly string[] LayerNames =
        {
            "Ground",
            "Paths",
            "Soil",
            "Decoration",
        };

        private CozyTileSeason season;
        private int paintLayerIndex;
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

        private void OnEnable() => Reload();

        private void OnGUI()
        {
            EditorGUILayout.LabelField(
                "Farm Development Kit — Cozy Seasonal Tiles",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Slices every non-empty 16×16 cell under CozyFarm/Full/Tiles, " +
                "separates the complete atlas into four seasons and builds four Unity Tile Palettes.",
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
                if (GUILayout.Button("Refresh", GUILayout.Height(28f))) Reload();
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
                paintLayerIndex = EditorGUILayout.Popup(
                    "Paint target",
                    Mathf.Clamp(paintLayerIndex, 0, LayerNames.Length - 1),
                    LayerNames);
                if (GUILayout.Button("Open season palette", GUILayout.Width(170f)))
                {
                    OpenPalette(season, LayerNames[paintLayerIndex]);
                }
            }

            thumbnailSize = EditorGUILayout.Slider("Thumbnail size", thumbnailSize, 36f, 96f);
            List<SpriteEntry> visible = entries.Where(entry => entry.Season == season).ToList();
            EditorGUILayout.LabelField(
                $"{DisplayName(season)}: {visible.Count} non-empty tiles",
                EditorStyles.miniBoldLabel);

            if (visible.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    AssetDatabase.IsValidFolder(TileSourceRoot)
                        ? "No sliced sprites were found. Run 'Prepare / reslice all tile sheets'."
                        : $"Folder not found: {TileSourceRoot}. Import the full Cozy Farm pack first.",
                    MessageType.Warning);
                return;
            }

            float cellWidth = thumbnailSize + 12f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((position.width - 24f) / cellWidth));
            scroll = EditorGUILayout.BeginScrollView(scroll);
            for (int index = 0; index < visible.Count;)
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

        private void DrawSpriteButton(SpriteEntry entry)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(thumbnailSize + 8f)))
            {
                Rect previewRect = GUILayoutUtility.GetRect(
                    thumbnailSize,
                    thumbnailSize,
                    GUILayout.Width(thumbnailSize),
                    GUILayout.Height(thumbnailSize));

                DrawCheckerBackground(previewRect);
                DrawSpritePreview(entry.Sprite, previewRect);

                if (GUI.Button(previewRect, GUIContent.none, GUIStyle.none))
                {
                    Selection.activeObject = entry.Sprite;
                    EditorGUIUtility.PingObject(entry.Sprite);
                }

                string label = entry.Sprite.name;
                var content = new GUIContent(label, label);
                GUILayout.Label(
                    content,
                    EditorStyles.centeredGreyMiniLabel,
                    GUILayout.Width(thumbnailSize),
                    GUILayout.Height(30f));
            }
        }

        private static void DrawSpritePreview(Sprite sprite, Rect destination)
        {
            if (sprite == null || sprite.texture == null) return;

            Rect textureRect = sprite.textureRect;
            Texture2D texture = sprite.texture;
            Rect uv = new Rect(
                textureRect.x / texture.width,
                textureRect.y / texture.height,
                textureRect.width / texture.width,
                textureRect.height / texture.height);

            float sourceAspect = textureRect.width / Mathf.Max(1f, textureRect.height);
            Rect fitted = destination;
            if (sourceAspect > 1f)
            {
                fitted.height = destination.width / sourceAspect;
                fitted.y += (destination.height - fitted.height) * 0.5f;
            }
            else
            {
                fitted.width = destination.height * sourceAspect;
                fitted.x += (destination.width - fitted.width) * 0.5f;
            }

            GUI.DrawTextureWithTexCoords(fitted, texture, uv, true);
        }

        private static void DrawCheckerBackground(Rect rect)
        {
            const float square = 8f;
            Color light = EditorGUIUtility.isProSkin
                ? new Color(0.24f, 0.24f, 0.24f, 1f)
                : new Color(0.78f, 0.78f, 0.78f, 1f);
            Color dark = EditorGUIUtility.isProSkin
                ? new Color(0.18f, 0.18f, 0.18f, 1f)
                : new Color(0.65f, 0.65f, 0.65f, 1f);

            for (float y = rect.y; y < rect.yMax; y += square)
            {
                int row = Mathf.FloorToInt((y - rect.y) / square);
                for (float x = rect.x; x < rect.xMax; x += square)
                {
                    int column = Mathf.FloorToInt((x - rect.x) / square);
                    Rect cell = new Rect(
                        x,
                        y,
                        Mathf.Min(square, rect.xMax - x),
                        Mathf.Min(square, rect.yMax - y));
                    EditorGUI.DrawRect(cell, ((row + column) & 1) == 0 ? light : dark);
                }
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
                    result.Add(new SpriteEntry(
                        sprite,
                        path,
                        ResolveSeason(path, sprite.name, sprite.rect.center.x, sprite.texture.width)));
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
            Debug.Log($"Prepared {paths.Length} Cozy Farm seasonal tile sheet(s).");
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
            string sheetName = Sanitize(Path.GetFileNameWithoutExtension(path));
            var metadata = new List<SpriteMetaData>();

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (!HasVisiblePixel(pixels, texture.width, column, row)) continue;
                    float centerX = column * TileSize + TileSize * 0.5f;
                    CozyTileSeason tileSeason = ResolveSeason(path, string.Empty, centerX, texture.width);
                    metadata.Add(new SpriteMetaData
                    {
                        name = $"{tileSeason.ToString().ToLowerInvariant()}_{sheetName}_{column:D3}_{row:D3}",
                        rect = new Rect(column * TileSize, row * TileSize, TileSize, TileSize),
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

        private static bool HasVisiblePixel(Color32[] pixels, int width, int column, int row)
        {
            int startX = column * TileSize;
            int startY = row * TileSize;
            for (int y = 0; y < TileSize; y++)
            {
                int offset = (startY + y) * width + startX;
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
            List<SpriteEntry> all = DiscoverSprites();
            foreach (CozyTileSeason value in Seasons)
            {
                BuildPalette(value, all.Where(entry => entry.Season == value).ToArray());
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Rebuilt Cozy Farm palettes for all four seasons.");
        }

        private static void BuildPalette(CozyTileSeason value, IReadOnlyList<SpriteEntry> seasonEntries)
        {
            string seasonName = value.ToString();
            string seasonRoot = GeneratedRoot + "/" + seasonName;
            string tileFolder = seasonRoot + "/Tiles";
            EnsureFolder(seasonRoot);
            EnsureFolder(tileFolder);

            var tiles = new List<Tile>();
            foreach (SpriteEntry entry in seasonEntries)
            {
                string guid = AssetDatabase.AssetPathToGUID(entry.AssetPath);
                string tilePath = tileFolder + "/" + Sanitize(guid + "_" + entry.Sprite.name) + ".asset";
                Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
                if (tile == null)
                {
                    tile = ScriptableObject.CreateInstance<Tile>();
                    AssetDatabase.CreateAsset(tile, tilePath);
                }
                tile.name = entry.Sprite.name;
                tile.sprite = entry.Sprite;
                tile.colliderType = Tile.ColliderType.None;
                EditorUtility.SetDirty(tile);
                tiles.Add(tile);
            }

            UnityTilePaletteBridge.CreateOrReplacePalette(
                PaletteRoot,
                "Cozy Farm - " + seasonName,
                tilemap => PopulatePalette(tilemap, tiles));
        }

        private static void PopulatePalette(Tilemap tilemap, IReadOnlyList<Tile> tiles)
        {
            const int paletteColumns = 24;
            for (int index = 0; index < tiles.Count; index++)
            {
                tilemap.SetTile(
                    new Vector3Int(index % paletteColumns, -(index / paletteColumns), 0),
                    tiles[index]);
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
                    $"No Tilemap named '{layerName}' is loaded in the current scene.",
                    "OK");
                return;
            }

            if (palette == null || !UnityTilePaletteBridge.OpenAndActivate(palette, target))
            {
                EditorUtility.DisplayDialog(
                    "Seasonal tiles",
                    "Unity could not activate the requested Tile Palette.",
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

            float normalized = textureWidth <= 0f
                ? 0f
                : Mathf.Clamp01(horizontalCenter / textureWidth);
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
        }
    }
}
