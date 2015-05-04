using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildImage: {FileName}")]
    public class BuildImageCommand : FileCommand
    {
        public BuildImageCommand(string fileName)
            : base(fileName)
        {
        }
    }
}
