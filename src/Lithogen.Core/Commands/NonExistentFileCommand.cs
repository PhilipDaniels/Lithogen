using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("NonexistentFile: {FileName}")]
    public class NonexistentFileCommand : FileCommand
    {
        public NonexistentFileCommand(string fileName)
            : base(fileName)
        {
        }
    }
}
