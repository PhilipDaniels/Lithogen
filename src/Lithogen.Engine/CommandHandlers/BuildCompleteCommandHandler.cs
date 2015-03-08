using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class BuildCompleteCommandHandler : ICommandHandler<BuildCompleteCommand>
    {
        const string LOG_PREFIX = "BuildCompleteCommandHandler: ";
        readonly ILogger Logger;

        public BuildCompleteCommandHandler(ILogger logger)
        {
            Logger = logger.ThrowIfNull("logger");
        }

        public void Handle(BuildCompleteCommand command)
        {
            command.ThrowIfNull("command");

            Logger.LogMessage(LOG_PREFIX + "Your website is at {0}.", command.WebsiteTargetDirectory);
        }
    }
}
