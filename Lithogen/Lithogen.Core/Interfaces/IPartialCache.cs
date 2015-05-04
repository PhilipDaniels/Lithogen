using System.Collections.Generic;

namespace Lithogen.Core.Interfaces
{
    public interface IPartialCache
    {
        /// <summary>
        /// Loads all the text files from the partials folder.
        /// </summary>
        void Load();

        /// <summary>
        /// Flushes all files from the cache.
        /// </summary>
        int Flush();

        /// <summary>
        /// Returns the number of text files loaded in the cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The set of files that are currently cached.
        /// </summary>
        IEnumerable<ITextFile> Files { get; }

        /// <summary>
        /// Given a partial name, returns the matching partial template based on
        /// the files in the cache.
        /// </summary>
        /// <param name="partialName">The name of the partial.</param>
        /// <param name="relativeTo">Filename to resolve relative to.</param>
        /// <returns>TextFile object containing the body of the template.</returns>
        ITextFile ResolvePartial(string partialName, string relativeTo);
    }
}
