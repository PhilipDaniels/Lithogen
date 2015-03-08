using Lithogen.Core;
using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("NonExistentFile: {Filename}")]
    public class NonExistentFileCommand : FileCommand
    {
        public NonExistentFileCommand(string filename)
            : base(filename)
        {
        }
    }
}
