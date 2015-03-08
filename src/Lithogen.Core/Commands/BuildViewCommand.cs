using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildView: {Filename}")]
    public class BuildViewCommand : FileCommand
    {
        public BuildViewCommand(string filename)
            : base(filename)
        {
        }
    }
}
