using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Lithogen.Core;

namespace Lithogen.Engine
{
    /// <summary>
    /// Wrapper around <code>FileSystemWatcher</code> that tries to uniquefy events,
    /// because <code>FileSystemWatcher</code> raises lots of duplicates.
    /// </summary>
    public sealed class DirectoryWatcher : IDisposable
    {
        const int TimerPeriodMillisecs = 100;
        readonly string Directory;
        readonly FileSystemWatcher Watcher;
        readonly ConcurrentQueue<FileNotification> NotifiedEvents;
        readonly Timer Timer;
        bool Disposed;

        public ICollection<string> FilesToIgnore { get; private set; }
        public ICollection<string> DirectoriesToIgnore { get; private set; }

        public event EventHandler<IEnumerable<FileNotification>> ChangedFiles;
        
        public DirectoryWatcher(string directory)
        {
            Directory = directory.ThrowIfDirectoryDoesNotExist("directory");
            Watcher = new FileSystemWatcher();
            NotifiedEvents = new ConcurrentQueue<FileNotification>();
            FilesToIgnore = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            DirectoriesToIgnore = new List<string>();
            Timer = new Timer(OnTimeout, null, TimerPeriodMillisecs, TimerPeriodMillisecs);
        }

        public void Start()
        {
            ThrowIfDisposed();

            Watcher.Path = Directory;
            Watcher.IncludeSubdirectories = true;
            Watcher.Created += Watcher_Created;
            Watcher.Changed += Watcher_Created;
            Watcher.Deleted += Watcher_Deleted;
            Watcher.Renamed += Watcher_Renamed;

            Watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            ThrowIfDisposed();

            Watcher.EnableRaisingEvents = false;
            Timer.Dispose();
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Watcher.Dispose();
                Timer.Dispose();
                Disposed = true;
            }
        }

        bool ShouldIgnore(string filename)
        {
            return FilesToIgnore.Contains(filename) ||
                   DirectoriesToIgnore.Any(d => filename.StartsWith(d, StringComparison.OrdinalIgnoreCase));
        }

        void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (ShouldIgnore(e.FullPath))
                return;

            var n = new FileNotification(ConvertWatcherType(e.ChangeType), e.FullPath);
            NotifiedEvents.Enqueue(n);
        }

        void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (ShouldIgnore(e.FullPath))
                return;

            var n = new FileNotification(ConvertWatcherType(e.ChangeType), e.FullPath);
            NotifiedEvents.Enqueue(n);
        }

        void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (ShouldIgnore(e.FullPath))
                return;

            var n = new FileNotification(ConvertWatcherType(e.ChangeType), e.FullPath);
            NotifiedEvents.Enqueue(n);
        }

        void OnTimeout(object state)
        {
            ThrowIfDisposed();

            if (NotifiedEvents.IsEmpty)
                return;

            // When the timer fires, get all pending notifications from the queue,
            // simplify/uniqueify them, and yield them as events. This eliminates
            // duplicate events that the FileSystemWatcher raises.
            var notifications = new List<FileNotification>();
            FileNotification n;
            while (NotifiedEvents.TryDequeue(out n))
                notifications.Add(n);

            if (notifications.Count == 0)
                return;

            var evt = ChangedFiles;
            if (evt != null)
                evt(this, notifications.Distinct());
        }

        static FileNotificationType ConvertWatcherType(WatcherChangeTypes type)
        {
            switch (type)
            {
                case WatcherChangeTypes.Created:
                    return FileNotificationType.Build;
                case WatcherChangeTypes.Deleted:
                    return FileNotificationType.Clean;
                case WatcherChangeTypes.Changed:
                    return FileNotificationType.Build;
                case WatcherChangeTypes.Renamed:
                    return FileNotificationType.Build;
                default:
                    throw new ArgumentException("Unexpected watcher type " + type.ToString());
            }
        }

        void ThrowIfDisposed()
        {
            if (Disposed)
                throw new InvalidOperationException("The DirectoryWatcher is already disposed.");
        }
    }
}
