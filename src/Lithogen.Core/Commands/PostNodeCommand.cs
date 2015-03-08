using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostNode")]
    public class PostNodeCommand : PostCommand<NodeCommand>
    {
        public PostNodeCommand(NodeCommand command)
            : base(command)
        {
        }
    }
}
