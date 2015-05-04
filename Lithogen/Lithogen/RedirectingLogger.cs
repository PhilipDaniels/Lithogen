using System;
using System.IO;
using System.Threading;
using BassUtils;
using Lithogen.Core;

namespace Lithogen
{
    /// <summary>
    /// The RedirectingLogger writes messages to the standard output/error, where they will be
    /// picked up by the Lithogen shim; plus it copies everything to a file.
    /// </summary>
    class RedirectingLogger : LoggerBase
    {
        readonly string LogFileName;

        // We need to be multi-threading safe because we are a singleton, returned
        // by SimpleInjector.
        ReaderWriterLockSlim padlock = new ReaderWriterLockSlim();

        public RedirectingLogger(string logFileName)
        {
            LogFileName = logFileName.ThrowIfNullOrWhiteSpace("logFileName");
        }

        protected override void WriteMessage(string message)
        {
            message.ThrowIfNull("message");

            // Already thread-safe.
            Console.WriteLine(message);

            padlock.EnterWriteLock();
            try
            {
                using (StreamWriter sw = File.AppendText(LogFileName))
                {
                    sw.WriteLine(message);
                }
            }
            finally
            {
                padlock.ExitWriteLock();
            }
        }

        protected override void WriteError(string message)
        {
            message.ThrowIfNull("message");

            _HasLoggedErrors = true;

            // Already thread-safe.
            Console.Error.WriteLine(message);

            padlock.EnterWriteLock();
            try
            {
                using (StreamWriter sw = File.AppendText(LogFileName))
                {
                    sw.WriteLine(message);
                }
            }
            finally
            {
                padlock.ExitWriteLock();
            }
        }

        public override bool HasLoggedErrors
        {
            get { return _HasLoggedErrors; }
        }
        bool _HasLoggedErrors;
    }
}
