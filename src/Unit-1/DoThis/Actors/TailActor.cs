namespace WinTail.Actors
{
    using System.IO;
    using System.Text;
    using Akka.Actor;

    /// <summary>
    ///     Monitors the file at <see cref="filePath" /> for changes and sends
    ///     file updates to console.
    /// </summary>
    class TailActor : UntypedActor
    {
        private readonly string filePath;
        private readonly IActorRef reporterActor;
        private FileStream fileStream;
        private FileObserver observer;
        private StreamReader fileStreamReader;

        public TailActor(IActorRef reporterActor, string filePath)
        {
            this.reporterActor = reporterActor;
            this.filePath = filePath;
        }

        protected override void OnReceive(object message)
        {
            if (message is FileWrite)
            {
                // Move file cursor forward
                // pull results from cursor to end of file and write to output
                // (this is assuming a log file type format that is append-only)
                var text = fileStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(text))
                {
                    reporterActor.Tell(text);
                }
            }
            else if (message is FileError)
            {
                var fileError = message as FileError;
                reporterActor.Tell($"Tail error: {fileError.Reason}");
            }
            else if (message is InitialRead)
            {
                var initialRead = message as InitialRead;
                reporterActor.Tell(initialRead.Text);
            }
        }

        /// <summary>
        /// Cleanup OS handles for <see cref="fileStreamReader"/> 
        /// and <see cref="FileObserver"/>
        /// </summary>
        protected override void PostStop()
        {
            observer.Dispose();
            observer = null;

            fileStreamReader.Close();
            fileStreamReader.Dispose();
            fileStreamReader = null;

            base.PostStop();
        }

        protected override void PreStart()
        {
            // Start watching file for changes
            observer = new FileObserver(Self, Path.GetFullPath(filePath));
            observer.Start();

            // Open the file stream with shared read/write permissions
            // (so the file can be written to while open)
            fileStream = new FileStream(Path.GetFullPath(filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStreamReader = new StreamReader(fileStream, Encoding.UTF8);

            // Read the initial contents of the file and send it to console as first msg
            var text = fileStreamReader.ReadToEnd();
            Self.Tell(new InitialRead(filePath, text));
        }

        /// <summary>
        ///     Signal that the file has changed, and we need to
        ///     read the next line of the file.
        /// </summary>
        public class FileWrite
        {
            public FileWrite(string filename)
            {
                Filename = filename;
            }

            public string Filename { get; private set; }
        }

        /// <summary>
        ///     Signal that the OS had an error accessing the file.
        /// </summary>
        public class FileError
        {
            public FileError(string filename, string reason)
            {
                Filename = filename;
                Reason = reason;
            }

            public string Filename { get; private set; }
            public string Reason { get; private set; }
        }

        /// <summary>
        ///     Signal to read the initial contents of the file at actor startup.
        /// </summary>
        public class InitialRead
        {
            public InitialRead(string filename, string text)
            {
                Filename = filename;
                Text = text;
            }

            public string Filename { get; private set; }
            public string Text { get; private set; }
        }
    }
}