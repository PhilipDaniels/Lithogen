using System.Collections.Generic;

namespace Lithogen.Core.Interfaces
{
    /// <summary>
    /// Interface exists so that we can return an immutable object from the CommandLineParser.
    /// </summary>
    public interface ICommandLineArgs
    {
        bool Help { get; set; }
        bool FullHelp { get; set; }
        string HelpMessage { get; }
        LoggingLevel LoggingLevel { get; }
        string GenProjectFile { get; }
        string SettingsFile { get; }
        bool Clean { get; }
        bool Serve { get; }
        bool Watch { get; }
        IEnumerable<string> BuildSteps { get; }
        IEnumerable<string> Files { get; }
        short Port { get; }
        int ViewDOP { get; }
    }
}
