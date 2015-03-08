using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.ExamplePlugin
{
    /// <summary>
    /// An example file processor. Drop into the Plugins folder and see if it is loaded.
    /// </summary>
    public class NullProcessor : IProcessor
    {
        const string LOG_PREFIX = "NullProcessor: ";
        readonly ILogger TheLogger;

        public NullProcessor(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Process(IPipelineFile file)
        {
            file.ThrowIfNull("file");

            TheLogger.LogMessage(LOG_PREFIX + "Doing absolutely nothing with {0}.", file.WorkingFilename);
        }
    }
}
