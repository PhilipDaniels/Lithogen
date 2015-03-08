using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostBuildView")]
    public class PostBuildViewCommand : PostCommand<BuildViewCommand>
    {
        public PostBuildViewCommand(BuildViewCommand command)
            : base(command)
        {
        }
    }
}
