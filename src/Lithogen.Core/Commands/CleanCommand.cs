using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("Clean: {Directory}")]
    public class CleanCommand : DirectoryCommand
    {
        public CleanCommand(string directory)
            : base(directory)
        {
        }
    }
}
