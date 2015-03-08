using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("UnknownFile: {Filename}")]
    public class UnknownFileCommand : FileCommand
    {
        public UnknownFileCommand(string filename)
            : base(filename)
        {
        }
    }
}
