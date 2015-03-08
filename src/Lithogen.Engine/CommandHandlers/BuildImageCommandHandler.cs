using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandHandlers
{
    public class BuildImageCommandHandler : FileCopyCommandHandler<BuildImageCommand>
    {
        public BuildImageCommandHandler(ILogger logger, IRebaser rebaser)
            : base(logger, rebaser)
        {
        }
    }
}
