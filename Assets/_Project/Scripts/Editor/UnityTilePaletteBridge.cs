using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FarmSimulator.Editor
{
    /// <summary>
    /// Small compatibility bridge around the 2D Tilemap Editor package.
    /// Reflection keeps the project resilient across minor package API changes.
    /// </summary>
    public static class UnityTilePaletteBridge
    {
        private const string PaletteUtilityTypeName =
            "UnityEditor.Tilemaps.GridPaletteUtility";
        private const string PaintingStateTypeName =
            "UnityEditor.Tilemaps.GridPaintingState";

        public static bool IsAvailable =>
            FindType(PaletteUtilityTypeName) != null &&
            FindType(PaintingStateTypeName) != null;

        public static GameObject CreateOrReplacePalette(
            string folderPath,
            string paletteName,
            Action<Tilemap> populate)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentException(
                    "Palette folder cannot be empty.",
                    nameof(folderPath));
            }
            if (string.IsNullOrWhiteSpace(paletteName))
            {
                throw new ArgumentException(
                    "Palette name cannot be empty.",
                    nameof(paletteName));
            }
            if (populate == null)
            {
                throw new ArgumentNullException(nameof(populate));
            }

            string palettePath = folderPath + "/" + paletteName + ".prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(palettePath) != null)
            {
                AssetDatabase.DeleteAsset(palettePath);
            }

            Type utilityType = FindType(PaletteUtilityTypeName);
            if (utilityType == null)
            {
                throw new InvalidOperationException(
                    "2D Tilemap Editor is unavailable. Install com.unity.2d.tilemap.");
            }

            MethodInfo createMethod = utilityType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(method => method.Name == "CreateNewPalette");
            if (createMethod == null)
            {
                throw new MissingMethodException(
                    PaletteUtilityTypeName,
                    "CreateNewPalette");
            }

            object[] arguments = BuildCreateArguments(
                createMethod,
                folderPath,
                paletteName);
            createMethod.Invoke(null, arguments);

            GameObject paletteAsset =
                AssetDatabase.LoadAssetAtPath<GameObject>(palettePath);
            if (paletteAsset == null)
            {
                throw new InvalidOperationException(
                    $"Unity did not create the palette at '{palettePath}'.");
            }

            GameObject contents = PrefabUtility.LoadPrefabContents(palettePath);
            try
            {
                Tilemap tilemap = contents.GetComponentInChildren<Tilemap>(true);
                if (tilemap == null)
                {
                    var child = new GameObject(
                        "Palette",
                        typeof(Tilemap),
                        typeof(TilemapRenderer));
                    child.transform.SetParent(contents.transform, false);
                    tilemap = child.GetComponent<Tilemap>();
                }

                tilemap.ClearAllTiles();
                populate(tilemap);
                PrefabUtility.SaveAsPrefabAsset(contents, palettePath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(contents);
            }

            AssetDatabase.ImportAsset(
                palettePath,
                ImportAssetOptions.ForceSynchronousImport);
            return AssetDatabase.LoadAssetAtPath<GameObject>(palettePath);
        }

        public static bool OpenAndActivate(
            GameObject paletteAsset,
            Tilemap paintTarget)
        {
            if (paletteAsset == null)
            {
                throw new ArgumentNullException(nameof(paletteAsset));
            }
            if (paintTarget == null)
            {
                throw new ArgumentNullException(nameof(paintTarget));
            }

            if (!EditorApplication.ExecuteMenuItem("Window/2D/Tile Palette"))
            {
                return false;
            }

            Selection.activeGameObject = paintTarget.gameObject;
            EditorGUIUtility.PingObject(paintTarget.gameObject);

            Type stateType = FindType(PaintingStateTypeName);
            if (stateType == null)
            {
                return false;
            }

            SetStaticProperty(stateType, "palette", paletteAsset);
            SetStaticProperty(stateType, "scenePaintTarget", paintTarget.gameObject);
            SetStaticProperty(stateType, "scenePaintTarget", paintTarget);
            SceneView.RepaintAll();
            return true;
        }

        private static object[] BuildCreateArguments(
            MethodInfo method,
            string folderPath,
            string paletteName)
        {
            ParameterInfo[] parameters = method.GetParameters();
            var arguments = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                ParameterInfo parameter = parameters[index];
                Type type = parameter.ParameterType;
                string name = parameter.Name ?? string.Empty;

                if (type == typeof(string))
                {
                    arguments[index] = name.IndexOf(
                            "folder",
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf(
                            "path",
                            StringComparison.OrdinalIgnoreCase) >= 0
                        ? folderPath
                        : paletteName;
                }
                else if (type == typeof(GridLayout.CellLayout))
                {
                    arguments[index] = GridLayout.CellLayout.Rectangle;
                }
                else if (type == typeof(GridLayout.CellSwizzle))
                {
                    arguments[index] = GridLayout.CellSwizzle.XYZ;
                }
                else if (type == typeof(Vector3))
                {
                    arguments[index] = Vector3.one;
                }
                else if (type.IsEnum)
                {
                    string[] names = Enum.GetNames(type);
                    string automatic = names.FirstOrDefault(value =>
                        string.Equals(
                            value,
                            "Automatic",
                            StringComparison.OrdinalIgnoreCase));
                    arguments[index] = Enum.Parse(
                        type,
                        automatic ?? names[0]);
                }
                else if (parameter.HasDefaultValue)
                {
                    arguments[index] = parameter.DefaultValue;
                }
                else if (type.IsValueType)
                {
                    arguments[index] = Activator.CreateInstance(type);
                }
                else
                {
                    arguments[index] = null;
                }
            }

            return arguments;
        }

        private static void SetStaticProperty(
            Type type,
            string propertyName,
            object value)
        {
            PropertyInfo property = type.GetProperty(
                propertyName,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static);
            if (property == null || !property.CanWrite)
            {
                return;
            }

            Type targetType = property.PropertyType;
            if (value != null && !targetType.IsInstanceOfType(value))
            {
                return;
            }

            property.SetValue(null, value);
        }

        private static Type FindType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName, false);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
