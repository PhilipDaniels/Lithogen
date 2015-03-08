using System;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandLine
{
    /// <summary>
    /// Used to classify files based on their filenames.
    /// </summary>
    public class FileClassifier : IFileClassifier
    {
        readonly ISettings TheSettings;

        public FileClassifier(ISettings settings)
        {
            TheSettings = settings.ThrowIfNull("settings");
        }

        /// <summary>
        /// Given a filename, work out where in the project structure it lives
        /// and determine its <code>FileClass</code>.
        /// </summary>
        /// <param name="filename">Filename.</param>
        /// <returns>The FileClass of the filename.</returns>
        public FileClass Classify(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");

            if (filename.StartsWith(TheSettings.ContentDirectory, StringComparison.InvariantCultureIgnoreCase))
                return FileClass.Content;
            else if (filename.StartsWith(TheSettings.ImagesDirectory, StringComparison.InvariantCultureIgnoreCase))
                return FileClass.Image;
            else if (filename.StartsWith(TheSettings.ScriptsDirectory, StringComparison.InvariantCultureIgnoreCase))
                return FileClass.Script;
            else if (filename.StartsWith(TheSettings.PartialsDirectory, StringComparison.InvariantCultureIgnoreCase))
                return FileClass.Partial;
            else if (filename.StartsWith(TheSettings.ViewsDirectory, StringComparison.InvariantCultureIgnoreCase))
                return FileClass.View;
            else
                return FileClass.Unknown;
        }
    }
}
