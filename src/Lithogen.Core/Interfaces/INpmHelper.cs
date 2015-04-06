namespace Lithogen.Core.Interfaces
{
    public interface INpmHelper
    {
        /// <summary>
        /// Checks to see if an "npm install" command is required by comparing the
        /// timestamp of the <paramref name="packageJsonFileName"/> (package.json)
        /// to a separately maintained "last run" file.
        /// </summary>
        /// <param name="packageJsonFileName">Full path of the package.json file.</param>
        /// <returns>True if an "npm install" command is required, false otherwise.</returns>
        bool InstallIsRequired(string packageJsonFileName);

        /// <summary>
        /// Performs an "npm install" on the specified package.json file.
        /// </summary>
        /// <param name="packageJsonFileName">Full path of the package.json file.</param>
        void PerformInstall(string packageJsonFileName);

        /// <summary>
        /// Returns the name of the "last run" filename for a particular package.json file.
        /// </summary>
        /// <param name="packageJsonFileName">Full path of the package.json file.</param>
        /// <returns>Name of the corresponding last run file.</returns>
        string LastRunFileName(string packageJsonFileName);
    }
}
