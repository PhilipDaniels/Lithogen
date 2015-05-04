using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostBuildImage")]
    public class PostBuildImageCommand : PostCommand<BuildImageCommand>
    {
        public PostBuildImageCommand(BuildImageCommand command)
            : base(command)
        {
        }
    }
}
