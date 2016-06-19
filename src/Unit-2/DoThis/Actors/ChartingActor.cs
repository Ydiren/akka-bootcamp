namespace ChartApp.Actors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms.DataVisualization.Charting;
    using Akka.Actor;

    public class ChartingActor : UntypedActor
    {
        private readonly Chart chart;
        private Dictionary<string, Series> seriesIndex;

        public ChartingActor(Chart chart)
            : this(chart, new Dictionary<string, Series>())
        {
        }

        public ChartingActor(Chart chart, Dictionary<string, Series> seriesIndex)
        {
            this.chart = chart;
            this.seriesIndex = seriesIndex;
        }

        protected override void OnReceive(object message)
        {
            if (message is InitializeChart)
            {
                var ic = message as InitializeChart;
                HandleInitialize(ic);
            }
        }

        #region Individual Message Type Handlers

        private void HandleInitialize(InitializeChart ic)
        {
            if (ic.InitialSeries != null)
            {
                //swap the two series out
                seriesIndex = ic.InitialSeries;
            }

            //delete any existing series
            chart.Series.Clear();

            //attempt to render the initial chart
            if (seriesIndex.Any())
            {
                foreach (var series in seriesIndex)
                {
                    //force both the chart and the internal index to use the same names
                    series.Value.Name = series.Key;
                    chart.Series.Add(series.Value);
                }
            }
        }

        #endregion

        #region Messages

        public class InitializeChart
        {
            public InitializeChart(Dictionary<string, Series> initialSeries)
            {
                InitialSeries = initialSeries;
            }

            public Dictionary<string, Series> InitialSeries { get; }
        }

        #endregion
    }
}