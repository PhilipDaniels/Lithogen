using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreNode")]
    public class PreNodeCommand : PreCommand<NodeCommand>
    {
        public PreNodeCommand(NodeCommand command)
            : base(command)
        {
        }
    }
}
