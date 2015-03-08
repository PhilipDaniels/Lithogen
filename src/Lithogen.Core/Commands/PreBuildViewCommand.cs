using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreBuildView")]
    public class PreBuildViewCommand : PreCommand<BuildViewCommand>
    {
        public PreBuildViewCommand(BuildViewCommand command)
            : base(command)
        {
        }
    }
}
