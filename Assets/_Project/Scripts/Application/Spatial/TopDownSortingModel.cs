using System;

namespace FarmSimulator.Application.Spatial
{
    public static class TopDownSortingModel
    {
        public const int DefaultBaseOrder = 10000;
        public const int DefaultOrdersPerUnit = 100;

        public static int OrderForFeetY(
            double feetY,
            int baseOrder = DefaultBaseOrder,
            int ordersPerUnit = DefaultOrdersPerUnit)
        {
            if (ordersPerUnit <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ordersPerUnit),
                    ordersPerUnit,
                    "Orders per unit must be greater than zero.");
            }

            double depthOffset = Math.Round(
                feetY * ordersPerUnit,
                MidpointRounding.AwayFromZero);
            double order = baseOrder - depthOffset;

            if (order < int.MinValue || order > int.MaxValue)
            {
                throw new OverflowException(
                    "The calculated sorting order is outside Int32 range.");
            }

            return (int)order;
        }
    }
}
