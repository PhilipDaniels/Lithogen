using System.IO;
using System.Linq;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Gitcheatsheet.CommandHandlers
{
    public class PreBuildAssetsCommandHandler : ICommandHandler<PreBuildAssetsCommand>
    {
        const string LOG_PREFIX = "PreBuildAssetsCommandHandler: ";
        readonly ILogger TheLogger;
        readonly ISettings TheSettings;
        readonly IRebaser Rebaser;

        public PreBuildAssetsCommandHandler(ILogger logger, ISettings settings, IRebaser rebaser)
        {
            TheLogger = logger.ThrowIfNull("logger");
            TheSettings = settings.ThrowIfNull("settings");
            Rebaser = rebaser.ThrowIfNull("rebaser");
        }

        public void Handle(PreBuildAssetsCommand command)
        {
            command.ThrowIfNull("command");

            var themes = Directory.GetFiles(TheSettings.ContentDirectory, "theme*.css");
            foreach (var themeFile in themes)
            {
                string destThemeFile = Rebaser.RebaseFileNameIntoOutputDirectory(themeFile);
                FileUtils.EnsureParentDirectory(destThemeFile);
                File.Copy(themeFile, destThemeFile, true);
            }

            TheLogger.LogMessage(LOG_PREFIX + "Copied {0} themes to {1}", themes.Count(), TheSettings.ContentDirectory);
        }
    }
}
