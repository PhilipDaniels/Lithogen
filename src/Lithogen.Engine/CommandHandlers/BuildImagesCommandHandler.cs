using System.IO;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;
using Microsoft.VisualBasic.FileIO;

namespace Lithogen.Engine.CommandHandlers
{
    public class BuildImagesCommandHandler : ICommandHandler<BuildImagesCommand>
    {
        const string LOG_PREFIX = "BuildImagesCommandHandler: ";
        readonly ILogger TheLogger;

        public BuildImagesCommandHandler(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        public void Handle(BuildImagesCommand command)
        {
            command.ThrowIfNull("command");
            command.WebsiteTargetDirectory.ThrowIfNullOrWhiteSpace("command.WebsiteTargetDirectory");
            command.ImagesDirectory.ThrowIfDirectoryDoesNotExist("command.ImagesDirectory");

            string destDir = Path.Combine(command.WebsiteTargetDirectory, Path.GetFileName(command.ImagesDirectory));
            FileSystem.CopyDirectory(command.ImagesDirectory, destDir, true);
            TheLogger.LogMessage(LOG_PREFIX + "Recursively copied {0} to {1}", command.ImagesDirectory, destDir);
        }
    }
}
