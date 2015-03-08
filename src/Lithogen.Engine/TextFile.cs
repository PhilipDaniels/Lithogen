using Lithogen.Core;
using System.Diagnostics;
using System.IO;

namespace Lithogen.Engine
{
    /// <summary>
    /// Represents a simple text file, such as a partial template, on
    /// which no further processing is required.
    /// </summary>
    [DebuggerDisplay("{Filename} : {Contents == null ? \"\" : Contents.Substring(0, 40)}")]
    public class TextFile : ITextFile
    {
        /// <summary>
        /// The filename.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// The contents of the file.
        /// </summary>
        public string Contents { get; set; }

        /// <summary>
        /// The file info object for this file.
        /// </summary>
        public IFileInfo FileInfo { get; private set; }

        /// <summary>
        /// Construct a new instance. Reads the file from disk.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        public TextFile(string filename)
        {
            Filename = filename.ThrowIfFileDoesNotExist("filename");
            Contents = File.ReadAllText(Filename);
            FileInfo = new Lithogen.Engine.FileInfo(Filename);
        }
    }
}
