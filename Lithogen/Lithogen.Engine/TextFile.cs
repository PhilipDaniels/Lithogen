using System.Diagnostics;
using System.IO;
using BassUtils;
using Lithogen.Core;

namespace Lithogen.Engine
{
    /// <summary>
    /// Represents a simple text file, such as a partial template, on
    /// which no further processing is required.
    /// </summary>
    [DebuggerDisplay("{FileName} : {Contents == null ? \"\" : Contents.Substring(0, 40)}")]
    public class TextFile : ITextFile
    {
        /// <summary>
        /// The filename.
        /// </summary>
        public string FileName { get; private set; }

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
        /// <param name="fileName">The file to load.</param>
        public TextFile(string fileName)
        {
            FileName = fileName.ThrowIfFileDoesNotExist("fileName");
            Contents = File.ReadAllText(FileName);
            FileInfo = new Lithogen.Engine.FileInfo(FileName);
        }
    }
}
