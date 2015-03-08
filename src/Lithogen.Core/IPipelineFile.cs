using System.Collections.Generic;
using System.Dynamic;

namespace Lithogen.Core
{
    /// <summary>
    /// Represents a text file which is flowing through the view processor pipeline.
    /// </summary>
    public interface IPipelineFile : ITextFile
    {
        /// <summary>
        /// The current working filename. Each step in the pipeline
        /// can transform this, for example Foo.cshtml -> Foo.html.
        /// </summary>
        string WorkingFilename { get; set; }

        /// <summary>
        /// Data automatically loaded by Lithogen using ModelInjectors.
        /// </summary>
        ExpandoObject Data { get; set; }

        /// <summary>
        /// Arbitrary tag data for users. Not touched by Lithogen.
        /// </summary>
        ExpandoObject UserData { get; set; }

        /// <summary>
        /// The name of the layout, if any. Derived from <code>Data</code>.
        /// </summary>
        string Layout { get; }

        /// <summary>
        /// The name of the layout, if any. Derived from <code>Data</code>.
        /// Not used for Razor templates which use an @model directive.
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// The desired output extension, if any. Derived from <code>Data</code>.
        /// Not necessary to set this for most files, because most processors
        /// will pick a good default.
        /// </summary>
        string ExtOut { get; }

        /// <summary>
        /// Whether to publish (i.e. copy to the output directory). Derived from <code>Data</code>.
        /// </summary>
        bool Publish { get; }

        /// <summary>
        /// The configuration in effect for this file.
        /// </summary>
        IDirectoryConfiguration DefaultConfiguration { get; }

        /// <summary>
        /// Returns a (possibly empty) ordered list of processor names for this file.
        /// </summary>
        /// <param name="extension">The extension to use for the lookup.</param>
        IEnumerable<string> GetProcessorNames(string extension);
    }
}
