using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostUnknownFile")]
    public class PostUnknownFileCommand : PostCommand<UnknownFileCommand>
    {
        public PostUnknownFileCommand(UnknownFileCommand command)
            : base(command)
        {
        }
    }
}
