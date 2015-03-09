using System;
using Lithogen.Core;

namespace Lithogen.Engine
{
    /// <summary>
    /// Provides some utilities that can be used to help build concrete ModelInjectors.
    /// </summary>
    public static class ModelInjectorUtilities
    {
        /// <summary>
        /// Strips front matter, if any, from the file's contents. The front matter
        /// is returned as a string.
        /// </summary>
        /// <param name="file">The file to strip the front matter from.</param>
        /// <param name="marker">The marker used to delimit the front matter, for example "---" for Yaml.</param>
        /// <returns>The front matter string, or null if there is none.</returns>
        public static string StripFrontMatter(ITextFile file, string marker)
        {
            file.ThrowIfNull("file");
            marker.ThrowIfNullOrEmpty("marker");

            string mm = GetMatchingMarker(file.Contents, marker);
            if (mm == null)
                return null;
            int indexOfClosingMarker = file.Contents.IndexOf(mm, mm.Length, StringComparison.OrdinalIgnoreCase);
            if (indexOfClosingMarker == 0)
                return null;

            string frontMatter = file.Contents.Substring(mm.Length, indexOfClosingMarker - mm.Length);
            file.Contents = file.Contents.Substring(indexOfClosingMarker + mm.Length);
            return frontMatter;
        }

        /// <summary>
        /// Figure out if the contents start with the specified marker.
        /// No need to use regexes for something this simple.
        /// But allow for different file encodings.
        /// </summary>
        static string GetMatchingMarker(string contents, string marker)
        {
            if (contents.StartsWith(marker + "\r\n", StringComparison.OrdinalIgnoreCase))
                return marker + "\r\n";
            else if (contents.StartsWith(marker + "\r", StringComparison.OrdinalIgnoreCase))
                return marker + "\r";
            else if (contents.StartsWith(marker + "\n", StringComparison.OrdinalIgnoreCase))
                return marker + "\n";
            else
                return null;
        }
    }
}
