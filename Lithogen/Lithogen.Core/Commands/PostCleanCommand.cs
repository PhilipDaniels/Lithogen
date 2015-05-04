using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostClean")]
    public class PostCleanCommand : PostCommand<CleanCommand>
    {
        public PostCleanCommand(CleanCommand command)
            : base(command)
        {
        }
    }
}
