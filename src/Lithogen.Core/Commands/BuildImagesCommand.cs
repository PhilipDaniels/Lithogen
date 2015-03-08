using Lithogen.Core;
using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildImages: Copy from {ImagesDirectory}")]
    public class BuildImagesCommand : ICommand
    {
        public string WebsiteTargetDirectory { get; private set; }
        public string ImagesDirectory { get; private set; }

        public BuildImagesCommand(string websiteTargetDirectory, string imagesDirectory)
        {
            WebsiteTargetDirectory = websiteTargetDirectory.ThrowIfNullOrWhiteSpace("websiteTargetDirectory");
            ImagesDirectory = imagesDirectory.ThrowIfDirectoryDoesNotExist("imagesDirectory");
        }
    }
}
