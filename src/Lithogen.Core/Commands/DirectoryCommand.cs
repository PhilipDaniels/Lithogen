using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("{Directory}")]
    public abstract class DirectoryCommand : ICommand
    {
        public string Directory { get; private set; }

        protected DirectoryCommand(string directory)
        {
            Directory = directory.ThrowIfNullOrWhiteSpace("directory");
        }
    }
}
