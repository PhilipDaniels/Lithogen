namespace Lithogen.Core.Interfaces
{
    public interface IViewPipeline
    {
        void ProcessFile(string filename);
        void ProcessDirectory(string viewsDirectory);
    }
}
