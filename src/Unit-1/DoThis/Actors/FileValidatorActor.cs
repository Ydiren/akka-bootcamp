namespace WinTail.Actors
{
    using System.IO;
    using Akka.Actor;
    using Messages.Error;
    using Messages.Neutral;
    using Messages.Success;

    /// <summary>
    /// Actor that validates user input and signals result to others
    /// </summary>
    public class FileValidatorActor : UntypedActor
    {
        private readonly IActorRef consoleWriterActor;
        private readonly IActorRef tailCoordinatorActor;

        public FileValidatorActor(IActorRef consoleWriterActor, IActorRef tailCoordinatorActor)
        {
            this.consoleWriterActor = consoleWriterActor;
            this.tailCoordinatorActor = tailCoordinatorActor;
        }

        protected override void OnReceive(object message)
        {
            string msg = message as string;

            if (string.IsNullOrEmpty(msg))
            {
                // Signal that the user needs to supply an input

                consoleWriterActor.Tell(new NullInputError("Input was blank. Please try again.\n"));

                // Tell the sender to continue doing its thing (whaterver that may be, this actor doesn't care)
                Sender.Tell(new ContinueProcessing());
            }
            else
            {
                var valid = IsFileUri(msg);
                if (valid)
                {
                    // Signal successful input
                    consoleWriterActor.Tell(new InputSuccess($"Starting processing for {msg}"));

                    // Start coordinator
                    tailCoordinatorActor.Tell(new TailCoordinatorActor.StartTail(msg, consoleWriterActor));
                }
                else
                {
                    // Signal that the input was bad
                    consoleWriterActor.Tell(new ValidationError($"{msg} is not an existing URI on disk."));

                    // Tell sender to continue doing its thing
                    // (whatever that may be, this actor doesn't care)
                    Sender.Tell(new ContinueProcessing());
                }
            }
        }

        private bool IsFileUri(string path)
        {
            return File.Exists(path);
        }
    }
}