using System;
using System.Linq;
using FarmSimulator.Application.Player;
using FarmSimulator.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class FarmerSpriteAssetPipelineTests
    {
        [Test]
        public void FarmerTextureUsesPixelArtImportSettings()
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(
                    FarmerSpriteAssetPipeline.SpriteSheetAssetPath)
                as TextureImporter;

            Assert.That(importer, Is.Not.Null);
            Assert.That(
                importer.userData,
                Is.EqualTo(FarmerSpriteAssetPipeline.ImportSignature));
            Assert.That(
                importer.textureType,
                Is.EqualTo(TextureImporterType.Sprite));
            Assert.That(
                importer.spriteImportMode,
                Is.EqualTo(SpriteImportMode.Multiple));
            Assert.That(importer.filterMode, Is.EqualTo(FilterMode.Point));
            Assert.That(importer.mipmapEnabled, Is.False);
            Assert.That(
                importer.textureCompression,
                Is.EqualTo(TextureImporterCompression.Uncompressed));
            Assert.That(
                importer.spritePixelsPerUnit,
                Is.EqualTo(PlayerAnimationModel.PixelsPerUnit)
                    .Within(0.001f));

            Sprite[] sprites =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(
                    FarmerSpriteAssetPipeline.SpriteSheetAssetPath)
                .OfType<Sprite>()
                .ToArray();

            Assert.That(sprites, Has.Length.EqualTo(12));
            Assert.That(
                sprites.Select(sprite => sprite.name),
                Is.Unique);
            Assert.That(
                sprites.All(sprite =>
                    Mathf.Approximately(
                        sprite.rect.width,
                        PlayerAnimationModel.FrameWidthPixels) &&
                    Mathf.Approximately(
                        sprite.rect.height,
                        PlayerAnimationModel.FrameHeightPixels)),
                Is.True);
        }

        [Test]
        public void FarmerAnimatorContainsAllIdleAndWalkStates()
        {
            AnimatorController controller =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(
                    FarmerSpriteAssetPipeline.ControllerAssetPath);
            Assert.That(controller, Is.Not.Null);

            string[] stateNames = controller.layers[0]
                .stateMachine.states
                .Select(childState => childState.state.name)
                .ToArray();

            foreach (PlayerAnimationState state in
                     Enum.GetValues(typeof(PlayerAnimationState)))
            {
                string stateName = PlayerAnimationModel.StateName(state);
                Assert.That(stateNames, Does.Contain(stateName));

                AnimationClip clip =
                    AssetDatabase.LoadAssetAtPath<AnimationClip>(
                        FarmerSpriteAssetPipeline.ClipAssetPath(state));
                Assert.That(clip, Is.Not.Null);
                Assert.That(
                    clip.frameRate,
                    Is.EqualTo(PlayerAnimationModel.FrameRate)
                        .Within(0.001f));
            }
        }
    }
}
