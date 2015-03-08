using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System;
using System.IO;

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
        /// <param name="destinationFilename">Name of the file (within the Views folder after processing) to write.</param>
        /// <param name="contents">Contents of the file.</param>
        public virtual void WriteFile(string destinationFilename, string contents)
        {
            destinationFilename.ThrowIfNullOrWhiteSpace("filename");
            contents.ThrowIfNull("contents");

            destinationFilename = Rebaser.RebaseFileNameIntoOutputDirectory(destinationFilename);
            CheckDestinationFilename(destinationFilename);
            FileUtils.EnsureParentDirectory(destinationFilename);
            FileUtils.WriteFileWithUtf8Preamble(destinationFilename, contents);
            TheLogger.LogVerbose(LOG_PREFIX + "Wrote {0} characters to {1}", contents.Length, destinationFilename);
        }

        /// <summary>
        /// Copies a file from the source to the output directory, rebasing it in the process.
        /// </summary>
        /// <param name="sourceFilename">Source filename.</param>
        public virtual void CopyFile(string sourceFilename)
        {
            sourceFilename.ThrowIfFileDoesNotExist("sourceFilename");

            string destinationFilename = Rebaser.RebaseFileNameIntoOutputDirectory(sourceFilename);
            CheckDestinationFilename(destinationFilename);
            FileUtils.EnsureParentDirectory(destinationFilename);
            File.Copy(sourceFilename, destinationFilename, true);
            TheLogger.LogVerbose(LOG_PREFIX + "Copied {0} to {1}", sourceFilename, destinationFilename);
        }

        void CheckDestinationFilename(string destinationFilename)
        {
            // Sanity check: we must always be writing into the output directory.
            if (!destinationFilename.StartsWith(TheSettings.LithogenWebsiteDirectory, System.StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidOperationException("Attempt to write to a file that is not in the output directory: " + destinationFilename);
        }
    }
}
