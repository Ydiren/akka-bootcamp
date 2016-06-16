namespace WinTail.Actors
{
    using System;
    using Akka.Actor;
    using Messages.Error;
    using Messages.Success;

    /// <summary>
    ///     Actor responsible for serializing message writes to the console.
    ///     (write one message at a time, champ :)
    /// </summary>
    class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is InputError)
            {
                var msg = message as InputError;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg.Reason);
            }
            else if (message is InputSuccess)
            {
                var msg = message as InputSuccess;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg.Reason);
            }
            else
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }
    }
}