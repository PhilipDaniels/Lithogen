using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreClean")]
    public class PreCleanCommand : PreCommand<CleanCommand>
    {
        public PreCleanCommand(CleanCommand command)
            : base(command)
        {
        }
    }
}
