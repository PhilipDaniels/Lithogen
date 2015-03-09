using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildView: {FileName}")]
    public class BuildViewCommand : FileCommand
    {
        public BuildViewCommand(string fileName)
            : base(fileName)
        {
        }
    }
}
