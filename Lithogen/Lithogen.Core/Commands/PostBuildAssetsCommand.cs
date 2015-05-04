using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostBuildAssets")]
    public class PostBuildAssetsCommand : PostCommand<BuildAssetsCommand>
    {
        public PostBuildAssetsCommand(BuildAssetsCommand command)
            : base(command)
        {
        }
    }
}
