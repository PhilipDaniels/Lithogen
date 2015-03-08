using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class NonExistentFileCommandHandler : ICommandHandler<NonExistentFileCommand>
    {
        const string LOG_PREFIX = "NonExistentFileCommandHandler: ";
        readonly ILogger TheLogger;

        public NonExistentFileCommandHandler(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Handle(NonExistentFileCommand command)
        {
            command.ThrowIfNull("command");

            TheLogger.LogMessage(LOG_PREFIX + "The file {0} does not exist, ignoring.", command.Filename);
        }
    }
}
