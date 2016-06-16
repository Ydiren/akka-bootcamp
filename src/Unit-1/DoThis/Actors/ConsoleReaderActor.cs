namespace WinTail.Actors
{
    using System;
    using Akka.Actor;
    using Messages.Error;

    /// <summary>
    ///     Actor responsible for reading FROM the console.
    ///     Also responsible for calling <see cref="ActorSystem.Shutdown" />.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";
        private readonly IActorRef validationActor;

        public ConsoleReaderActor(IActorRef validationActor)
        {
            this.validationActor = validationActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }
            else if (message is InputError)
            {
                validationActor.Tell(message as InputError);
            }

            GetAndValidateInput();
        }

        private void DoPrintInstructions()
        {
            Console.WriteLine("Please provide the URI of a log file on disk.\n");
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (!string.IsNullOrEmpty(message) && string.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // If user typed ExitCommand, shut down the entire actor
                // system (allows the process to exit)
                Context.System.Shutdown();
                return;
            }

            // Otherwise, just hand message off to validation actor
            validationActor.Tell(message);
        }
    }
}