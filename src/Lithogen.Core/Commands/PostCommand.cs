namespace Lithogen.Core.Commands
{
    public abstract class PostCommand<T> : IPostCommand<T>
        where T : ICommand
    {
        public T Command { get; private set; }

        protected PostCommand(T command)
        {
            Command = command.ThrowIfNull("command");
        }
    }
}
