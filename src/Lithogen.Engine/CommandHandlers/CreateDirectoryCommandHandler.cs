using System.IO;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class CreateDirectoryCommandHandler : ICommandHandler<CreateDirectoryCommand>
    {
        const string LOG_PREFIX = "CreateDirectoryCommandHandler: ";
        readonly ILogger TheLogger;

        public CreateDirectoryCommandHandler(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Handle(CreateDirectoryCommand command)
        {
            command.ThrowIfNull("command");
            command.Directory.ThrowIfNullOrWhiteSpace("command.Directory");

            if (!Directory.Exists(command.Directory))
            {
                Directory.CreateDirectory(command.Directory);
                TheLogger.LogMessage(LOG_PREFIX + "Directory {0} created.", command.Directory);
            }
        }
    }
}
