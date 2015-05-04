using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostCreateDirectory")]
    public class PostCreateDirectoryCommand : PostCommand<CreateDirectoryCommand>
    {
        public PostCreateDirectoryCommand(CreateDirectoryCommand command)
            : base(command)
        {
        }
    }
}
