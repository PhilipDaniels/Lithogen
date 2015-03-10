using System;
using System.Collections.Generic;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// This simple, default PartialResolver always looks in the Shared folder for a template name.
    /// </summary>
    public class SimplePartialResolver : IPartialResolver
    {
        /// <summary>
        /// Given a partial name, returns the matching partial template as a tuple
        /// of filename and body.
        /// </summary>
        /// <param name="fromFiles">The set of files to resolve the partial from.</param>
        /// <param name="partialName">The name of the partial.</param>
        /// <param name="relativeTo">Filename to resolve relative to.</param>
        /// <returns>TextFile object containing the body of the template.</returns>
        public ITextFile ResolvePartial(IEnumerable<ITextFile> fromFiles, string partialName, string relativeTo)
        {
            fromFiles.ThrowIfNull("fromFiles");
            partialName.ThrowIfNullOrWhiteSpace("partialName");

            // Try for an exact match.
            foreach (var file in fromFiles)
                if (file.FileName.Equals(partialName, StringComparison.OrdinalIgnoreCase))
                    return file;

            // Try for a partial match.
            foreach (var file in fromFiles)
            {
                if (file.FileName.EndsWith(partialName, StringComparison.OrdinalIgnoreCase))
                    return file;
            }

            throw new ArgumentException("The partialName " + partialName + " is unknown.");
        }
    }
}
