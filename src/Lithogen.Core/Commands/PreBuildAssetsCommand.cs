using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreBuildAssets")]
    public class PreBuildAssetsCommand : PreCommand<BuildAssetsCommand>
    {
        public PreBuildAssetsCommand(BuildAssetsCommand command)
            : base(command)
        {
        }
    }
}
