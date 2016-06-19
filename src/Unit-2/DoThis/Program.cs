namespace ChartApp
{
    using System;
    using System.Windows.Forms;
    using Akka.Actor;

    internal static class Program
    {
        /// <summary>
        ///     ActorSystem we'll be using to publish data to charts
        ///     and subscribe from performance counters
        /// </summary>
        public static ActorSystem ChartActors;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ChartActors = ActorSystem.Create("ChartActors");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}