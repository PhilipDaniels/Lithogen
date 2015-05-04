using System.Collections.Generic;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class CompositeModelInjector : ICompositeModelInjector
    {
        readonly IEnumerable<IModelInjector> Injectors;

        public CompositeModelInjector(params IModelInjector[] injectors)
        {
            Injectors = injectors.ThrowIfNull("injectors");
        }

        public void InjectModels(IPipelineFile file)
        {
            file.ThrowIfNull("file");

            foreach (var injector in Injectors)
                injector.Inject(file);
        }
    }
}
