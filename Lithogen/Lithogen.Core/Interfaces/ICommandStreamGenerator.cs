using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System.Collections.Generic;

namespace Lithogen.Core.Interfaces
{
    public interface ICommandStreamGenerator
    {
        /// <summary>
        /// Gets the commands implied by a Lithogen command line.
        /// </summary>
        /// <param name="args">Command line arguments object.</param>
        /// <returns>List of commands.</returns>
        IEnumerable<ICommand> GetCommands(ICommandLineArgs args);

        /// <summary>
        /// Gets the build/clean commands implied by a set of files.
        /// </summary>
        /// <param name="files">The set of files.</param>
        /// <returns>List of commands.</returns>
        IEnumerable<ICommand> GetFileCommands(IEnumerable<string> files);

        /// <summary>
        /// Gets the build/clean commands implied by a set of files.
        /// </summary>
        /// <param name="files">The set of files.</param>
        /// <returns>List of commands.</returns>
        IEnumerable<ICommand> GetFileCommands(IEnumerable<FileNotification> files);
    }
}
