using System;

namespace Lithogen.Core.Interfaces
{
    public interface ILogger
    {
        bool HasLoggedErrors { get; }
        LoggingLevel LoggingLevel { get; set; }

        void LogMessage(string message, params object[] args);
        void LogMessageWithoutPrefix(string message, params object[] args);
        void LogMultilineMessage(string message);

        void LogVerbose(string message, params object[] args);

        void LogError(string message, params object[] args);
        void LogErrorWithoutPrefix(string message, params object[] args);
        void LogMultilineError(string message);
    }
}
