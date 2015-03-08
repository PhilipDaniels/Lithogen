using System.Collections.Generic;

namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// The job of the partial template resolver is to pick an appropriate file
    /// from a set of files. It may be provided with a full pathname, or just "_Layout".
    /// Sees much use in RazorProcessor.
    /// </summary>
    public interface IPartialResolver
    {
        /// <summary>
        /// Given a partial name, returns the matching partial template.
        /// </summary>
        /// <param name="fromFiles">The set of files to resolve the partial from.</param>
        /// <param name="partialName">The name of the partial.</param>
        /// <param name="relativeTo">Filename to resolve relative to.</param>
        /// <returns>TextFile object containing the body of the template.</returns>
        ITextFile ResolvePartial(IEnumerable<ITextFile> fromFiles, string partialName, string relativeTo);
    }
}
