using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreBuildImage")]
    public class PreBuildImageCommand : PreCommand<BuildImageCommand>
    {
        public PreBuildImageCommand(BuildImageCommand command)
            : base(command)
        {
        }
    }
}
