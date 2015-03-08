using System.Diagnostics;
using Lithogen.Core;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("Arguments: {Arguments}")]
    public abstract class ArgumentsCommand : ICommand
    {
        public string Arguments { get; private set; }

        protected ArgumentsCommand(string arguments)
        {
            if (arguments != null)
                Arguments = arguments.Trim();
        }
    }
}
