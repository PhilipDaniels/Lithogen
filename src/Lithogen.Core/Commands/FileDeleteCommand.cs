using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("FileDelete: {Filename}")]
    public class FileDeleteCommand : FileCommand
    {
        public FileDeleteCommand(string filename)
            : base(filename)
        {
        }
    }
}
