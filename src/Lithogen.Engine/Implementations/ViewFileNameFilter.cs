using System;
using System.IO;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

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
        /// <param name="fileName">The file to check.</param>
        /// <returns>True if the file should be ignored, false otherwise.</returns>
        public virtual bool ShouldIgnore(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            // Ignore stuff in the ASP.Net MVC Shared directory. This is partials and layouts
            // which we deal with by loading them separately (so that layout resolution can occur).
            if (fileName.StartsWith(TheSettings.PartialsDirectory, StringComparison.OrdinalIgnoreCase))
                return true;

            // Ignore files starting with an underscore, includes things such as _ViewStart.cshtml.
            string basename = Path.GetFileName(fileName);
            if (basename.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                return true;

            if (basename.Equals(".gitignore", StringComparison.OrdinalIgnoreCase) ||
                basename.Equals(".gitattributes", StringComparison.OrdinalIgnoreCase)
                )
                return true;

            // Everything else, including .html, .txt etc we accept.
            return false;
        }
    }
}
