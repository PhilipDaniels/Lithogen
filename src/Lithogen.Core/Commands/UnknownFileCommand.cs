using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("UnknownFile: {FileName}")]
    public class UnknownFileCommand : FileCommand
    {
        public UnknownFileCommand(string fileName)
            : base(fileName)
        {
        }
    }
}
