namespace Lithogen.Core.Interfaces
{
    public interface INpm
    {
        bool InstallIsRequired(string packageJsonFilename);
        void PerformInstall(string nodeExePath, string packageJsonFilename);
        string LastRunFilename(string packageJsonFilename);
    }
}
