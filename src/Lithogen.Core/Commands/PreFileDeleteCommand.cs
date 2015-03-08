using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreFileDelete")]
    public class PreFileDeleteCommand : PreCommand<FileDeleteCommand>
    {
        public PreFileDeleteCommand(FileDeleteCommand command)
            : base(command)
        {
        }
    }
}
