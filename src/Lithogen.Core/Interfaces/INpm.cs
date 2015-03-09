namespace Lithogen.Core.Interfaces
{
    public interface INpm
    {
        bool InstallIsRequired(string packageJsonFileName);
        void PerformInstall(string nodeExePath, string packageJsonFileName);
        string LastRunFileName(string packageJsonFileName);
    }
}
