using System;
using System.Collections.Generic;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// Base class for processor factories. Does in fact contain a fully working implementation,
    /// but only uses Activator.CreateInstance to create processors. Better implementations
    /// are possible, e.g. using a DI engine. Subclasses typically only need to override
    /// <code>MakeProcessors</code>.
    /// </summary>
    public class ProcessorFactory : IProcessorFactory
    {
        public ProcessorFactory()
        {
        }

        /// <summary>
        /// Gets the set of processor types for an extension, sorted by their <code>SortOrder</code>.
        /// The returned list may be empty.
        /// </summary>
        /// <param name="extension">File extension, such as "md" or "cshtml".</param>
        /// <returns>Sorted list of processors (may be empty).</returns>
        public virtual IEnumerable<Type> GetProcessorTypes(IEnumerable<string> processorTypeNames)
        {
            // TODO: This will only work for processors in this assembly?
            foreach (var ptn in processorTypeNames)
                yield return Type.GetType(ptn);
        }

        /// <summary>
        /// Constructs a new set of file processors for the given extension, sorted by their <code>SortOrder</code>.
        /// The returned list may be empty.
        /// </summary>
        /// <param name="extension">File extension, such as "md" or "cshtml".</param>
        /// <returns>A list of objects which can process that extension.</returns>
        public virtual IEnumerable<IProcessor> MakeProcessors(IEnumerable<string> processorTypeNames)
        {
            var processorTypes = GetProcessorTypes(processorTypeNames);
            foreach (var pt in processorTypes)
            {
                IProcessor processor = (IProcessor)Activator.CreateInstance(pt);
                yield return processor;
            }
        }
    }
}
