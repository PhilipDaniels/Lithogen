using System.Diagnostics;
using BassUtils;

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
