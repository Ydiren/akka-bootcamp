namespace ChartApp.Actors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms.DataVisualization.Charting;
    using Akka.Actor;

    public class ChartingActor : ReceiveActor
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

            Receive<InitializeChart>(ic => HandleInitialize(ic));
            Receive<AddSeries>(addSeries => HandleAddSeries(addSeries));
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

        private void HandleAddSeries(AddSeries series)
        {
            if (!string.IsNullOrEmpty(series.Series.Name) && !seriesIndex.ContainsKey(series.Series.Name))
            {
                seriesIndex.Add(series.Series.Name, series.Series);
                chart.Series.Add(series.Series);
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

        public class AddSeries
        {
            public AddSeries(Series series)
            {
                Series = series;
            }

            public Series Series { get; private set; }
        }

        #endregion
    }
}