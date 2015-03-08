using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class NodeCommandHandler : ICommandHandler<NodeCommand>
    {
        readonly IProcessRunner ProcessRunner;

        public NodeCommandHandler(IProcessRunner processRunner)
        {
            ProcessRunner = processRunner.ThrowIfNull("processRunner");
        }

        public void Handle(NodeCommand command)
        {
            command.ThrowIfNull("command");
            command.NodeExePath.ThrowIfNullOrWhiteSpace("command.NodeExePath");

            string args = command.Arguments == null ? "" : command.Arguments.Trim();
            using (var p = ProcessRunner.MakeProcess(command.NodeExePath, args))
            {
                ProcessRunner.Execute(p);
            }
        }
    }
}
