using FarmSimulator.Application;
using UnityEditor;
using UnityEditor.Build;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class ProjectIdentityConfigurator
    {
        static ProjectIdentityConfigurator()
        {
            EditorApplication.delayCall += EnsureProjectIdentity;
        }

        [MenuItem("Tools/Farm Simulator/Validate Project Identity")]
        public static void EnsureProjectIdentity()
        {
            bool changed = false;

            if (PlayerSettings.companyName != ProjectIdentity.CompanyName)
            {
                PlayerSettings.companyName = ProjectIdentity.CompanyName;
                changed = true;
            }

            if (PlayerSettings.productName != ProjectIdentity.ProductName)
            {
                PlayerSettings.productName = ProjectIdentity.ProductName;
                changed = true;
            }

            string currentIdentifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Standalone);
            if (currentIdentifier != ProjectIdentity.StandaloneApplicationIdentifier)
            {
                PlayerSettings.SetApplicationIdentifier(
                    NamedBuildTarget.Standalone,
                    ProjectIdentity.StandaloneApplicationIdentifier);
                changed = true;
            }

            if (changed)
            {
                AssetDatabase.SaveAssets();
            }
        }
    }
}
