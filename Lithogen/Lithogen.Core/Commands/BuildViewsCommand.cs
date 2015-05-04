using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildViews: {Directory}")]
    public class BuildViewsCommand : DirectoryCommand
    {
        public BuildViewsCommand(string directory)
            : base(directory)
        {
        }
    }
}
