using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FarmSimulator.Application.Player;
using FarmSimulator.Presentation.Player;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Networking;

namespace FarmSimulator.Editor
{
    [InitializeOnLoad]
    public static class FarmerSpriteAssetPipeline
    {
        public const string SourceUrl =
            "https://raw.githubusercontent.com/xArtXtrAx/farming-game-A/" +
            "dd32056c9f8142a2322bc2c1d41f0b05b002598f/" +
            "public/assets/farmer/farmer-spritesheet.png";

        public const string AssetRoot =
            "Assets/_Project/Resources/Characters/Farmer";

        public const string SpriteSheetAssetPath =
            AssetRoot + "/farmer-spritesheet.png";

        public const string AnimationFolder =
            AssetRoot + "/Animations";

        public const string ControllerAssetPath =
            AnimationFolder + "/FarmerAnimator.controller";

        public const string ImportSignature =
            "farm-simulator-farmer-64x72-v1";

        private static bool downloadInProgress;

        static FarmerSpriteAssetPipeline()
        {
            EditorApplication.delayCall += EnsureAssets;
        }

        [MenuItem("Tools/Farm Simulator/Rebuild Farmer Sprite Assets")]
        public static void RebuildAssets()
        {
            DeleteGeneratedAnimationAssets();
            TextureImporter importer =
                AssetImporter.GetAtPath(SpriteSheetAssetPath) as TextureImporter;
            if (importer != null)
            {
                importer.userData = string.Empty;
                importer.SaveAndReimport();
            }

            EditorApplication.delayCall += EnsureAssets;
        }

        public static void EnsureAssets()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += EnsureAssets;
                return;
            }

            if (!File.Exists(SpriteSheetAssetPath))
            {
                BeginDownload();
                return;
            }

            if (ConfigureImporterIfNeeded())
            {
                return;
            }

