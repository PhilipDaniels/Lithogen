using System.Diagnostics;

namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Runs a child process, capturing its stdout and stderr and
    /// sending them to the log.
    /// </summary>
    public interface IProcessRunner
    {
        /// <summary>
        /// Construct a new process in a form that will allow us to capture its output.
        /// </summary>
        /// <param name="exePath">Path to the exe to run.</param>
        /// <param name="arguments">Arguments to the exe.</param>
        /// <param name="workingDirectory">Working directory, pass null to default to the directory the exe is in.</param>
        /// <returns>Process object. Not yet started.</returns>
        Process MakeProcess(string exePath, string arguments, string workingDirectory = null);

        /// <summary>
        /// Executes the specified process and captures its stdout and stderr and
        /// sends them to the logger.
        /// </summary>
        /// <param name="process">Process to execute.</param>
        void Execute(Process process);
    }
}
