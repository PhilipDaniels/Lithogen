using System.IO;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class FileDeleteCommandHandler : ICommandHandler<FileDeleteCommand>
    {
        const string LOG_PREFIX = "FileDeleteCommandHandler: ";
        readonly ILogger TheLogger;
        readonly IRebaser Rebaser;

        public FileDeleteCommandHandler(ILogger logger, IRebaser rebaser)
        {
            TheLogger = logger.ThrowIfNull("logger");
            Rebaser = rebaser.ThrowIfNull("rebaser");
        }

        public void Handle(FileDeleteCommand command)
        {
            command.ThrowIfNull("command");
            command.FileName.ThrowIfNullOrWhiteSpace("command.Filename");

            string destFileName = Rebaser.RebaseFileNameIntoOutputDirectory(command.FileName);
            File.Delete(destFileName);
            TheLogger.LogMessage(LOG_PREFIX + "Deleted {0}", destFileName);
        }
    }
}
