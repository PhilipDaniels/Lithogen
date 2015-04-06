using System.Collections.Generic;

namespace Lithogen.Core
{
    /// <summary>
    /// Represents the settings that this invocation of Lithogen is working with.
    /// These settings are written to Lithogen.xml and Lithogen.json from where
    /// they are picked up by downstream programs.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// The importance of messages logged by the build. Defaults to HIGH (chatty).
        /// </summary>
        string MessageImportance { get; }

        /// <summary>
        /// Where Lithogen (the exes, node etc.) is installed.
        /// </summary>
        string InstallationDirectory { get; }

        /// <summary>
        /// The full path of the Lithogen program.
        /// </summary>
        string LithogenExePath { get; }

        /// <summary>
        /// The full path of the Node program.
        /// </summary>
        string NodeExePath { get; }

        /// <summary>
        /// The full path of the NPM program.
        /// </summary>
        string NpmExePath { get; }

        /// <summary>
        /// The directory that contains CSS, Less, SASS etc.
        /// </summary>
        string ContentDirectory { get; }

        /// <summary>
        /// The directory that contains images.
        /// </summary>
        string ImagesDirectory { get; }

        /// <summary>
        /// The directory that contains JavaScript.
        /// Optional, a default will be assumed if this is not set.
        /// </summary>
        string ScriptsDirectory { get; }

        /// <summary>
        /// The directory that contains the Views.
        /// Optional, a default will be assumed if this is not set.
        /// </summary>
        string ViewsDirectory { get; }

        /// <summary>
        /// The directory that contains the Partial Views and Layouts.
        /// </summary>
        string PartialsDirectory { get; }

        /// <summary>
        /// The full path of the solution file. This will not be set if you run MSBuild
        /// from the command line, it is only set within Visual Studio. So it is best to
        /// avoid creating tasks that rely on it.
        /// </summary>
        string SolutionFile { get; }

        /// <summary>
        /// The full path of the project file (.csproj) that you want Lithogen to build.
        /// </summary>
        string ProjectFile { get; }

        /// <summary>
        /// The build configuration - Debug, Release etc.
        /// </summary>
        string Configuration { get; }

        /// <summary>
        /// The target directory of the project that is being built.
        /// Typically this is the bin folder.
        /// </summary>
        string TargetDirectory { get; }

        /// <summary>
        /// The directory where the website will be written.
        /// </summary>
        string LithogenWebsiteDirectory { get; }

        /// <summary>
        /// The path where the Xml config file should be written.
        /// </summary>
        string XmlConfigFile { get; }

        /// <summary>
        /// The path where the JSON config file should be written.
        /// </summary>
        string JsonConfigFile { get; }

        /// <summary>
        /// The path where the Lithogen log file should be written.
        /// </summary>
        string LogFile { get; }

        /// <summary>
        /// The source website project's project directory.
        /// </summary>
        string ProjectDirectory { get; }

        /// <summary>
        /// Gets the list of directories from which assemblies can be dynamically loaded.
        /// Typically this includes the output directory of the website, so that Lithogen
        /// can load the model from there.
        /// </summary>
        IEnumerable<string> AssemblyLoadDirectories { get; }

        /// <summary>
        /// The URL used by the built-in web server.
        /// </summary>
        string ServeUrl { get; }

        /// <summary>
        /// The URL used to implement "live reload" functionality.
        /// </summary>
        string ReloadUrl { get; }

        /// Mutable things from here on.

        /// <summary>
        /// Specifies the degree of parallelism to use when compiling views.
        /// </summary>
        int ViewDOP { get; set; }

        string ProjectLithogenDirectory { get; }
    }
}
