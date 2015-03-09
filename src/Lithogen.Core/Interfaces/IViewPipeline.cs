namespace Lithogen.Core.Interfaces
{
    public interface IViewPipeline
    {
        void ProcessFile(string fileName);
        void ProcessDirectory(string viewsDirectory);
    }
}
