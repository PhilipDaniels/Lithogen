using System;
using System.Collections.Generic;
using System.IO;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// Used to cache a set of files in a folder. Mainly used for caching partials
    /// so that they do not need to be reloaded as each view is processed.
    /// </summary>
    /// <remarks>
    /// This class makes no attempt to be thread-safe. That is left up to clients.
    /// For this reason it is sealed; clients should contain an instance rather
    /// than trying to inherit.
    /// </remarks>
    public sealed class TextFileCache : ITextFileCache
    {
        const string LOG_PREFIX = "TextFileCache: ";
        readonly ILogger TheLogger;
        readonly Dictionary<string, ITextFile> Cache;
        readonly HashSet<string> BadFiles;
        string Directory;

        public TextFileCache(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
            Cache = new Dictionary<string, ITextFile>();
            BadFiles = new HashSet<string>();
        }

        /// <summary>
        /// Loads all the text files from the <paramref name="directory"/> and all sub-directories
        /// into the cache.
        /// </summary>
        /// <param name="directory">The directory to load files from.</param>
        public void Load(string directory)
        {
            Directory = directory.ThrowIfDirectoryDoesNotExist("directory");

            LoadImpl();
        }

        /// <summary>
        /// Flushes all files from the cache.
        /// </summary>
        public void Flush()
        {
            Cache.Clear();
            BadFiles.Clear();
        }

        /// <summary>
        /// Returns the number of text files loaded in the cache.
        /// </summary>
        public int Count
        {
            get
            {
                return Cache.Count;
            }
        }

        /// <summary>
        /// The set of files that are currently cached.
        /// </summary>
        public IEnumerable<ITextFile> Files
        {
            get
            {
                return Cache.Values;
            }
        }

        /// <summary>
        /// Retrieves a file. Will throw if the <paramref name="fileName"/> is not in the cache.
        /// The file must be under the cacheDirectory.
        /// </summary>
        /// <param name="fileName">File to retrieve.</param>
        /// <returns>Text file object.</returns>
        public ITextFile GetFile(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");
            if (BadFiles.Contains(fileName))
                throw new ArgumentException("The file " + fileName + " is not a text file.");
            if (!fileName.StartsWith(Directory, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The file " + fileName + " is not within the cache directory of " + Directory);

            ITextFile file;
            if (Cache.TryGetValue(fileName, out file))
                return file;
            else
                throw new ArgumentException("The file " + fileName + " is not in the cache.");
        }

        void LoadImpl()
        {
            foreach (string filename in System.IO.Directory.EnumerateFiles(Directory, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    var tf = new TextFile(filename);
                    Cache.Add(filename, tf);
                }
                catch
                {
                    // People may put junk in their partials directory, there is not
                    // much we can do about that.
                    BadFiles.Add(filename);
                }
            }

            TheLogger.LogMessage(LOG_PREFIX + "Loaded {0} text files from {1}, and ignored {2} non-text files.", Cache.Count, Directory, BadFiles.Count);
        }
    }
}
