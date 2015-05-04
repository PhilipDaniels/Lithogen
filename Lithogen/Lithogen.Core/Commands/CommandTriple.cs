namespace Lithogen.Core.Commands
{
    public interface ICommandTriple
    {
    }

    public class CommandTriple<T> : ICommandTriple, ICommand
        where T : ICommand
    {
        public IPreCommand<T> PreCommand { get; set; }
        public T Command { get; set; }
        public IPostCommand<T> PostCommand { get; set; }
    }
}
