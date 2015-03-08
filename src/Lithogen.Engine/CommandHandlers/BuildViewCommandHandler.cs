using System.IO;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class BuildViewCommandHandler : ICommandHandler<BuildViewCommand>
    {
        readonly IViewPipeline ViewPipeline;

        public BuildViewCommandHandler(IViewPipeline viewPipeline)
        {
            ViewPipeline = viewPipeline.ThrowIfNull("viewPipeline");
        }

        public void Handle(BuildViewCommand command)
        {
            command.ThrowIfNull("command");
            if (!File.Exists(command.Filename))
                return;

            ViewPipeline.ProcessFile(command.Filename);
        }
    }
}
