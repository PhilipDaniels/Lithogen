using Lithogen.Core;
using System;
using System.Diagnostics;

namespace Lithogen.Core.Commands
{
    [DebuggerDisplay("BuildAssets: {ContentDirectory}, {ScriptsDirectory}")]
    public class BuildAssetsCommand : ICommand
    {
        public string NodeExePath { get; private set; }
        public string ContentDirectory { get; private set; }
        public string ScriptsDirectory { get; private set; }
        public string WebsiteTargetDirectory { get; private set; }
        public string ProjectDirectory { get; private set; }

        public BuildAssetsCommand
            (
            string nodeExePath,
            string contentDirectory,
            string scriptsDirectory,
            string websiteTargetDirectory,
            string projectDirectory
            )
        {
            NodeExePath = nodeExePath.ThrowIfFileDoesNotExist("nodeExePath");
            if (contentDirectory == null && scriptsDirectory == null)
                throw new ArgumentException("At least one of contentDirectory and scriptsDirectory must be set.");
            ContentDirectory = contentDirectory;
            ScriptsDirectory = scriptsDirectory;
            WebsiteTargetDirectory = websiteTargetDirectory.ThrowIfNullOrWhiteSpace("websiteTargetDirectory");
            ProjectDirectory = projectDirectory.ThrowIfDirectoryDoesNotExist("projectDirectory");
        }
    }
}
