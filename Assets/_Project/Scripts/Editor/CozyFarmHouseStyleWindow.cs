using System;
using UnityEditor;
using UnityEngine;

namespace FarmSimulator.Editor
{
    public sealed class CozyFarmHouseStyleWindow : EditorWindow
    {
        private const string SelectedVariantKey =
            "FarmSimulator.CozyFarm.SelectedHouseVariant";

        private int selectedIndex;

        public static string SelectedVariantId
        {
            get => EditorPrefs.GetString(
                SelectedVariantKey,
                CozyFarmBuildingCatalog.DefaultHouseId);
            set => EditorPrefs.SetString(SelectedVariantKey, value);
        }

        [MenuItem("Tools/Farm Simulator/House Style Selector")]
        public static void Open()
        {
            var window = GetWindow<CozyFarmHouseStyleWindow>();
            window.titleContent = new GUIContent("Cozy House Style");
            window.minSize = new Vector2(390f, 210f);
            window.Show();
        }

        private void OnEnable()
        {
            selectedIndex = FindSelectedIndex();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Cozy Farm House Style", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Select a full house extracted from buildings.png. Applying a style rebuilds only the generated facade and uses the variant metadata for scale, entrance, spawn, collider and sorting.",
                MessageType.Info);

            var labels = new string[CozyFarmBuildingCatalog.Houses.Count];
            for (int index = 0; index < labels.Length; index++)
            {
                labels[index] = CozyFarmBuildingCatalog.Houses[index].DisplayName;
            }

            selectedIndex = EditorGUILayout.Popup("House style", selectedIndex, labels);
            CozyFarmBuildingCatalog.HouseVariant variant =
                CozyFarmBuildingCatalog.Houses[selectedIndex];

            EditorGUILayout.Space();
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("Variant id", variant.Id);
                EditorGUILayout.RectIntField("Atlas region", variant.SourceRect);
                EditorGUILayout.Vector2Field("Door anchor", variant.DoorAnchor);
                EditorGUILayout.Vector2Field("Portal offset", variant.PortalOffset);
                EditorGUILayout.Vector2Field("Spawn offset", variant.SpawnOffset);
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Generate all sprites", GUILayout.Height(30f)))
                {
                    CozyFarmBuildingCatalog.EnsureAssets();
                }

                if (GUILayout.Button("Apply selected style", GUILayout.Height(30f)))
                {
                    SelectedVariantId = variant.Id;
                    CozyFarmHouseExteriorUpgrader.ApplyVariant(variant.Id);
                }
            }
        }

        private static int FindSelectedIndex()
        {
            string selectedId = SelectedVariantId;
            for (int index = 0; index < CozyFarmBuildingCatalog.Houses.Count; index++)
            {
                if (string.Equals(
                    CozyFarmBuildingCatalog.Houses[index].Id,
                    selectedId,
                    StringComparison.Ordinal))
                {
                    return index;
                }
            }

            return 0;
        }
    }
}
