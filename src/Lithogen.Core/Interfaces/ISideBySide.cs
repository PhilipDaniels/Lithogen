using System.Collections.Generic;

namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Side by side files are .json or .yaml files with the same name as the main file.
    /// Side by side files can start with a leading "_", which prevents them from being
    /// copied to the output directory.
    /// </summary>
    public interface ISideBySide
    {
        /// <summary>
        /// Returns the default set of side-by-side extensions.
        /// These are used if you don't pass any in.
        /// </summary>
        IEnumerable<string> DefaultSideBySideExtensions { get; }

        /// <summary>
        /// Retrieves a list of the side-by-side files that exist for a given file.
        /// List may be empty.
        /// </summary>
        /// <param name="fileName">The file.</param>
        /// <param name="sideBySideExtension">Expected extension, e.g. "yaml" or "json".</param>
        /// <returns>List of existing files.</returns>
        IEnumerable<string> GetSideBySideFiles(string fileName, string sideBySideExtension);

        /// <summary>
        /// Returns true if the file is a side-by-side file. This is determined by checking the file extension
        /// against a list of possible extensions. No checking is done that a corresponding main file
        /// exists, so this is a fast method.
        /// </summary>
        /// <param name="possibleSideBySideFileName">The filename you want to check.</param>
        /// <param name="sideBySideExtensions">Set of extensions to check against. Normally you
        /// would leave this blank to use the defaults.</param>
        /// <returns>True if this is a side-by-side file, false otherwise.</returns>
        bool IsSideBySideFile(string possibleSideBySideFileName, params string[] sideBySideExtensions);

        /// <summary>
        /// Gets the corresponding main file for a side-by-side file. Will return null if <paramref name="possibleSideBySideFileName"/>
        /// is not actually a side-by-side file or if there is no corresponding main file.
        /// If a file is returned, it is guaranteed to exist on disk (so the method can be slow because it
        /// has to do a file search).
        /// </summary>
        /// <param name="possibleSideBySideFileName">The filename you want to check.</param>
        /// <param name="sideBySideExtensions">Set of extensions to check against. Normally you
        /// would leave this blank to use the defaults.</param>
        /// <returns>The name of the main file if it exists, false otherwise.</returns>
        string GetMainFile(string possibleSideBySideFileName, params string[] sideBySideExtensions);
    }
}
