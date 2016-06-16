namespace WinTail
{
    using System;
    using System.IO;
    using Actors;
    using Akka.Actor;

    class FileObserver : IDisposable
    {
        private readonly IActorRef tailActor;
        private readonly string absoluteFilePath;
        private readonly string fileDir;
        private readonly string fileNameOnly;
        private FileSystemWatcher watcher;

        public FileObserver(IActorRef tailActor, string absoluteFilePath)
        {
            this.tailActor = tailActor;
            this.absoluteFilePath = absoluteFilePath;

            fileDir = Path.GetDirectoryName(absoluteFilePath);
            fileNameOnly = Path.GetFileName(absoluteFilePath);
        }

        /// <summary>
        /// Begin monitoring the file
        /// </summary>
        public void Start()
        {
            // Make watcher to observe our speciic file
            watcher = new FileSystemWatcher(fileDir, fileNameOnly);

            // Watch our file for changes to the file name,
            // or new messages being written to the file
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            // Assign callbacks for event types
            watcher.Changed += OnFileChanged;
            watcher.Error += OnFileError;

            // Start watching
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Callback for <see cref="FileSystemWatcher"/> file change events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                // Here we use a special ActorRefs.NoSender
                // since this event can happen many times,
                // this is a little microoptimisation
                tailActor.Tell(new TailActor.FileWrite(e.Name), ActorRefs.NoSender);
            }
        }

        private void OnFileError(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stop monitoring file
        /// </summary>
        public void Dispose()
        {
            watcher.Dispose();
        }
    }
}