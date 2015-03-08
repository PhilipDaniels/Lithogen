using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PreNpm")]
    public class PreNpmCommand : PreCommand<NpmCommand>
    {
        public PreNpmCommand(NpmCommand command)
            : base(command)
        {
        }
    }
}
