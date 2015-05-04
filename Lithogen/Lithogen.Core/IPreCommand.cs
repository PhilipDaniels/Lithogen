namespace Lithogen.Core
{
    public interface IPreCommand<T> : ICommand
        where T : ICommand
    {
        bool Handled { get; set; }
        T Command { get; }
    }
}
