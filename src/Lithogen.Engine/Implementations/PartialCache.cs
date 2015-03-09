using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class PartialCache : IPartialCache
    {
        readonly ISettings TheSettings;
        readonly IPartialResolver Resolver;
        readonly ITextFileCache TextFileCache;
        object padlock = new object();

        public PartialCache
            (
            ISettings settings,
            IPartialResolver resolver,
            ITextFileCache textFileCache
            )
        {
            TheSettings = settings.ThrowIfNull("settings");
            Resolver = resolver.ThrowIfNull("resolver");
            TextFileCache = textFileCache.ThrowIfNull("textFileCache");
        }

        /// <summary>
        /// Loads all the text files from the partials folder.
        /// </summary>
        public void Load()
        {
            if (!Loaded)
            {
                lock (padlock)
                {
                    if (!Loaded)
                    {
                        if (Directory.Exists(TheSettings.PartialsDirectory))
                            TextFileCache.Load(TheSettings.PartialsDirectory);
                        Loaded = true;
                    }
                }
            }
        }
        bool Loaded;

        /// <summary>
        /// Flushes all files from the cache.
        /// </summary>
        public int Flush()
        {
            int count = 0;

            lock (padlock)
            {
                count = TextFileCache.Count;
                if (count > 0)
                    TextFileCache.Flush();

                Loaded = false;
            }

            return count;
        }

        /// <summary>
        /// Returns the number of text files loaded in the cache.
        /// </summary>
        public int Count
        {
            get
            {
                return TextFileCache.Count;
            }
        }

        /// <summary>
        /// The set of files that are currently cached.
        /// </summary>
        public IEnumerable<ITextFile> Files
        {
            get
            {
                Load();
                List<ITextFile> files;
                lock (padlock)
                {
                    files = TextFileCache.Files.ToList();
                }
                return files;
            }
        }

        /// <summary>
        /// Given a partial name, returns the matching partial template based on
        /// the files in the cache.
        /// </summary>
        /// <param name="partialName">The name of the partial.</param>
        /// <param name="relativeTo">Filename to resolve relative to.</param>
        /// <returns>TextFile object containing the body of the template.</returns>
        public ITextFile ResolvePartial(string partialName, string relativeTo)
        {
            return Resolver.ResolvePartial(TextFileCache.Files, partialName, relativeTo);
        }
    }
}
