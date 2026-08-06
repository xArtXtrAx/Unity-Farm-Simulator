using System.Linq;
using FarmSimulator.Application.Scenes;
using FarmSimulator.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class CozyInteriorHouseArtTests
    {
        [Test]
        public void CuratesHouseSpritesFromImportedInteriorSheets()
        {
            CozyInteriorHouseArtPipeline.EnsureAssets();

            string[] expected =
            {
                "cozy_interior_wall_cream",
                "cozy_interior_floor_wood",
                "cozy_interior_door_cream",
                "cozy_interior_bed_cream",
                "cozy_interior_rug_warm",
            };

            string[] actual =
                CozyInteriorHouseArtPipeline.LoadSprites()
                    .Keys
                    .OrderBy(name => name)
                    .ToArray();

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void CuratedInteriorSpritesUsePixelArtSettings()
        {
            string[] paths =
            {
                CozyInteriorHouseArtPipeline.WallpapersPath,
                CozyInteriorHouseArtPipeline.DoorsPath,
                CozyInteriorHouseArtPipeline.BedsPath,
                CozyInteriorHouseArtPipeline.RugsPath,
            };

            CozyInteriorHouseArtPipeline.EnsureAssets();

            foreach (string path in paths)
            {
                TextureImporter importer =
                    AssetImporter.GetAtPath(path) as TextureImporter;
                Assert.That(importer, Is.Not.Null);
                Assert.That(
                    importer.spriteImportMode,
                    Is.EqualTo(SpriteImportMode.Multiple));
                Assert.That(importer.spritePixelsPerUnit, Is.EqualTo(16f));
                Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point));
                Assert.That(importer.mipmapEnabled, Is.False);
            }
        }

        [Test]
        public void HouseSceneActuallyUsesCuratedInteriorSprites()
        {
            HouseAndSleepScenePipeline.EnsureScenes();
            CozyInteriorHouseArtPipeline.EnsureAssets();

            Assert.That(
                CozyInteriorHouseSceneUpgrader.ApplyToHouseScene(
                    force: true),
                Is.True);

            Scene scene = SceneManager.GetSceneByPath(
                ProjectSceneNames.HouseInteriorPath);
            bool openedHere = !scene.IsValid() || !scene.isLoaded;
            if (openedHere)
            {
                scene = EditorSceneManager.OpenScene(
                    ProjectSceneNames.HouseInteriorPath,
                    OpenSceneMode.Additive);
            }

            try
            {
                Assert.That(
                    Find(scene, CozyInteriorHouseSceneUpgrader.MarkerName),
                    Is.Not.Null);

                SpriteRenderer floor = Find(scene, "Wood Floor")
                    .GetComponentsInChildren<SpriteRenderer>(true)
                    .First();
                Assert.That(
                    floor.sprite.name,
                    Is.EqualTo("cozy_interior_floor_wood"));

                SpriteRenderer door = Find(scene, "Interior Door")
                    .GetComponent<SpriteRenderer>();
                Assert.That(
                    door.sprite.name,
                    Is.EqualTo("cozy_interior_door_cream"));

                Transform bedVisual = Find(scene, "Cozy Interior Bed");
                Assert.That(bedVisual, Is.Not.Null);
                Assert.That(
                    bedVisual.GetComponent<SpriteRenderer>().sprite.name,
                    Is.EqualTo("cozy_interior_bed_cream"));
            }
            finally
            {
                if (openedHere && scene.IsValid() && scene.isLoaded)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        private static Transform Find(Scene scene, string objectName)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Transform result = root
                    .GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(
                        candidate => candidate.name == objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
