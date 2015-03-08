using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostFileDelete")]
    public class PostFileDeleteCommand : PostCommand<FileDeleteCommand>
    {
        public PostFileDeleteCommand(FileDeleteCommand command)
            : base(command)
        {
        }
    }
}
