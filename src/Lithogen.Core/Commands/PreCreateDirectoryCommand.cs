using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreCreateDirectory")]
    public class PreCreateDirectoryCommand : PreCommand<CreateDirectoryCommand>
    {
        public PreCreateDirectoryCommand(CreateDirectoryCommand command)
            : base(command)
        {
        }
    }
}
