using System.Collections.Generic;

namespace Lithogen.Core
{
    /// <summary>
    /// Represents the configuration for a single directory.
    /// </summary>
    public interface IDirectoryConfiguration
    {
        /// <summary>
        /// The set of configurations, by file extension.
        /// </summary>
        IDictionary<string, IExtensionConfiguration> ExtensionMappings { get; }
    }
}
