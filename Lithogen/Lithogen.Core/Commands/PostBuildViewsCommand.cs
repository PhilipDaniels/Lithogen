using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostBuildViews")]
    public class PostBuildViewsCommand : PostCommand<BuildViewsCommand>
    {
        public PostBuildViewsCommand(BuildViewsCommand command)
            : base(command)
        {
        }
    }
}
