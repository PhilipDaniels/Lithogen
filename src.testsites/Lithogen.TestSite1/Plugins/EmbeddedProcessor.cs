using Lithogen.Core;
using Lithogen.Core.Interfaces;
using BassUtils;

namespace Lithogen.TestSite1.Plugins
{
    /// <summary>
    /// This class exists simply to test that loading of plugins
    /// from within the website works. It should show up in the Output window
    /// (assuming you have a mapping established).
    /// </summary>
    public class EmbeddedProcessor : IProcessor
    {
        const string LOG_PREFIX = "EmbeddedProcessor: ";
        readonly ILogger TheLogger;

        public EmbeddedProcessor(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Process(IPipelineFile file)
        {
            file.ThrowIfNull("file");

            TheLogger.LogMessage(LOG_PREFIX + "Running...");
        }
    }
}