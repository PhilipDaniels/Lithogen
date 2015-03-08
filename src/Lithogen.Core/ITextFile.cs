namespace Lithogen.Core
{
    /// <summary>
    /// Represents a simple text file, such as a partial template, on
    /// which no further processing is required.
    /// </summary>
    public interface ITextFile
    {
        /// <summary>
        /// The filename.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// The contents of the file.
        /// </summary>
        string Contents { get; set; }

        /// <summary>
        /// The file info object for this file.
        /// </summary>
        IFileInfo FileInfo { get; }
    }
}