            BuildAnimationAssetsIfNeeded();
        }

        public static string ClipAssetPath(PlayerAnimationState state)
        {
            return AnimationFolder + "/" +
                PlayerAnimationModel.StateName(state) + ".anim";
        }

        private static void BeginDownload()
        {
            if (downloadInProgress)
            {
                return;
            }

            Directory.CreateDirectory(AssetRoot);
            downloadInProgress = true;

            UnityWebRequest request = UnityWebRequest.Get(SourceUrl);
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            operation.completed += _ =>
            {
                try
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError(
                            "Could not download the frozen farmer spritesheet: " +
                            request.error);
                        return;
                    }

                    File.WriteAllBytes(
                        SpriteSheetAssetPath,
                        request.downloadHandler.data);
                    AssetDatabase.ImportAsset(
                        SpriteSheetAssetPath,
                        ImportAssetOptions.ForceSynchronousImport |
                        ImportAssetOptions.ForceUpdate);
                    Debug.Log(
                        "Downloaded the frozen farmer spritesheet from " +
                        "farming-game-A.");
                }
                finally
                {
                    request.Dispose();
                    downloadInProgress = false;
                    EditorApplication.delayCall += EnsureAssets;
                }
            };
        }

        private static bool ConfigureImporterIfNeeded()
        {
            TextureImporter importer =
                AssetImporter.GetAtPath(SpriteSheetAssetPath) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(
                    SpriteSheetAssetPath,
                    ImportAssetOptions.ForceSynchronousImport);
                EditorApplication.delayCall += EnsureAssets;
                return true;
            }

            bool alreadyConfigured =
                importer.userData == ImportSignature &&
                importer.textureType == TextureImporterType.Sprite &&
                importer.spriteImportMode == SpriteImportMode.Multiple &&
                Mathf.Approximately(
                    importer.spritePixelsPerUnit,
                    PlayerAnimationModel.PixelsPerUnit) &&
                importer.filterMode == FilterMode.Point &&
                !importer.mipmapEnabled &&
                importer.textureCompression ==
                    TextureImporterCompression.Uncompressed &&
                AssetDatabase.LoadAllAssetRepresentationsAtPath(
                    SpriteSheetAssetPath).OfType<Sprite>().Count() ==
                    PlayerAnimationModel.Columns * PlayerAnimationModel.Rows;

            if (alreadyConfigured)
            {
                return false;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = PlayerAnimationModel.PixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression =
                TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.userData = ImportSignature;

#pragma warning disable 0618
            importer.spritesheet = CreateSpriteMetadata();
#pragma warning restore 0618

            importer.SaveAndReimport();
            EditorApplication.delayCall += EnsureAssets;
            return true;
        }

        private static SpriteMetaData[] CreateSpriteMetadata()
        {
            var metadata = new List<SpriteMetaData>(
                PlayerAnimationModel.Columns * PlayerAnimationModel.Rows);

            for (int row = 0; row < PlayerAnimationModel.Rows; row++)
            {
                for (int column = 0;
                     column < PlayerAnimationModel.Columns;
                     column++)
                {
                    int frameIndex =
                        row * PlayerAnimationModel.Columns + column;
                    int unityRow =
                        PlayerAnimationModel.Rows - 1 - row;

                    metadata.Add(new SpriteMetaData
                    {
                        name = PlayerAnimationModel.SpriteName(frameIndex),
                        rect = new Rect(
                            column * PlayerAnimationModel.FrameWidthPixels,
                            unityRow * PlayerAnimationModel.FrameHeightPixels,
                            PlayerAnimationModel.FrameWidthPixels,
                            PlayerAnimationModel.FrameHeightPixels),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(
                            PlayerAnimationModel.PivotNormalizedX,
                            PlayerAnimationModel.PivotNormalizedY),
                    });
                }
            }

            return metadata.ToArray();
        }

        private static void BuildAnimationAssetsIfNeeded()
        {
            bool allClipsExist = Enum
                .GetValues(typeof(PlayerAnimationState))
                .Cast<PlayerAnimationState>()
                .All(state =>
                    AssetDatabase.LoadAssetAtPath<AnimationClip>(
                        ClipAssetPath(state)) != null);

            if (allClipsExist &&
                AssetDatabase.LoadAssetAtPath<AnimatorController>(
                    ControllerAssetPath) != null)
            {
                return;
            }

            Directory.CreateDirectory(AnimationFolder);
            AssetDatabase.Refresh();

            Dictionary<string, Sprite> sprites =
                AssetDatabase.LoadAllAssetRepresentationsAtPath(
                    SpriteSheetAssetPath)
                .OfType<Sprite>()
                .ToDictionary(sprite => sprite.name);

            if (sprites.Count !=
                PlayerAnimationModel.Columns * PlayerAnimationModel.Rows)
            {
                Debug.LogError(
                    $"Expected 12 farmer sprites but imported {sprites.Count}.");
                return;
            }

            DeleteGeneratedAnimationAssets();

            var clips = new Dictionary<PlayerAnimationState, AnimationClip>();
            foreach (PlayerAnimationState state in
                     Enum.GetValues(typeof(PlayerAnimationState)))
            {
                clips[state] = CreateClip(state, sprites);
            }

            AnimatorController controller =
                AnimatorController.CreateAnimatorControllerAtPath(
                    ControllerAssetPath);
            AnimatorStateMachine stateMachine =
                controller.layers[0].stateMachine;

            foreach (PlayerAnimationState state in
                     Enum.GetValues(typeof(PlayerAnimationState)))
            {
                AnimatorState animatorState = stateMachine.AddState(
                    PlayerAnimationModel.StateName(state));
                animatorState.motion = clips[state];

                if (state == PlayerAnimationState.IdleDown)
                {
                    stateMachine.defaultState = animatorState;
                }
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(
                "Generated farmer idle/walk clips and Animator controller.");
        }

        private static AnimationClip CreateClip(
            PlayerAnimationState state,
            IReadOnlyDictionary<string, Sprite> sprites)
        {
            int[] frameIndices = PlayerAnimationModel.Frames(state);
            bool loop = PlayerAnimationModel.Loops(state);
            float frameDuration = 1f / PlayerAnimationModel.FrameRate;

            var keyframes = new List<ObjectReferenceKeyframe>(
                frameIndices.Length + 1);
            for (int index = 0; index < frameIndices.Length; index++)
            {
                keyframes.Add(new ObjectReferenceKeyframe
                {
                    time = index * frameDuration,
                    value = sprites[
                        PlayerAnimationModel.SpriteName(
                            frameIndices[index])],
                });
            }

            keyframes.Add(new ObjectReferenceKeyframe
            {
                time = frameIndices.Length * frameDuration,
                value = sprites[
                    PlayerAnimationModel.SpriteName(frameIndices[0])],
            });

            var clip = new AnimationClip
            {
                name = PlayerAnimationModel.StateName(state),
                frameRate = PlayerAnimationModel.FrameRate,
            };

            var binding = new EditorCurveBinding
            {
                path = string.Empty,
                type = typeof(SpriteRenderer),
                propertyName = "m_Sprite",
            };
            AnimationUtility.SetObjectReferenceCurve(
                clip,
                binding,
                keyframes.ToArray());

            AnimationClipSettings settings =
                AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            AssetDatabase.CreateAsset(clip, ClipAssetPath(state));
            return clip;
        }

        private static void DeleteGeneratedAnimationAssets()
        {
            foreach (PlayerAnimationState state in
                     Enum.GetValues(typeof(PlayerAnimationState)))
            {
                AssetDatabase.DeleteAsset(ClipAssetPath(state));
            }

            AssetDatabase.DeleteAsset(ControllerAssetPath);
        }
    }
}
