using Lithogen.Core;
using System;
using System.Collections.Generic;

namespace Lithogen.Engine.Configuration
{
    public class DirectoryConfiguration : IDirectoryConfiguration
    {
        Dictionary<string, IExtensionConfiguration> _Mappings;

        public DirectoryConfiguration()
        {
            _Mappings = new Dictionary<string, IExtensionConfiguration>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The set of configurations, by file extension.
        /// </summary>
        public IDictionary<string, IExtensionConfiguration> ExtensionMappings
        {
            get { return _Mappings; }
        }
    }
}
