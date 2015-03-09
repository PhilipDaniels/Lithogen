using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class PipelineFileLoader : IPipelineFileLoader
    {
        readonly IConfigurationResolver ConfigurationResolver;

        public PipelineFileLoader(IConfigurationResolver configurationResolver)
        {
            ConfigurationResolver = configurationResolver.ThrowIfNull("configurationResolver");
        }

        public IPipelineFile Load(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            var pipelineFile = new PipelineFile(fileName);
            pipelineFile.DefaultConfiguration = ConfigurationResolver.GetConfiguration(fileName);
            return pipelineFile;
        }
    }
}
