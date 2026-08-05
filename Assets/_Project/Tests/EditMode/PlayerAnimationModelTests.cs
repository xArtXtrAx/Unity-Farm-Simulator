using System;
using System.Linq;
using FarmSimulator.Application.Player;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class PlayerAnimationModelTests
    {
        [Test]
        public void OriginalSpritesheetLayoutIsPreserved()
        {
            Assert.That(PlayerAnimationModel.FrameWidthPixels, Is.EqualTo(64));
            Assert.That(PlayerAnimationModel.FrameHeightPixels, Is.EqualTo(72));
            Assert.That(PlayerAnimationModel.Columns, Is.EqualTo(3));
            Assert.That(PlayerAnimationModel.Rows, Is.EqualTo(4));
            Assert.That(PlayerAnimationModel.FrameRate, Is.EqualTo(8));
            Assert.That(
                PlayerAnimationModel.PivotNormalizedY,
                Is.EqualTo(0.12f).Within(0.0001f));
        }

        [TestCase(FacingDirection.Down, false, PlayerAnimationState.IdleDown)]
        [TestCase(FacingDirection.Down, true, PlayerAnimationState.WalkDown)]
        [TestCase(FacingDirection.Left, false, PlayerAnimationState.IdleLeft)]
        [TestCase(FacingDirection.Left, true, PlayerAnimationState.WalkLeft)]
        [TestCase(FacingDirection.Right, false, PlayerAnimationState.IdleRight)]
        [TestCase(FacingDirection.Right, true, PlayerAnimationState.WalkRight)]
        [TestCase(FacingDirection.Up, false, PlayerAnimationState.IdleUp)]
        [TestCase(FacingDirection.Up, true, PlayerAnimationState.WalkUp)]
        public void ResolvesAnimationState(
            FacingDirection facing,
            bool moving,
            PlayerAnimationState expected)
        {
            Assert.That(
                PlayerAnimationModel.Resolve(facing, moving),
                Is.EqualTo(expected));
        }

        [Test]
        public void IdleFramesMatchFrozenPrototype()
        {
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.IdleDown),
                Is.EqualTo(new[] { 1 }));
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.IdleLeft),
                Is.EqualTo(new[] { 4 }));
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.IdleRight),
                Is.EqualTo(new[] { 7 }));
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.IdleUp),
                Is.EqualTo(new[] { 10 }));
        }

        [Test]
        public void WalkFramesMatchFrozenPrototype()
        {
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.WalkDown),
                Is.EqualTo(new[] { 0, 1, 2, 1 }));
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.WalkLeft),
                Is.EqualTo(new[] { 3, 4, 5, 4 }));
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.WalkRight),
                Is.EqualTo(new[] { 6, 7, 8, 7 }));
            Assert.That(
                PlayerAnimationModel.Frames(PlayerAnimationState.WalkUp),
                Is.EqualTo(new[] { 9, 10, 11, 10 }));
        }

        [Test]
        public void EveryFrameHasAStableUniqueSpriteName()
        {
            string[] names = Enumerable
                .Range(
                    0,
                    PlayerAnimationModel.Columns *
                    PlayerAnimationModel.Rows)
                .Select(PlayerAnimationModel.SpriteName)
                .ToArray();

            Assert.That(names, Is.Unique);
            Assert.That(names[0], Is.EqualTo("farmer_down_0"));
            Assert.That(names[11], Is.EqualTo("farmer_up_2"));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => PlayerAnimationModel.SpriteName(12));
        }
    }
}
