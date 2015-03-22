using Lithogen.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace Lithogen.Engine
{
    /// <summary>
    /// Represents a text file which is flowing through the view processor pipeline.
    /// </summary>
    [DebuggerDisplay("{WorkingFileName} : {Contents == null ? \"\" : Contents.Substring(0, 40)}")]
    public class PipelineFile : TextFile, IPipelineFile
    {
        /// <summary>
        /// The current working filename. Each step in the pipeline
        /// can transform this, for example Foo.cshtml -> Foo.html.
        /// </summary>
        public string WorkingFileName { get; set; }

        /// <summary>
        /// Data automatically loaded by Lithogen using ModelInjectors.
        /// </summary>
        public ExpandoObject Data { get; set; }

        /// <summary>
        /// Arbitrary tag data for users. Not touched by Lithogen.
        /// </summary>
        public ExpandoObject UserData { get; set; }

        /// <summary>
        /// The configuration in effect for this file.
        /// </summary>
        public IDirectoryConfiguration DefaultConfiguration { get; set; }

        /// <summary>
        /// Returns a (possibly empty) ordered list of processor names for this file.
        /// </summary>
        /// <param name="extension">The extension to use for the lookup.</param>
        public IEnumerable<string> GetProcessorNames(string extension)
        {
            IExtensionConfiguration extConfig; 
            if (DefaultConfiguration == null || !DefaultConfiguration.ExtensionMappings.TryGetValue(extension, out extConfig))
                return Enumerable.Empty<string>();
            else
                return extConfig.Processors;
        }

        /// <summary>
        /// Construct a new object. Reads the file from disk and sets
        /// <code>WorkingFilename</code> to be equal to <code>Filename</code>.
        /// </summary>
        /// <param name="fileName">The file to load.</param>
        public PipelineFile(string fileName)
            : base(fileName)
        {
            WorkingFileName = fileName;
            Data = new ExpandoObject();
            UserData = new ExpandoObject();
        }

        /// <summary>
        /// The name of the layout, if any. Derived from <code>Data</code>.
        /// </summary>
        public string Layout
        {
            get
            {
                return GetProperty<string>("layout", null);
            }
        }

        /// <summary>
        /// The name of the layout, if any. Derived from <code>Data</code>.
        /// Not used for Razor templates which use an @model directive.
        /// </summary>
        public string ModelName
        {
            get
            {
                return GetProperty<string>("model", null);
            }
        }

        /// <summary>
        /// The desired output extension, if any. Derived from <code>Data</code>.
        /// Not necessary to set this for most files, because most processors
        /// will pick a good default.
        /// </summary>
        public string ExtOut
        {
            get
            {
                return GetProperty<string>("extout", null);
            }
        }

        /// <summary>
        /// Whether to publish (i.e. copy to the output directory). Derived from <code>Data</code>.
        /// </summary>
        public bool Publish
        {
            get
            {
                return GetProperty<bool>("publish", true);
            }
        }

        T GetProperty<T>(string name, T defaultValue)
        {
            // Try from data.
            object value = (from kvp in (Data as IDictionary<string, object>)
                            where kvp.Key.Equals(name, StringComparison.OrdinalIgnoreCase)
                            select kvp.Value).SingleOrDefault();

            if (value != null)
                return (T)value;

            // Try from config.
            IExtensionConfiguration extConfig = GetExtConfig();
            if (extConfig == null)
                return defaultValue;

            if (name == "layout")
                value = extConfig.DefaultLayout;
            else if (name == "model")
                value = extConfig.DefaultModelName;
            else if (name == "extout")
                value = extConfig.DefaultModelName;
            else if (name == "publish")
                value = extConfig.DefaultPublish;
            else
                throw new InvalidOperationException("Unexpected name.");

            if (value == null)
                return defaultValue;
            else
                return (T)value;
        }

        IExtensionConfiguration GetExtConfig()
        {
            if (DefaultConfiguration == null)
                return null;
            IExtensionConfiguration extConfig;
            DefaultConfiguration.ExtensionMappings.TryGetValue(FileInfo.Extension, out extConfig);
            return extConfig;
        }
    }
}
