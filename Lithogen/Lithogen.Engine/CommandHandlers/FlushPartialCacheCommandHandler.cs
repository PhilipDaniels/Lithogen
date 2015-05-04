using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class FlushPartialCacheCommandHandler : ICommandHandler<FlushPartialCacheCommand>
    {
        const string LOG_PREFIX = "FlushPartialCacheCommandHandler: ";
        readonly ILogger TheLogger;
        readonly IPartialCache PartialCache;

        public FlushPartialCacheCommandHandler(ILogger logger, IPartialCache partialCache)
        {
            TheLogger = logger.ThrowIfNull("logger");
            PartialCache = partialCache.ThrowIfNull("partialCache");
        }

        public void Handle(FlushPartialCacheCommand command)
        {
            command.ThrowIfNull("command");

            int count = PartialCache.Flush();
            if (count > 0)
                TheLogger.LogMessage(LOG_PREFIX + "Flushed {0} files from the partial cache.", count);
        }
    }
}
