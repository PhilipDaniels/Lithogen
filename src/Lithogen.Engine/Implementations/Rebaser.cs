using System;
using System.IO;
using System.Text;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class Rebaser : IRebaser
    {
        public const string PathToRoot = "PATHTOROOT(~)";

        readonly ISettings TheSettings;

        public Rebaser(ISettings settings)
        {
            TheSettings = settings.ThrowIfNull("settings");
        }

        /// <summary>
        /// Rebase a filename into the output directory, that is, taking account of where the file is
        /// in the source directory, determine what the corresponding file in the output directory
        /// (LithogenWebsiteDirectory) will be.
        /// </summary>
        /// <param name="fileName">The filename to rebase.</param>
        /// <returns>A corresponding filename under the LithogenWebsiteDirectory.</returns>
        public string RebaseFileNameIntoOutputDirectory(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            if (fileName.StartsWith(TheSettings.LithogenWebsiteDirectory, StringComparison.OrdinalIgnoreCase))
                return fileName;

            string subPath;
            if (fileName.StartsWith(TheSettings.ViewsDirectory, StringComparison.OrdinalIgnoreCase))
            {
                // Files in the view folder get moved up a level.
                subPath = fileName.Replace(TheSettings.ViewsDirectory, "", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                subPath = fileName.Replace(TheSettings.ProjectDirectory, "", StringComparison.OrdinalIgnoreCase);
            }

            subPath = subPath.TrimStart(new char[] { '\\', '/' });

            string rebasedName = Path.Combine(TheSettings.LithogenWebsiteDirectory, subPath);
            return rebasedName;
        }

        /// <summary>
        /// For a particular <paramref name="fileName"/>, which must be under the project directory
        /// in a known directory, determine the path to the root of the website. This will either
        /// be blank or a set of "../" sequences sufficient to get up to the root.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns>Path to the root of the website.</returns>
        public string GetPathToRoot(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            if (!FileIsInKnownDirectory(fileName))
                throw new ArgumentException("The file is not in a known directory: " + fileName, "fileName");

            int numDirs = 0;
            string f = Path.GetDirectoryName(fileName);
            while (!f.Equals(TheSettings.ProjectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                numDirs++;
                f = Path.GetDirectoryName(f);
            }

            // Files in the view folder get moved up a level.
            if (fileName.StartsWith(TheSettings.ViewsDirectory, StringComparison.OrdinalIgnoreCase))
                numDirs--;

            if (numDirs < 0)
                throw new InvalidOperationException("numDirs should never be negative, that indicates you have gone outside the project directory");

            if (numDirs == 0)
            {
                return "./";
            }
            else
            {
                var sb = new StringBuilder();
                for (int i = 0; i < numDirs; i++)
                    sb.Append("../");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Search for the PATHTOROOT(~) symbol in a the <paramref name="contents"/> of a <paramref name="fileName"/>
        /// and replace it with the appropriate path from <code>GetPathToRoot().</code>
        /// </summary>
        /// <param name="fileName">The filename that holds the contents.</param>
        /// <param name="contents">The contents.</param>
        /// <returns>The contents with the PATHTOROOT(~) markers replaced.</returns>
        public string ReplaceRootsInFile(string fileName, string contents)
        {
            fileName.ThrowIfNullOrWhiteSpace("filename");
            contents.ThrowIfNull("contents");

            string rootPath = GetPathToRoot(fileName);
            contents = contents.Replace(PathToRoot, rootPath);
            return contents;
        }

        bool FileIsInKnownDirectory(string filename)
        {
            return filename.StartsWith(TheSettings.ViewsDirectory, StringComparison.OrdinalIgnoreCase) ||
                   filename.StartsWith(TheSettings.ContentDirectory, StringComparison.OrdinalIgnoreCase) ||
                   filename.StartsWith(TheSettings.ScriptsDirectory, StringComparison.OrdinalIgnoreCase) ||
                   filename.StartsWith(TheSettings.ImagesDirectory, StringComparison.OrdinalIgnoreCase);
        }
    }
}
