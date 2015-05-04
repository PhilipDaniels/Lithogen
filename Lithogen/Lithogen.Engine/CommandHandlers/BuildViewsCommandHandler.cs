using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class BuildViewsCommandHandler : ICommandHandler<BuildViewsCommand>
    {
        readonly IViewPipeline ViewPipeline;

        public BuildViewsCommandHandler(IViewPipeline viewPipeline)
        {
            ViewPipeline = viewPipeline.ThrowIfNull("viewPipeline");
        }

        public void Handle(BuildViewsCommand command)
        {
            command.ThrowIfNull("command");
            command.Directory.ThrowIfDirectoryDoesNotExist("command.Directory");

            ViewPipeline.ProcessDirectory(command.Directory);
        }
    }
}
