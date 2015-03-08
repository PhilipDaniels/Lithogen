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

        public IPipelineFile Load(string filename)
        {
            filename.ThrowIfNullOrWhiteSpace("filename");

            var pipelineFile = new PipelineFile(filename);
            pipelineFile.DefaultConfiguration = ConfigurationResolver.GetConfiguration(filename);
            return pipelineFile;
        }
    }
}
