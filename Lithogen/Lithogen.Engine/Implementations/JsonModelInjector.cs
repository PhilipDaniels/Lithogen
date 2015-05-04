using System;
using System.Dynamic;
using System.IO;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class JsonModelInjector : IModelInjector
    {
        const string LOG_PREFIX = "JsonModelInjector: ";
        readonly ILogger TheLogger;
        readonly ISideBySide SideBySide;

        public JsonModelInjector(ILogger logger, ISideBySide sideBySide)
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
            foreach (var jsonFile in SideBySide.GetSideBySideFiles(file.FileName, "json"))
            {
                string fileContents = File.ReadAllText(jsonFile);
                MergeJson(file, fileContents);
                TheLogger.LogVerbose(LOG_PREFIX + "Loaded Json from " + jsonFile);
            }
        }

        static void InjectFrontMatter(IPipelineFile file)
        {
            string frontMatter = ModelInjectorUtilities.StripFrontMatter(file, "###");
            if (String.IsNullOrWhiteSpace(frontMatter))
                return;

            MergeJson(file, frontMatter);
        }

        static void MergeJson(IPipelineFile file, string jsonString)
        {
            var jsonExpando = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(jsonString);
            file.Data.Merge(jsonExpando);
        }
    }
}
