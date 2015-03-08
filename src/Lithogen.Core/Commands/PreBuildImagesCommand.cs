using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreBuildImages")]
    public class PreBuildImagesCommand : PreCommand<BuildImagesCommand>
    {
        public PreBuildImagesCommand(BuildImagesCommand command)
            : base(command)
        {
        }
    }
}
