using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System.Collections.Generic;

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
