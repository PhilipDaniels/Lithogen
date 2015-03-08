using System.Diagnostics;
using Lithogen.Core;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("Message: {Message}")]
    public class MessageCommand : ICommand
    {
        public string Message { get; private set; }

        public MessageCommand()
        {
        }

        public MessageCommand(string message)
        {
            Message = message;
        }
    }
}
