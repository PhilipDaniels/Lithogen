using System;
using BassUtils;
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
        /// <param name="fileName">Filename.</param>
        /// <returns>The FileClass of the filename.</returns>
        public FileClass Classify(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            if (fileName.StartsWith(TheSettings.ContentDirectory, StringComparison.OrdinalIgnoreCase))
                return FileClass.Content;
            else if (fileName.StartsWith(TheSettings.ImagesDirectory, StringComparison.OrdinalIgnoreCase))
                return FileClass.Image;
            else if (fileName.StartsWith(TheSettings.ScriptsDirectory, StringComparison.OrdinalIgnoreCase))
                return FileClass.Script;
            else if (fileName.StartsWith(TheSettings.PartialsDirectory, StringComparison.OrdinalIgnoreCase))
                return FileClass.Partial;
            else if (fileName.StartsWith(TheSettings.ViewsDirectory, StringComparison.OrdinalIgnoreCase))
                return FileClass.View;
            else
                return FileClass.Unknown;
        }
    }
}
