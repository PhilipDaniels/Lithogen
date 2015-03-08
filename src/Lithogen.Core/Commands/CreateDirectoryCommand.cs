using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("CreateDirectoryCommand: {Directory}")]
    public class CreateDirectoryCommand : DirectoryCommand
    {
        public CreateDirectoryCommand(string directory)
            : base(directory)
        {
        }
    }
}
