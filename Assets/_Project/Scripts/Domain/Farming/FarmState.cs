using System;
using System.Collections.Generic;

namespace FarmSimulator.Domain.Farming
{
    public sealed class FarmState
    {
        private readonly Dictionary<string, FarmPlotState> plots =
            new Dictionary<string, FarmPlotState>(StringComparer.Ordinal);

        public int PlotCount => plots.Count;

        public FarmPlotState GetOrCreatePlot(string plotId)
        {
            if (string.IsNullOrWhiteSpace(plotId))
            {
                throw new ArgumentException(
                    "Plot id is required.",
                    nameof(plotId));
            }

            if (!plots.TryGetValue(plotId, out FarmPlotState state))
            {
                state = new FarmPlotState();
                plots.Add(plotId, state);
            }

            return state;
        }

        public bool TryGetPlot(string plotId, out FarmPlotState state)
        {
            if (string.IsNullOrWhiteSpace(plotId))
            {
                state = null;
                return false;
            }

            return plots.TryGetValue(plotId, out state);
        }

        public int AdvanceDay()
        {
            int changedPlots = 0;
            foreach (FarmPlotState plot in plots.Values)
            {
                if (plot.AdvanceDay())
                {
                    changedPlots++;
                }
            }

            return changedPlots;
        }

        public void Reset()
        {
            plots.Clear();
        }
    }
}
