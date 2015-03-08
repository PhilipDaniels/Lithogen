using Lithogen.Core;
using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("{Filename}")]
    public abstract class FileCommand : ICommand
    {
        public string Filename { get; private set; }

        public FileCommand(string filename)
        {
            Filename = filename.ThrowIfNullOrWhiteSpace("filename");
        }
    }
}
