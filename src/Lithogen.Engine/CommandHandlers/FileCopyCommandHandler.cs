using System.IO;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public abstract class FileCopyCommandHandler<T> : ICommandHandler<T>
        where T : FileCommand
    {
        const string LOG_PREFIX = "FileCopyCommandHandler: ";
        readonly ILogger TheLogger;
        readonly IRebaser Rebaser;

        protected FileCopyCommandHandler(ILogger logger, IRebaser rebaser)
        {
            TheLogger = logger.ThrowIfNull("logger");
            Rebaser = rebaser.ThrowIfNull("rebaser");
        }

        public void Handle(T command)
        {
            command.ThrowIfNull("command");
            if (!File.Exists(command.Filename))
                return;

            string destFileName = Rebaser.RebaseFileNameIntoOutputDirectory(command.Filename);
            FileUtils.EnsureParentDirectory(destFileName);
            File.Copy(command.Filename, destFileName, true);
            TheLogger.LogMessage(LOG_PREFIX + "Copied {0} to {1}", command.Filename, destFileName);
        }
    }
}
