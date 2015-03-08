using System.Collections.Generic;

namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Used to cache a set of files in a folder. Mainly used for caching partials
    /// so that they do not need to be reloaded as each view is processed.
    /// </summary>
    public interface ITextFileCache
    {
        /// <summary>
        /// Loads all the text files from the <paramref name="directory"/> and all sub-directories
        /// into the cache.
        /// </summary>
        /// <param name="directory">The directory to load files from.</param>
        void Load(string directory);

        /// <summary>
        /// Flushes all files from the cache.
        /// </summary>
        void Flush();

        /// <summary>
        /// Returns the number of text files loaded in the cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The set of files that are currently cached.
        /// </summary>
        IEnumerable<ITextFile> Files { get; }

        /// <summary>
        /// Retrieves a file. May cause a cache load if the cache is empty.
        /// The file must be under the cacheDirectory.
        /// </summary>
        /// <param name="filename">File to retrieve.</param>
        /// <returns>Text file object.</returns>
        ITextFile GetFile(string filename);
    }
}
