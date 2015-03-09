using System;
using System.IO;
using System.Text;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class Rebaser : IRebaser
    {
        public static string PATHTOROOT = "PATHTOROOT(~)";

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
        /// <param name="filename">The filename to rebase.</param>
        /// <returns>A corresponding filename under the LithogenWebsiteDirectory.</returns>
        public string RebaseFileNameIntoOutputDirectory(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");

            if (filename.StartsWith(TheSettings.LithogenWebsiteDirectory, StringComparison.InvariantCultureIgnoreCase))
                return filename;

            string subPath;
            if (filename.StartsWith(TheSettings.ViewsDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                // Files in the view folder get moved up a level.
                subPath = filename.Replace(TheSettings.ViewsDirectory, "", StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                subPath = filename.Replace(TheSettings.ProjectDirectory, "", StringComparison.InvariantCultureIgnoreCase);
            }

            subPath = subPath.TrimStart(new char[] { '\\', '/' });

            string rebasedName = Path.Combine(TheSettings.LithogenWebsiteDirectory, subPath);
            return rebasedName;
        }

        /// <summary>
        /// For a particular <paramref name="filename"/>, which must be under the project directory
        /// in a known directory, determine the path to the root of the website. This will either
        /// be blank or a set of "../" sequences sufficient to get up to the root.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>Path to the root of the website.</returns>
        public string GetPathToRoot(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");

            if (!FileIsInKnownDirectory(filename))
                throw new ArgumentException("The file is not in a known directory: " + filename, "containingFilename");

            int numDirs = 0;
            string f = Path.GetDirectoryName(filename);
            while (!f.Equals(TheSettings.ProjectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                numDirs++;
                f = Path.GetDirectoryName(f);
            }

            // Files in the view folder get moved up a level.
            if (filename.StartsWith(TheSettings.ViewsDirectory, StringComparison.OrdinalIgnoreCase))
                numDirs--;

            if (numDirs < 0)
                throw new InvalidOperationException("numDirs should never be negative, that indicates you have gone outside the project directory");

            if (numDirs == 0)
            {
                // TODO: or "./"?
                return "";
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
        /// Search for the PATHTOROOT(~) symbol in a the <paramref name="contents"/> of a <paramref name="filename"/>
        /// and replace it with the appropriate path from <code>GetPathToRoot().</code>
        /// </summary>
        /// <param name="filename">The filename that holds the contents.</param>
        /// <param name="contents">The contents.</param>
        /// <returns>The contents with the PATHTOROOT(~) markers replaced.</returns>
        public string ReplaceRootsInFile(string filename, string contents)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");
            contents.ThrowIfNull("contents");

            string rootPath = GetPathToRoot(filename);
            contents = contents.Replace(PATHTOROOT, rootPath);
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
