using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostBuildImages")]
    public class PostBuildImagesCommand : PostCommand<BuildImagesCommand>
    {
        public PostBuildImagesCommand(BuildImagesCommand command)
            : base(command)
        {
        }
    }
}
