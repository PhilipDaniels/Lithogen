using Lithogen.Core;
using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("Npm: {Arguments}")]
    public class NpmCommand : NodeCommand
    {
        public string PackageJsonPath { get; private set; }

        public NpmCommand(string nodeExePath, string arguments, string packageJsonPath)
            : base(nodeExePath, arguments)
        {
            PackageJsonPath = packageJsonPath.ThrowIfNullOrWhiteSpace("packageJsonPath");
        }
    }
}
