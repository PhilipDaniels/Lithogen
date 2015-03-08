using System.IO;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class NpmCommandHandler : ICommandHandler<NpmCommand>
    {
        readonly IProcessRunner ProcessRunner;

        public NpmCommandHandler(IProcessRunner processRunner)
        {
            ProcessRunner = processRunner.ThrowIfNull("processRunner");
        }

        public void Handle(NpmCommand command)
        {
            command.ThrowIfNull("command");
            command.NodeExePath.ThrowIfFileDoesNotExist("command.NodeExePath");
            command.PackageJsonPath.ThrowIfFileDoesNotExist("command.PackageJsonPath");

            string npmPath = Path.GetDirectoryName(command.NodeExePath);
            npmPath = Path.Combine(npmPath, @"node_modules\npm\bin\npm-cli.js");
            string args = command.Arguments == null ? "" : command.Arguments.Trim();
            string workingDir = Path.GetDirectoryName(command.PackageJsonPath);

            using (var p = ProcessRunner.MakeProcess(command.NodeExePath, npmPath + " " + args, workingDir))
            {
                ProcessRunner.Execute(p);
            }
        }
    }
}
