namespace Lithogen.Core
{
    /// <summary>
    /// The classification of files that Lithogen uses.
    /// </summary>
    public enum FileClass
    {
        Unknown,        // Action => ignore the file.
        Content,        // Action => call bundler to rebuild the entire directory.
        Script,         // Action => call bundler to rebuild the entire directory.
        Image,          // Action => Copy the file to the output directory.
        Partial,        // Action => Invalidate the entire PartialCache.
        View            // Action => Pass the file to the ViewProcessor.
    }
}
