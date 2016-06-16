namespace WinTail
{
    using System;
    using Akka.Actor;
    using Messages.Error;
    using Messages.Neutral;
    using Messages.Success;

    /// <summary>
    /// Actor that validates user input and signals result to others
    /// </summary>
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            this.consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            string msg = message as string;
            if (string.IsNullOrEmpty(msg))
            {
                // Signal that the user needs to supply an input
                consoleWriterActor.Tell(new NullInputError("No input received."));
            }
            else
            {
                var valid = IsValid(msg);
                if (valid)
                {
                    // Send success to console writer
                    consoleWriterActor.Tell(new InputSuccess("Thank you! Message was valid."));
                }
                else
                {
                    // Signal that the input was bad
                    consoleWriterActor.Tell(new ValidationError("Invalid: input had odd number of characters."));
                }
            }

            // Tell sender to continue doing its thing
            // (whatever that may be, this actor doesn't care)
            Sender.Tell(new ContinueProcessing());
        }

        private bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}