using Lithogen.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Lithogen.Engine
{
    /// <summary>
    /// An instance of this class is required by the default ViewDirectoryProcessor
    /// so that it can create a processor to deal with each file that it encounters.
    /// </summary>
    public interface IProcessorFactory
    {
        /// <summary>
        /// Resolve processor type names into processor type objects.
        /// The returned list may be empty.
        /// </summary>
        /// <param name="processorTypeNames">List of fully qualified processor type names.</param>
        /// <returns>Ordered list of processor types.</returns>
        IEnumerable<Type> GetProcessorTypes(IEnumerable<string> processorTypeNames);

        /// <summary>
        /// Constructs a new set of file processors.
        /// The returned list may be empty.
        /// </summary>
        /// <param name="processorTypeNames">List of fully qualified processor type names.</param>
        /// <returns>A list of IProcesor objects.</returns>
        IEnumerable<IProcessor> MakeProcessors(IEnumerable<string> processorTypeNames);
    }
}
