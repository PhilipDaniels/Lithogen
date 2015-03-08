using Lithogen.Core;
using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildComplete")]
    public class BuildCompleteCommand : ICommand
    {
        public string WebsiteTargetDirectory { get; private set; }

        public BuildCompleteCommand(string websiteTargetDirectory)
        {
            WebsiteTargetDirectory = websiteTargetDirectory.ThrowIfNullOrWhiteSpace("websiteTargetDirectory");
        }
    }
}
