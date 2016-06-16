namespace WinTail.Actors
{
    using System;
    using Akka.Actor;
    using Messages.Error;
    using Messages.Neutral;
    using Messages.Success;

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
            Console.WriteLine("Write whatever you want into the console!");
            Console.Write("Some lines will appear as");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" red ");
            Console.ResetColor();
            Console.Write(" and others will appear as");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" green! ");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (string.IsNullOrEmpty(message))
            {
                Self.Tell(new NullInputError("No input received."));
            }
            else if (string.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                Context.System.Shutdown();
            }
            else
            {
                var valid = IsValid(message);
                if (valid)
                {
                    validationActor.Tell(new InputSuccess("Thank you! Message was valid."));

                    // Continue reading messages from console
                    Self.Tell(new ContinueProcessing());
                }
                else
                {
                    Self.Tell(new ValidationError("Invalid: Input had odd number of characters."));
                }
            }
        }

        private bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}