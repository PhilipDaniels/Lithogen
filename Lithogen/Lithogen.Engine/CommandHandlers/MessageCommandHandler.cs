using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class MessageCommandHandler : ICommandHandler<MessageCommand>
    {
        readonly ILogger TheLogger;

        public MessageCommandHandler(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Handle(MessageCommand command)
        {
            command.ThrowIfNull("command");

            if (command.Message != null)
                TheLogger.LogMessage(command.Message);
        }
    }
}
