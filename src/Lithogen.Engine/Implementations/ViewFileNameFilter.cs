using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System;
using System.IO;

namespace Lithogen.Engine.Implementations
{
    /// </summary>
    /// <summary>
    /// Determine whether a filename under the Views folder should be processed or totally ignored.
    /// Typically used to filter out junk like .gitignore or partials, which are dealt with separately
    /// by convention.
    /// </summary>
    /// <remarks>
    /// TODO: This can be made a lot more sophisticated (customizable regexes?)
    /// but this crude effort will do for a prototype.
    /// </remarks>
    public class ViewFileNameFilter : IViewFileNameFilter
    {
        readonly ISettings TheSettings;

        public ViewFileNameFilter(ISettings settings)
        {
            TheSettings = settings.ThrowIfNull("settings");
        }

        /// <summary>
        /// Determine whether a filename under the Views folder should be processed or totally ignored.
        /// Typically used to filter out junk like .gitignore or partials, which are dealt with separately
        /// by convention.
        /// </summary>
        /// <param name="filename">The file to check.</param>
        /// <returns>True if the file should be ignored, false otherwise.</returns>
        public virtual bool ShouldIgnore(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");

            // Ignore stuff in the ASP.Net MVC Shared directory. This is partials and layouts
            // which we deal with by loading them separately (so that layout resolution can occur).
            if (filename.StartsWith(TheSettings.PartialsDirectory, StringComparison.InvariantCultureIgnoreCase))
                return true;

            // Ignore files starting with an underscore, includes things such as _ViewStart.cshtml.
            string basename = Path.GetFileName(filename);
            if (basename.StartsWith("_"))
                return true;

            if (basename.Equals(".gitignore", StringComparison.InvariantCultureIgnoreCase) ||
                basename.Equals(".gitattributes", StringComparison.InvariantCultureIgnoreCase)
                )
                return true;

            // Everything else, including .html, .txt etc we accept.
            return false;
        }
    }
}
