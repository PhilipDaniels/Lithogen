namespace Lithogen.Core.Interfaces
{
    public interface IRebaser
    {
        /// <summary>
        /// Rebase a filename into the output directory, that is, taking account of where the file is
        /// in the source directory, determine what the corresponding file in the output directory
        /// (LithogenWebsiteDirectory) will be.
        /// </summary>
        /// <param name="fileName">The filename to rebase.</param>
        /// <returns>A corresponding filename under the LithogenWebsiteDirectory.</returns>
        string RebaseFileNameIntoOutputDirectory(string fileName);

        /// <summary>
        /// For a particular <paramref name="fileName"/>, which must be under the project directory
        /// in a known directory, determine the path to the root of the website. This will either
        /// be blank or a set of "../" sequences sufficient to get up to the root.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        /// <returns>Path to the root of the website.</returns>
        string GetPathToRoot(string fileName);

        /// <summary>
        /// Search for the PATHTOROOT(~) symbol in a the <paramref name="contents"/> of a <paramref name="fileName"/>
        /// and replace it with the appropriate path from <code>GetPathToRoot().</code>
        /// </summary>
        /// <param name="fileName">The filename that holds the contents.</param>
        /// <param name="contents">The contents.</param>
        /// <returns>The contents with the PATHTOROOT(~) markers replaced.</returns>
        string ReplaceRootsInFile(string fileName, string contents);
    }
}
