namespace Lithogen.Core
{
    public interface IPostCommand<T> : ICommand
        where T : ICommand
    {
        T Command { get; }
    }
}
