using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class UnknownFileCommandHandler : ICommandHandler<UnknownFileCommand>
    {
        const string LOG_PREFIX = "UnknownFileCommandHandler: ";
        readonly ILogger TheLogger;

        public UnknownFileCommandHandler(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Handle(UnknownFileCommand command)
        {
            command.ThrowIfNull("command");

            TheLogger.LogMessage(LOG_PREFIX + "The file {0} is unknown to Lithogen, ignoring.", command.Filename);
        }
    }
}
