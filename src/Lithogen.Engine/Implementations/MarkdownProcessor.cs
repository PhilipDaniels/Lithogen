using System.IO;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// The default processor for Markdown templates. Encodes as HTML.
    /// </summary>
    public class MarkdownProcessor : IProcessor
    {
        const string LOG_PREFIX = "MarkdownProcessor: ";
        readonly ILogger TheLogger;

        public MarkdownProcessor
            (
            ILogger logger
            )
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Process(IPipelineFile file)
        {
            file.ThrowIfNull("file");
            file.Contents.ThrowIfNull("file.Contents");

            TheLogger.LogVerbose(LOG_PREFIX + "Compiling {0}.", file.FileName);

            var md = new MarkdownDeep.Markdown();
            file.Contents = md.Transform(file.Contents);

            string newExtension = file.ExtOut ?? "html";
            file.WorkingFileName = Path.ChangeExtension(file.WorkingFileName, newExtension);
        }
    }
}
