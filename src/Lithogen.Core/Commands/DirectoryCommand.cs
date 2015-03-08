using Lithogen.Core;
using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("{Directory}")]
    public abstract class DirectoryCommand : ICommand
    {
        public string Directory { get; private set; }

        public DirectoryCommand(string directory)
        {
            Directory = directory.ThrowIfNullOrWhiteSpace("directory");
        }
    }
}
