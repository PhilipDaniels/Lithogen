using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// Runs a child process, capturing its stdout and stderr and
    /// sending them to the log.
    /// </summary>
    public class ProcessRunner : IProcessRunner
    {
        const string LOG_PREFIX = "ProcessRunner: ";
        readonly ILogger TheLogger;

        public ProcessRunner(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
        }

        /// <summary>
        /// Construct a new process in a form that will allow us to capture its output.
        /// </summary>
        /// <param name="exePath">Path to the exe to run.</param>
        /// <param name="arguments">Arguments to the exe.</param>
        /// <param name="workingDirectory">Working directory, pass null to default to the directory the exe is in.</param>
        /// <returns>Process object. Not yet started.</returns>
        public Process MakeProcess(string exePath, string arguments, string workingDirectory = null)
        {
            exePath = exePath.ThrowIfFileDoesNotExist("exePath");
            
            arguments = (arguments ?? "").Trim();
            if (workingDirectory == null)
            {
                workingDirectory = Path.GetDirectoryName(exePath);
            }
            else
            {
                workingDirectory = workingDirectory.ThrowIfNullOrWhiteSpace("workingDirectory").Trim();
                workingDirectory.ThrowIfDirectoryDoesNotExist("workingDirectory");
            }

            string pathDir = Path.GetDirectoryName(exePath);
            if (!workingDirectory.Equals(pathDir, StringComparison.OrdinalIgnoreCase))
                pathDir += ";" + workingDirectory;

            var p = new Process();
            p.StartInfo.FileName = exePath;
            p.StartInfo.WorkingDirectory = workingDirectory;
            p.StartInfo.EnvironmentVariables["PATH"] += ";" + pathDir;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.Arguments = arguments;
            
            return p;
        }

        /// <summary>
        /// Executes the specified process and captures its stdout and stderr and
        /// sends them to the logger.
        /// </summary>
        /// <param name="process">Process to execute.</param>
        public void Execute(Process process)
        {
            process.ThrowIfNull("process");

            process.OutputDataReceived += LogMultilineMessage;
            process.ErrorDataReceived += LogMultilineError;
            process.EnableRaisingEvents = true;

            TheLogger.LogMessage(LOG_PREFIX + "Working directory = " + process.StartInfo.WorkingDirectory);
            string msg = String.Format(CultureInfo.InvariantCulture, "  Attempting to start {0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments).Trim();
            TheLogger.LogMessage(LOG_PREFIX + msg);

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
            process.CancelErrorRead();
        }

        void LogMultilineMessage(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data))
                TheLogger.LogMultilineMessage(LOG_PREFIX + e.Data);
        }

        void LogMultilineError(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data))
                TheLogger.LogMultilineError(LOG_PREFIX + e.Data);
        }
    }
}
