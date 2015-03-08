using System.Collections.Generic;

namespace Lithogen.Core
{
    /// <summary>
    /// Represents the configuration for a single file extension, e.g. "cshtml".
    /// The properties are named "Default..." to stop confusion with the ones on
    /// the IPipelineFile.
    /// </summary>
    public interface IExtensionConfiguration
    {
        /// <summary>
        /// Ordered list of names of processors to apply.
        /// </summary>
        IEnumerable<string> Processors { get; }

        /// <summary>
        /// Whether the file should be published.
        /// </summary>
        bool? DefaultPublish { get; }

        /// <summary>
        /// Name of the layout the file uses.
        /// </summary>
        string DefaultLayout { get; }

        /// <summary>
        /// Name of the model the file uses.
        /// </summary>
        string DefaultModelName { get; }

        /// <summary>
        /// The extension that should be used for the output file.
        /// </summary>
        string DefaultExtOut { get; }

        /// <summary>
        /// A configuration file in a particular directory only overrides
        /// the settings already set from the file in the parent directory.
        /// </summary>
        /// <param name="parentConfig">The parent configuration object.</param>
        void ApplyDefaultsFromParent(IExtensionConfiguration parentConfig);
    }
}
