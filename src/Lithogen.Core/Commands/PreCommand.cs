using BassUtils;

namespace Lithogen.Core.Commands
{
    public abstract class PreCommand<T> : IPreCommand<T>
        where T : ICommand
    {
        public bool Handled { get; set; }
        public T Command { get; private set; }

        protected PreCommand(T command)
        {
            Command = command.ThrowIfNull("command");
        }
    }
}
