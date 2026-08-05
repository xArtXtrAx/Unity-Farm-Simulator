using System;
using FarmSimulator.Application.Spatial;
using NUnit.Framework;

namespace FarmSimulator.Tests.EditMode
{
    public sealed class TopDownSortingModelTests
    {
        [Test]
        public void LowerFeetRenderInFrontOfHigherFeet()
        {
            int lower = TopDownSortingModel.OrderForFeetY(-2d);
            int higher = TopDownSortingModel.OrderForFeetY(2d);

            Assert.That(lower, Is.GreaterThan(higher));
        }

        [Test]
        public void ZeroFeetUsesConfiguredBaseOrder()
        {
            Assert.That(
                TopDownSortingModel.OrderForFeetY(0d, 4500, 64),
                Is.EqualTo(4500));
        }

        [TestCase(1.004, 9900)]
        [TestCase(1.01, 9899)]
        [TestCase(-1.01, 10101)]
        public void QuantizesDepthDeterministically(
            double feetY,
            int expected)
        {
            Assert.That(
                TopDownSortingModel.OrderForFeetY(feetY),
                Is.EqualTo(expected));
        }

        [Test]
        public void RejectsNonPositivePrecision()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => TopDownSortingModel.OrderForFeetY(0d, 0, 0));
        }
    }
}
