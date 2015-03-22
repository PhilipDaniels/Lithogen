using System;
using System.IO;
using System.Text;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// The default output file writer. It rebases the filename and writes it into the output directory.
    /// </summary>
    public class OutputFileWriter : IOutputFileWriter
    {
        const string LOG_PREFIX = "OutputFileWriter: ";
        readonly ILogger TheLogger;
        readonly ISettings TheSettings;
        readonly IRebaser Rebaser;

        public OutputFileWriter(ILogger logger, ISettings settings, IRebaser rebaser)
        {
            TheLogger = logger.ThrowIfNull("logger");
            TheSettings = settings.ThrowIfNull("settings");
            Rebaser = rebaser.ThrowIfNull("rebaser");
        }

        /// <summary>
        /// Writes a text file to the output directory, rebasing it first.
        /// </summary>
        /// <param name="destinationFileName">Name of the file (within the Views folder after processing) to write.</param>
        /// <param name="contents">Contents of the file.</param>
        public virtual void WriteFile(string destinationFileName, string contents)
        {
            destinationFileName.ThrowIfNullOrWhiteSpace("destinationFileName");
            contents.ThrowIfNull("contents");

            destinationFileName = Rebaser.RebaseFileNameIntoOutputDirectory(destinationFileName);
            CheckDestinationFilename(destinationFileName);
            FileUtils.EnsureParentDirectory(destinationFileName);
            WriteFileWithUtf8Preamble(destinationFileName, contents);
            TheLogger.LogVerbose(LOG_PREFIX + "Wrote {0} characters to {1}", contents.Length, destinationFileName);
        }

        /// <summary>
        /// Copies a file from the source to the output directory, rebasing it in the process.
        /// </summary>
        /// <param name="sourceFileName">Source filename.</param>
        public virtual void CopyFile(string sourceFileName)
        {
            sourceFileName.ThrowIfFileDoesNotExist("sourceFileName");

            string destinationFileName = Rebaser.RebaseFileNameIntoOutputDirectory(sourceFileName);
            CheckDestinationFilename(destinationFileName);
            FileUtils.EnsureParentDirectory(destinationFileName);
            File.Copy(sourceFileName, destinationFileName, true);
            TheLogger.LogVerbose(LOG_PREFIX + "Copied {0} to {1}", sourceFileName, destinationFileName);
        }

        /// <summary>
        /// Writes text to a file, ensuring that the file is UTF-8 and includes
        /// the preamble. The file will be overwritten if it already exists.
        /// The directory must already exist.
        /// </summary>
        /// <param name="fileName">The filename to write to.</param>
        /// <param name="text">The text to write.</param>
        public static void WriteFileWithUtf8Preamble(string fileName, string text)
        {
            var enc = new UTF8Encoding(true);

            using (var fs = new FileStream(fileName, FileMode.Create))
            using (var sw = new StreamWriter(fs, enc))
            {
                sw.Write(text);
            }
        }

        void CheckDestinationFilename(string destinationFileName)
        {
            // Sanity check: we must always be writing into the output directory.
            if (!destinationFileName.StartsWith(TheSettings.LithogenWebsiteDirectory, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Attempt to write to a file that is not in the output directory: " + destinationFileName);
        }
    }
}
