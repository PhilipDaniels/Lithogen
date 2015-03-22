using System.Diagnostics;
using BassUtils;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("Node: {Arguments}")]
    public class NodeCommand : ArgumentsCommand
    {
        public string NodeExePath { get; private set; }

        public NodeCommand(string nodeExePath, string arguments)
            : base(arguments)
        {
            NodeExePath = nodeExePath.ThrowIfFileDoesNotExist("nodeExePath");
        }
    }
}
