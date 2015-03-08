using System.IO;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class CleanCommandHandler : ICommandHandler<CleanCommand>
    {
        const string LOG_PREFIX = "CleanCommandHandler: ";
        readonly ILogger TheLogger;

        public CleanCommandHandler(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Handle(CleanCommand command)
        {
            command.ThrowIfNull("command");
            command.Directory.ThrowIfNullOrWhiteSpace("command.Directory");

            if (Directory.Exists(command.Directory))
            {
                FileUtils.DeleteDirectoryContents(command.Directory);
                TheLogger.LogMessage(LOG_PREFIX + "Directory {0} cleaned.", command.Directory);
            }
        }
    }
}
