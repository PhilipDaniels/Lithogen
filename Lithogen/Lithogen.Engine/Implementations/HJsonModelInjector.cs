using System;
using System.Dynamic;
using System.IO;
using BassUtils;
using Hjson;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class HJsonModelInjector : IModelInjector
    {
        const string LOG_PREFIX = "HJsonModelInjector: ";
        readonly ILogger TheLogger;
        readonly ISideBySide SideBySide;

        public HJsonModelInjector(ILogger logger, ISideBySide sideBySide)
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
            foreach (var hjsonFile in SideBySide.GetSideBySideFiles(file.FileName, "hjson"))
            {
                string fileContents = File.ReadAllText(hjsonFile);
                MergeHJson(file, fileContents);
                TheLogger.LogVerbose(LOG_PREFIX + "Loaded HJson from " + hjsonFile);
            }
        }

        static void InjectFrontMatter(IPipelineFile file)
        {
            string frontMatter = ModelInjectorUtilities.StripFrontMatter(file, "===");
            if (String.IsNullOrWhiteSpace(frontMatter))
                return;

            MergeHJson(file, frontMatter);
        }

        static void MergeHJson(IPipelineFile file, string hjsonString)
        {
            // First convert HJson to JSON.
            string jsonString = null;
            using (var sr = new StringReader(hjsonString))
            {
                JsonObject data = HjsonValue.Load(sr).Qo();
                //string hjson = data.ToString(Stringify.Hjson);              // This is HJSON.
                jsonString = data.ToString(Stringify.Plain);                  // This is proper JSON, one big string.
                //string hformatted = data.ToString(Stringify.Formatted);     // This is proper JSON, indented.
            }

            // Then do the same as the JsonModelInjector.
            var jsonExpando = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(jsonString);
            file.Data.Merge(jsonExpando);
        }
    }
}
