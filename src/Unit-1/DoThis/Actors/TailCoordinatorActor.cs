namespace WinTail.Actors
{
    using System;
    using Akka.Actor;

    class TailCoordinatorActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is StartTail)
            {
                var msg = message as StartTail;

                // Here we are creating our first parent/child reationship!
                // The TailActor instance created here is a child
                // of this instance of TailCoordinatorActor
                Context.ActorOf(Props.Create(() => new TailActor(msg.ReporterActor, msg.FilePath)));
            }
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10,
                TimeSpan.FromSeconds(30),
                exception =>
                {
                    // Maybe we consider ArithmeticException to not be application critical
                    // So we just ignore the error and keep going
                    if (exception is ArithmeticException)
                    {
                        return Directive.Resume;
                    }

                    // Error that we cannot recover from, stop the failing actor
                    if (exception is NotSupportedException)
                    {
                        return Directive.Stop;
                    }

                    // In all other cases, just restart the failing actor
                    return Directive.Restart;
                });
        }

        /// <summary>
        ///     Start tailing the file at user-specified path.
        /// </summary>
        public class StartTail
        {
            public StartTail(string filePath, IActorRef reporterActor)
            {
                FilePath = filePath;
                ReporterActor = reporterActor;
            }

            public string FilePath { get; }

            public IActorRef ReporterActor { get; }
        }

        public class StopTail
        {
            public StopTail(string filePath)
            {
                FilePath = filePath;
            }

            public string FilePath { get; private set; }
        }
    }
}