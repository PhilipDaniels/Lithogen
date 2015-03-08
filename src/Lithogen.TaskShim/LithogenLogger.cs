using Lithogen.Core;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Lithogen.TaskShim
{
    /// <summary>
    /// The LithogenLogger just writes messages to the Visual Studio log.
    /// It does not attempt to write to the Lithogen.log file, because this can
    /// cause it to be locked, causing an exception when Lithogen.exe
    /// attempts to log to it.
    /// </summary>
    class LithogenLogger : LoggerBase
    {
        readonly TaskLoggingHelper Log;
        readonly MessageImportance MessageImportance;

        public LithogenLogger(TaskLoggingHelper log, MessageImportance messageImportance)
        {
            Log = log;
            MessageImportance = messageImportance;
        }

        protected override void WriteMessage(string message)
        {
            Log.LogMessage(MessageImportance, message);
        }

        protected override void WriteError(string message)
        {
            Log.LogError(message);
        }

        public override bool HasLoggedErrors
        {
            get { return Log.HasLoggedErrors; }
        }
    }
}
