using System;
using System.IO;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class YamlModelInjector : IModelInjector
    {
        const string LOG_PREFIX = "YamlModelInjector: ";
        readonly ILogger TheLogger;
        readonly ISideBySide SideBySide;

        public YamlModelInjector(ILogger logger, ISideBySide sideBySide)
        {
            TheLogger = logger.ThrowIfNull("logger");
            SideBySide = sideBySide.ThrowIfNull("sideBySide");
        }

        public void Inject(IPipelineFile file)
        {
            file.ThrowIfNull("file");
            file.Contents.ThrowIfNull("file.Contents");

            InjectSideBySide(file); 
            InjectFrontMatter(file);
        }

        void InjectSideBySide(IPipelineFile file)
        {
            foreach (var yamlFile in SideBySide.GetSideBySideFiles(file.FileName, "yaml"))
            {
                string yamlString = File.ReadAllText(yamlFile);
                var yamlExpando = YamlUtils.ToExpando(yamlString);
                file.Data.Merge(yamlExpando);
                TheLogger.LogVerbose(LOG_PREFIX + "Loaded Yaml from " + yamlFile);
            }
        }

        void InjectFrontMatter(IPipelineFile file)
        {
            string frontMatter = ModelInjectorUtilities.StripFrontMatter(file, "---");
            if (String.IsNullOrWhiteSpace(frontMatter))
                return;

            var yamlExpando = YamlUtils.ToExpando(frontMatter);
            file.Data.Merge(yamlExpando);
        }
    }
}
