using System;
using System.Globalization;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class BuildAssetsCommandHandler : ICommandHandler<BuildAssetsCommand>
    { 
        readonly IProcessRunner ProcessRunner;

        public BuildAssetsCommandHandler(IProcessRunner processRunner)
        {
            ProcessRunner = processRunner.ThrowIfNull("processRunner");
        }

        public void Handle(BuildAssetsCommand command)
        {
            command.ThrowIfNull("command");
            if (command.ContentDirectory == null && command.ScriptsDirectory == null)
                throw new ArgumentException("At least one of contentDirectory and scriptsDirectory must be set.");
            command.NodeExePath.ThrowIfFileDoesNotExist("command.NodeExePath");
            command.WebsiteTargetDirectory.ThrowIfDirectoryDoesNotExist("command.WebsiteTargetDirectory");
            command.ProjectDirectory.ThrowIfDirectoryDoesNotExist("command.ProjectDirectory");

            string args = String.Format
                (
                CultureInfo.InvariantCulture,
                @"bundler.js #outputFolder:{0} #baseFolder:{1}",
                command.WebsiteTargetDirectory, command.ProjectDirectory
                );

            if (!String.IsNullOrWhiteSpace(command.ContentDirectory))
                args += " " + command.ContentDirectory;
            if (!String.IsNullOrWhiteSpace(command.ScriptsDirectory))
                args += " " + command.ScriptsDirectory;

            using (var p = ProcessRunner.MakeProcess(command.NodeExePath, args))
            {
                ProcessRunner.Execute(p);
            }
        }
    }
}
