using System.IO;
using UnityEditor;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Keeps placeholder Tile main-object names aligned with their filenames.
    /// Semantic roles are stored as asset labels instead of changing the main-object name.
    /// </summary>
    [InitializeOnLoad]
    public static class PlaceholderTileNameNormalizer
    {
        private const string TileRoot = "Assets/_Project/Art/Placeholder/Tiles";
        private const string SemanticLabelPrefix = "farm-semantic:";

        static PlaceholderTileNameNormalizer()
        {
            EditorApplication.delayCall += Normalize;
        }

        [MenuItem("Tools/Farm Simulator/Farm Development Kit/Free Placeholder Art/Normalize Tile Names")]
        public static void Normalize()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Normalize;
                return;
            }

            NormalizeTile("ground_grass", "tile.ground.grass");
            NormalizeTile("path_dirt", "tile.path.dirt");
            NormalizeTile("soil_tilled", "tile.soil.tilled");
            NormalizeTile("soil_wet", "tile.soil.wet");
            NormalizeTile("water_basic", "tile.water.basic");

            AssetDatabase.SaveAssets();
        }

        private static void NormalizeTile(string fileName, string semanticKey)
        {
            string path = $"{TileRoot}/{fileName}.asset";
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile == null)
            {
                return;
            }

            if (tile.name != fileName)
            {
                tile.name = fileName;
                EditorUtility.SetDirty(tile);
            }

            string[] existingLabels = AssetDatabase.GetLabels(tile);
            string semanticLabel = SemanticLabelPrefix + semanticKey;
            bool alreadyLabeled = false;
            for (int i = 0; i < existingLabels.Length; i++)
            {
                if (existingLabels[i] == semanticLabel)
                {
                    alreadyLabeled = true;
                    break;
                }
            }

            if (!alreadyLabeled)
            {
                var labels = new string[existingLabels.Length + 1];
                existingLabels.CopyTo(labels, 0);
                labels[labels.Length - 1] = semanticLabel;
                AssetDatabase.SetLabels(tile, labels);
            }
        }
    }
}
