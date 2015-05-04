using System.Diagnostics;
using BassUtils;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("{FileName}")]
    public abstract class FileCommand : ICommand
    {
        public string FileName { get; private set; }

        protected FileCommand(string fileName)
        {
            FileName = fileName.ThrowIfNullOrWhiteSpace("fileName");
        }
    }
}
