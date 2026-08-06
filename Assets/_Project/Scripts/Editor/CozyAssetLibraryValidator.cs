using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public static class CozyAssetLibraryValidator
    {
        [MenuItem("Tools/Farm Simulator/Validate Complete Cozy Asset Library")]
        public static void ValidateCompleteLibrary()
        {
            bool exteriorImported =
                AssetDatabase.LoadAssetAtPath<TextAsset>(
                    CozyFarmFullPackImporter.ManifestAssetPath) != null;
            bool interiorImported =
                AssetDatabase.LoadAssetAtPath<TextAsset>(
                    CozyInteriorFullPackImporter.ManifestAssetPath) != null &&
                CozyInteriorAssetCatalog.IsImported;

            string exteriorStatus = exteriorImported
                ? "Ready"
                : "Missing: import Full Cozy Farm Pack";
            string interiorStatus = interiorImported
                ? "Ready"
                : "Missing: import Full Cozy Interior Pack";

            EditorUtility.DisplayDialog(
                "Cozy asset library",
                $"Exterior pack: {exteriorStatus}\n" +
                $"Interior pack: {interiorStatus}\n\n" +
                (exteriorImported && interiorImported
                    ? "The complete exterior/interior asset library is ready " +
                      "for scene-generation pipelines."
                    : "Import the missing licensed ZIP from the Farm Simulator " +
                      "Tools menu."),
                "OK");
        }
    }
}
