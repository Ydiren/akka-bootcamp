namespace ChartApp
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Windows.Forms.DataVisualization.Charting;
    using Actors;
    using Akka.Actor;
    using Akka.Util.Internal;

    public partial class Main : Form
    {
        private readonly AtomicCounter seriesCounter = new AtomicCounter(1);
        private IActorRef chartActor;

        public Main()
        {
            InitializeComponent();
        }

        #region Initialization

        private void Main_Load(object sender, EventArgs e)
        {
            chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
            var series = ChartDataHelper.RandomSeries("FakeSeries" + seriesCounter.GetAndIncrement());
            chartActor.Tell(new ChartingActor.InitializeChart(new Dictionary<string, Series>
            {
                {
                    series.Name, series
                }
            }));
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            chartActor.Tell(PoisonPill.Instance);

            //shut down the ActorSystem
            Program.ChartActors.Shutdown();
        }

        #endregion
    }
}