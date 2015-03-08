using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildImage: {Filename}")]
    public class BuildImageCommand : FileCommand
    {
        public BuildImageCommand(string filename)
            : base(filename)
        {
        }
    }
}
