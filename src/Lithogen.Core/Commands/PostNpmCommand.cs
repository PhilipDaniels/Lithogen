using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("PostNpm")]
    public class PostNpmCommand : PostCommand<NpmCommand>
    {
        public PostNpmCommand(NpmCommand command)
            : base(command)
        {
        }
    }
}
