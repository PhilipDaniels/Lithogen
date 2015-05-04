using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class NonexistentFileCommandHandler : ICommandHandler<NonexistentFileCommand>
    {
        const string LOG_PREFIX = "NonexistentFileCommandHandler: ";
        readonly ILogger TheLogger;

        public NonexistentFileCommandHandler(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Handle(NonexistentFileCommand command)
        {
            command.ThrowIfNull("command");

            TheLogger.LogMessage(LOG_PREFIX + "The file {0} does not exist, ignoring.", command.FileName);
        }
    }
}
