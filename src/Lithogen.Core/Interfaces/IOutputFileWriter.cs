namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// The writer terminates the file processor chain.
    /// </summary>
    public interface IOutputFileWriter
    {
        /// <summary>
        /// Writes a text file to the output directory, rebasing it first.
        /// </summary>
        /// <param name="destinationFilename">Name of the file (within the Views folder after processing) to write.</param>
        /// <param name="contents">Contents of the file.</param>
        void WriteFile(string destinationFilename, string contents);

        /// <summary>
        /// Copies a file from the source to the output directory, rebasing it in the process.
        /// </summary>
        /// <param name="sourceFilename">Source filename.</param>
        void CopyFile(string sourceFilename);
    }
}
