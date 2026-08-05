using System;
using FarmSimulator.Application.Player;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class PlayerMovementModelTests
    {
        [Test]
        public void InputInsideDeadZoneStopsAndPreservesFacing()
        {
            ProcessedMovement result = PlayerMovementModel.Process(
                0.1f,
                0.1f,
                FacingDirection.Left);

            Assert.That(result.IsMoving, Is.False);
            Assert.That(result.X, Is.Zero);
            Assert.That(result.Y, Is.Zero);
            Assert.That(result.Facing, Is.EqualTo(FacingDirection.Left));
        }

        [Test]
        public void AnalogMagnitudeIsRemappedAfterDeadZone()
        {
            ProcessedMovement result = PlayerMovementModel.Process(
                0.59f,
                0f,
                FacingDirection.Down);

            Assert.That(result.Strength, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(result.X, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(result.Facing, Is.EqualTo(FacingDirection.Right));
        }

        [Test]
        public void DiagonalDigitalInputHasNoSpeedAdvantage()
        {
            ProcessedMovement result = PlayerMovementModel.Process(
                1f,
                1f,
                FacingDirection.Down);

            double magnitude = Math.Sqrt(
                result.X * result.X + result.Y * result.Y);
            Assert.That(magnitude, Is.EqualTo(1d).Within(0.001d));
            Assert.That(result.Facing, Is.EqualTo(FacingDirection.Up));
        }

        [TestCase(-1f, 0.2f, FacingDirection.Left)]
        [TestCase(1f, 0.2f, FacingDirection.Right)]
        [TestCase(0.2f, -1f, FacingDirection.Down)]
        [TestCase(0.2f, 1f, FacingDirection.Up)]
        public void FacingUsesDominantAxis(
            float x,
            float y,
            FacingDirection expected)
        {
            Assert.That(
                PlayerMovementModel.ResolveFacing(
                    x,
                    y,
                    FacingDirection.Down),
                Is.EqualTo(expected));
        }
    }
}
