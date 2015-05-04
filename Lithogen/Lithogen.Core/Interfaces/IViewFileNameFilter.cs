namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Determine whether a filename under the Views folder should be processed or totally ignored.
    /// Typically used to filter out junk like .gitignore or partials, which are dealt with separately
    /// by convention.
    /// </summary>
    public interface IViewFileNameFilter
    {
        /// <summary>
        /// Determine whether a filename under the Views folder should be processed or totally ignored.
        /// Typically used to filter out junk like .gitignore or partials, which are dealt with separately
        /// by convention.
        /// </summary>
        /// <param name="fileName">The file to check.</param>
        /// <returns>True if the file should be ignored, false otherwise.</returns>
        bool ShouldIgnore(string fileName);
    }
}
