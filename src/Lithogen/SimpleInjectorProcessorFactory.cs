using Lithogen.Core.Interfaces;
using Lithogen.Engine.Implementations;
using SimpleInjector;
using System.Collections.Generic;
using System.Linq;

namespace Lithogen
{
    class SimpleInjectorProcessorFactory : ProcessorFactory
    {
        /// <summary>
        /// Constructs a set of new file processors for the given type names.
        /// May return an empty list if no mapping exists for the extension.
        /// </summary>
        /// <param name="processorTypeName">List of type names.</param>
        /// <returns>An object which can process that file type.</returns>
        public override IEnumerable<IProcessor> MakeProcessors(IEnumerable<string> processorTypeNames)
        {
            // Be careful to preserve order.
            foreach (var ptn in processorTypeNames)
            {
                var regProc = RegisteredProcessors.SingleOrDefault(rp => rp.Registration.ImplementationType.FullName == ptn);
                if (regProc != null)
                    yield return (IProcessor)regProc.GetInstance();
            }
        }

        IEnumerable<InstanceProducer> RegisteredProcessors
        {
            get
            {
                if (_RegisteredProcessors == null)
                {
                    lock (padlock)
                    {
                        if (_RegisteredProcessors == null)
                        {
                            _RegisteredProcessors = from p in Program.Container.GetCurrentRegistrations()
                                                    where p.ServiceType == typeof(IProcessor)
                                                    select p;
                        }
                    }
                }
                return _RegisteredProcessors;
            }
        }
        IEnumerable<InstanceProducer> _RegisteredProcessors;
        object padlock = new object();
    }
}
