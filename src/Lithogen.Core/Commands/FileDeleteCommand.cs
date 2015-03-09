using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("FileDelete: {FileName}")]
    public class FileDeleteCommand : FileCommand
    {
        public FileDeleteCommand(string fileName)
            : base(fileName)
        {
        }
    }
}
