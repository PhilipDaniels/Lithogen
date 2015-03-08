using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreBuildViews")]
    public class PreBuildViewsCommand : PreCommand<BuildViewsCommand>
    {
        public PreBuildViewsCommand(BuildViewsCommand command)
            : base(command)
        {
        }
    }
}
