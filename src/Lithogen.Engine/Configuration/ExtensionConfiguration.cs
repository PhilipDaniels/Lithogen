using System;
using System.Collections.Generic;
using System.Linq;
using BassUtils;
using Lithogen.Core;

namespace Lithogen.Engine.Configuration
{
    public class ExtensionConfiguration : IExtensionConfiguration
    {
        /// <summary>
        /// Ordered list of names of processors to apply.
        /// </summary>
        public IEnumerable<string> Processors { get; set; }

        /// <summary>
        /// Whether the file should be published.
        /// </summary>
        public bool? DefaultPublish { get; set; }

        /// <summary>
        /// Name of the layout the file uses.
        /// </summary>
        public string DefaultLayout { get; set; }

        /// <summary>
        /// Name of the model the file uses.
        /// </summary>
        public string DefaultModelName { get; set; }

        /// <summary>
        /// The extension that should be used for the output file.
        /// </summary>
        public string DefaultExtOut { get; set; }

        /// <summary>
        /// A configuration file in a particular directory only overrides
        /// the settings already set from the file in the parent directory.
        /// </summary>
        /// <param name="parentConfig">The parent configuration object.</param>
        public void ApplyDefaultsFromParent(IExtensionConfiguration parentConfig)
        {
            parentConfig.ThrowIfNull("parentConfig");

            if (Processors == null || !Processors.Any())
                Processors = parentConfig.Processors;
            if (!DefaultPublish.HasValue)
                DefaultPublish = parentConfig.DefaultPublish;
            if (String.IsNullOrWhiteSpace(DefaultLayout))
                DefaultLayout = parentConfig.DefaultLayout;
            if (String.IsNullOrWhiteSpace(DefaultModelName))
                DefaultModelName = parentConfig.DefaultModelName;
            if (String.IsNullOrWhiteSpace(DefaultExtOut))
                DefaultExtOut = parentConfig.DefaultExtOut;
        }
    }
}
