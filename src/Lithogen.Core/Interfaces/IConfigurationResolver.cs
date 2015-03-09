namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Gets the configuration in effect for a view file.
    /// </summary>
    public interface IConfigurationResolver
    {
        /// <summary>
        /// Gets the configuration in effect for a view file.
        /// </summary>
        /// <param name="fileName">The view file.</param>
        IDirectoryConfiguration GetConfiguration(string fileName);

        /// <summary>
        /// Checks to see whether there is a file processor for a particular extension.
        /// Whether there is depends on the configuration in the directory.
        /// </summary>
        /// <param name="fileName">The file to check.</param>
        /// <returns>True if the extension maps to a file processor, false otherwise.</returns>
        bool IsMappedExtension(string fileName);

        /// <summary>
        /// Checks to see whether there is a file processor for a particular extension.
        /// Whether there is depends on the configuration in the directory.
        /// </summary>
        /// <param name="directory">The directory in which to check.</param>
        /// <param name="extension">The file extension.</param>
        /// <returns>True if the extension maps to a file processor, false otherwise.</returns>
        bool IsMappedExtension(string directory, string extension);
    }
}
