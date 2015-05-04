using Lithogen.Core.Interfaces;
using System;
using System.Text;
using System.Threading;
using System.Globalization;

namespace Lithogen.Core
{
    /// <summary>
    /// Base class for Lithogen logging. Provides a running time count, a count of
    /// errors written and nicely formatted messages. Distinguishes between messages
    /// and errors to simplify things for MSBuild loggers.
    /// All actual logging is deferred to concrete subclasses.
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        public LoggingLevel LoggingLevel { get; set; }

        protected abstract void WriteMessage(string message);
        protected abstract void WriteError(string message);
        public abstract bool HasLoggedErrors { get; }

        readonly DateTime StartTime;
        DateTime LastTime;

        protected LoggerBase()
        {
            StartTime = DateTime.Now;
            LastTime = StartTime;
            LoggingLevel = LoggingLevel.Normal;
        }

        /// <summary>
        /// Logs a standard message, including time formatting.
        /// This is completely free-format.
        /// </summary>
        /// <param name="message">Format text of the message.</param>
        /// <param name="args">Any parameters.</param>
        public virtual void LogMessage(string message, params object[] args)
        {
            LogMessageImpl(LoggingLevel.Normal, true, message, args);
        }

        /// <summary>
        /// Logs a standard message, including time formatting.
        /// This is completely free-format. The message does not have the timestamp
        /// or prefix.
        /// </summary>
        /// <param name="message">Format text of the message.</param>
        /// <param name="args">Any parameters.</param>
        public virtual void LogMessageWithoutPrefix(string message, params object[] args)
        {
            LogMessageImpl(LoggingLevel.Normal, false, message, args);
        }

        /// <summary>
        /// Logs a multi-line message by splitting a single string into multiple
        /// lines at '\r' characters then logging each one individually.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public virtual void LogMultilineMessage(string message)
        {
            foreach (var m in SplitMultilineString(message))
                LogMessage(message);
        }

        /// <summary>
        /// Logs a message, but only if we are configured to be at the verbose logging level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="args">Any parameters.</param>
        public virtual void LogVerbose(string message, params object[] args)
        {
            LogMessageImpl(LoggingLevel.Verbose, true, message, args);
        }

        /// <summary>
        /// Logs an error message. This format is interpreted by MSBuild as an error
        /// and appears on the "Error List" window.
        /// </summary>
        /// <param name="message">Format text of the message.</param>
        /// <param name="args">Any parameters.</param>
        public virtual void LogError(string message, params object[] args)
        {
            LogErrorImpl(true, message, args);
        }

        /// <summary>
        /// Logs an error message. This format is interpreted by MSBuild as an error
        /// and appears on the "Error List" window. The message does not have the
        /// timestamp or prefix.
        /// </summary>
        /// <param name="message">Format text of the message.</param>
        /// <param name="args">Any parameters.</param>
        public virtual void LogErrorWithoutPrefix(string message, params object[] args)
        {
            LogErrorImpl(false, message, args);
        }

        /// <summary>
        /// Logs a multi-line error by splitting a single string into multiple
        /// lines at '\r' characters then logging each one individually.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public virtual void LogMultilineError(string message)
        {
            foreach (var m in SplitMultilineString(message))
                LogError(message);
        }

        protected virtual string FormatMessage(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
                message = String.Format(CultureInfo.InvariantCulture, message, args);

            var sb = new StringBuilder();
            DateTime now = DateTime.Now;

            double thisSecs = (now - LastTime).TotalSeconds;
            double totalSecs = (now - StartTime).TotalSeconds;
            sb.Append("[");
            var tss = SecondsToString(thisSecs);
            if (String.IsNullOrWhiteSpace(tss))
                sb.Append(tss + " ");
            else
                sb.Append(tss + ",");
            sb.Append(SecondsToString(totalSecs));
            sb.Append("]");
            sb.AppendFormat(" [{0,2:#0}] {1}", Thread.CurrentThread.ManagedThreadId, message ?? "");

            return sb.ToString();
        }

        static string SecondsToString(double seconds)
        {
            if (seconds < 0.05)
                return "      ";
            else if (seconds > 999.99)
                return String.Format(CultureInfo.InvariantCulture, "{0,6:######}", seconds);
            else
                return String.Format(CultureInfo.InvariantCulture, "{0,6:##0.00}", seconds);
        }

        void LogMessageImpl(LoggingLevel requestedLevel, bool includePrefix, string message, params object[] args)
        {
            // Is logging off altogether?
            if (LoggingLevel == LoggingLevel.Off)
                return;
            // Is the user asking for verbose when we are at normal?
            if (LoggingLevel == LoggingLevel.Normal && requestedLevel == LoggingLevel.Verbose)
                return;

            if (includePrefix)
            {
                message = FormatMessage(message, args);
            }
            else
            {
                if (args != null && args.Length > 0)
                    message = String.Format(CultureInfo.InvariantCulture, message, args);
            }

            LastTime = DateTime.Now;
            WriteMessage(message);
        }

        void LogErrorImpl(bool includePrefix, string message, params object[] args)
        {
            if (includePrefix)
            {
                message = FormatMessage(message, args);
            }
            else
            {
                if (args != null && args.Length > 0)
                    message = String.Format(CultureInfo.InvariantCulture, message, args);
            }

            LastTime = DateTime.Now;
            WriteError(message);
        }

        static string[] SplitMultilineString(string msg)
        {
            if (String.IsNullOrWhiteSpace(msg))
                return new string[] { };
            else if (msg.Contains(@"\n"))
                return msg.Split('\n');
            else
                return new string[] { msg };
        }
    }
}
