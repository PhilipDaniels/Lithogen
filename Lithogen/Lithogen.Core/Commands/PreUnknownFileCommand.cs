using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreUnknownFile")]
    public class PreUnknownFileCommand : PreCommand<UnknownFileCommand>
    {
        public PreUnknownFileCommand(UnknownFileCommand command)
            : base(command)
        {
        }
    }
}
