using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandLine
{
    [DebuggerDisplay("{OriginalArgsString}")]
    public class CommandLineParser
    {
        IStandardBuildSteps StandardBuildSteps;
        CommandLineArgs Cla;
        string[] OriginalArgs;

        public CommandLineParser(string commandLine)
        {
            if (!String.IsNullOrWhiteSpace(commandLine))
                OriginalArgs = commandLine.Split(' ');
            StandardBuildSteps = new StandardBuildSteps();
        }

        public CommandLineParser(string[] args)
        {
            OriginalArgs = args;
            StandardBuildSteps = new StandardBuildSteps();
        }

        /// <summary>
        /// The original set of arguments rendered as a single string.
        /// </summary>
        public string OriginalArgsString
        {
            get
            {
                if (OriginalArgs == null || OriginalArgs.Length == 0)
                    return "";
                else
                    return String.Join(" ", OriginalArgs);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification="Method must be bullet proof")]
        public ICommandLineArgs Parse(bool inServerMode, LoggingLevel currentLoggingLevel)
        {
            Cla = new CommandLineArgs();

            try
            {
                if (OriginalArgs != null)
                {
                    CheckForHelp();
                    if (Cla.Help) return Cla;
                    ParseLogLevel(inServerMode, currentLoggingLevel);
                    if (Cla.Help) return Cla;
                    ParseGen();
                    if (Cla.Help) return Cla;
                    ParseSettings();
                    if (Cla.Help) return Cla;
                    ParseClean();
                    if (Cla.Help) return Cla;
                    ParseBuild(inServerMode);
                    if (Cla.Help) return Cla;
                    ParseRebuild();
                    if (Cla.Help) return Cla;
                    ParseServe();
                    if (Cla.Help) return Cla;
                    ParseWatch();
                    if (Cla.Help) return Cla;

                    if (Cla.Watch && !Cla.Serve)
                    {
                        Cla.Help = true;
                        Cla.HelpMessage = "--watch requires --serve.";
                    }
                    ParsePort();
                    if (Cla.Help) return Cla;
                    ParseViewDOP();
                    if (Cla.Help) return Cla;

                    ParseFiles();
                }
            }
            catch (Exception ex)
            {
                // Never blow up for any user input.
                Cla.Help = true;
                Cla.HelpMessage = "Exception while parsing arguments: " + ex.ToString();
            }

            return Cla;
        }

        /// <summary>
        /// Returns the entire help text, as used when invoked from the command line.
        /// </summary>
        public static string FullHelpText
        {
            get
            {
                string usage = Assembly.GetExecutingAssembly().GetResourceAsString("CommandLine.Lithogen.usage.txt");
                return usage;
            }
        }

        /// <summary>
        /// Returns the help text down to and including the "Server Mode" section.
        /// </summary>
        public static string ShortHelpText
        {
            get
            {
                string t = FullHelpText.Before("Command Line Examples", StringComparison.OrdinalIgnoreCase).Trim();
                return t;
            }
        }

        #region Option Parsers
        void CheckForHelp()
        {
            Cla.Help = (from arg in NonFileArgs
                        where arg.Equals("-h", StringComparison.OrdinalIgnoreCase) ||
                              arg.Equals("--help", StringComparison.OrdinalIgnoreCase) ||
                              arg.Equals("h", StringComparison.OrdinalIgnoreCase) ||
                              arg.Equals("help", StringComparison.OrdinalIgnoreCase)
                        select arg).Any();

            Cla.FullHelp = (from arg in NonFileArgs
                            where arg.Equals("--fullhelp", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("fullhelp", StringComparison.OrdinalIgnoreCase)
                            select arg).Any();

            if (Cla.FullHelp)
                Cla.Help = false;
        }

        void ParseLogLevel(bool inServerMode, LoggingLevel currentLoggingLevel)
        {
            var logArgs = from arg in NonFileArgs
                           where arg.StartsWith("--log", StringComparison.OrdinalIgnoreCase) ||
                                 arg.StartsWith("log", StringComparison.OrdinalIgnoreCase)
                           select arg;

            if (logArgs.Count() == 0)
            {
                if (inServerMode)
                    Cla.LoggingLevel = currentLoggingLevel;
                else
                    Cla.LoggingLevel = LoggingLevel.Normal;
            }
            else if (logArgs.Count() == 1)
            {
                string val = (logArgs.ElementAt(0).After("=", StringComparison.OrdinalIgnoreCase) ?? "").Trim();
                if (val.Equals("quiet", StringComparison.OrdinalIgnoreCase))
                    Cla.LoggingLevel = LoggingLevel.Off;
                else if (val.Equals("normal", StringComparison.OrdinalIgnoreCase))
                    Cla.LoggingLevel = LoggingLevel.Normal;
                else if (val.Equals("verbose", StringComparison.OrdinalIgnoreCase))
                    Cla.LoggingLevel = LoggingLevel.Verbose;
                else
                {
                    Cla.Help = true;
                    Cla.HelpMessage = "Invalid logging level '" + val + "'. Valid values are quiet, normal and verbose.";
                }
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--log specified more than once.";
            }
        }

        void ParseGen()
        {
            var genArgs = from arg in NonFileArgs
                          where arg.StartsWith("--gen", StringComparison.OrdinalIgnoreCase)
                          select arg;

            if (genArgs.Count() == 0)
            {
                Cla.GenProjectFile = null;
            }
            else if (genArgs.Count() == 1)
            {
                Cla.GenProjectFile = genArgs.ElementAt(0).After("=", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--gen specified more than once.";
            }
        }

        void ParseSettings()
        {
            var settingsArgs = from arg in NonFileArgs
                               where arg.StartsWith("--settings", StringComparison.OrdinalIgnoreCase)
                               select arg;

            if (settingsArgs.Count() == 0)
            {
                // Default to Lithogen.xml near the exe.
                string dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                Cla.SettingsFile = Path.GetFullPath(Path.Combine(dir, "Lithogen.xml"));
            }
            else if (settingsArgs.Count() == 1)
            {
                Cla.SettingsFile = settingsArgs.ElementAt(0).After("=", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--settings specified more than once.";
            }
        }

        void ParseClean()
        {
            var cleanArgs = from arg in NonFileArgs
                            where arg.Equals("-c", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("--clean", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("c", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("clean", StringComparison.OrdinalIgnoreCase)
                            select arg;

            if (cleanArgs.Count() == 0)
            {
                Cla.Clean = false;
            }
            else if (cleanArgs.Count() == 1)
            {
                Cla.Clean = true;
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--clean specified more than once.";
            }
        }

        void ParseBuild(bool inServerMode)
        {
            // Check for build with no arguments.
            var buildArgs = from arg in NonFileArgs
                            where arg.Equals("-b", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("--build", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("b", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("build", StringComparison.OrdinalIgnoreCase) ||
                                  arg.StartsWith("-b=", StringComparison.OrdinalIgnoreCase) ||
                                  arg.StartsWith("--build=", StringComparison.OrdinalIgnoreCase) ||
                                  arg.StartsWith("b=", StringComparison.OrdinalIgnoreCase) ||
                                  arg.StartsWith("build=", StringComparison.OrdinalIgnoreCase)
                            select arg;

            if (buildArgs.Count() == 0)
            {
                // Use the defaults.
                if (!inServerMode)
                    ParseBuild(null);
            }
            else if (buildArgs.Count() == 1)
            {
                string steps = buildArgs.ElementAt(0).After("=", StringComparison.OrdinalIgnoreCase);
                ParseBuild(steps);
            }
            else if (buildArgs.Count() > 1)
            {
                Cla.Help = true;
                Cla.HelpMessage = "--build specified more than once.";
            }
        }

        void ParseBuild(string buildString)
        {
            if (buildString == null)
                buildString = "npm run build,csiv";

            // TODO: Need simple CSV parser to avoid problems where npm and node steps contain ','.
            string[] steps = buildString.Split(',');

            foreach (var step in steps)
            {
                string match = StandardBuildSteps.GetMatch(step);
                if (match != null)
                {
                    Cla.AddBuildStep(match);
                }
                else
                {
                    // Is this the case where the abbreviations are put together?
                    // Try and respect the order.
                    foreach (char c in step)
                    {
                        match = StandardBuildSteps.GetMatch(c.ToString());
                        if (match != null)
                        {
                            Cla.AddBuildStep(match);
                        }
                        else
                        {
                            Cla.Help = true;
                            Cla.HelpMessage = "Invalid build step: " + step;
                            Cla.ClearBuildSteps();
                            return;
                        }
                    }
                }
            }
        }

        void ParseRebuild()
        {
            var rebuildArgs = from arg in NonFileArgs
                            where arg.Equals("-r", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("--rebuild", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("r", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("rebuild", StringComparison.OrdinalIgnoreCase)
                            select arg;

            if (rebuildArgs.Count() == 0)
            {
                // Nothing to do.
            }
            else if (rebuildArgs.Count() == 1)
            {
                Cla.Clean = true;
                ParseBuild(null);
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--rebuild specified more than once.";
            }
        }

        void ParseServe()
        {
            var serveArgs = from arg in NonFileArgs
                            where arg.Equals("--serve", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("serve", StringComparison.OrdinalIgnoreCase)
                            select arg;

            if (serveArgs.Count() == 0)
            {
                Cla.Serve = false;
            }
            else if (serveArgs.Count() == 1)
            {
                Cla.Serve = true;
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--serve specified more than once.";
            }
        }

        void ParseWatch()
        {
            var watchArgs = from arg in NonFileArgs
                            where arg.Equals("--watch", StringComparison.OrdinalIgnoreCase) ||
                                  arg.Equals("watch", StringComparison.OrdinalIgnoreCase)
                            select arg;

            if (watchArgs.Count() == 0)
            {
                Cla.Watch = false;
            }
            else if (watchArgs.Count() == 1)
            {
                Cla.Watch = true;
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--watch specified more than once.";
            }
        }

        void ParsePort()
        {
            var portArgs = from arg in NonFileArgs
                           where arg.StartsWith("--port", StringComparison.OrdinalIgnoreCase)
                           select arg;

            if (portArgs.Count() == 0)
            {
                Cla.Port = 8080;
            }
            else if (portArgs.Count() == 1)
            {
                string p = portArgs.ElementAt(0).After("=", StringComparison.OrdinalIgnoreCase);
                if (String.IsNullOrWhiteSpace(p))
                {
                    Cla.Help = true;
                    Cla.HelpMessage = "No port specified, try a number between 1024 and 65536. Default is 8080.";
                }
                else
                {
                    short n;
                    if (Int16.TryParse(p, out n))
                    {
                        Cla.Port = n;
                    }
                    else
                    {
                        Cla.Help = true;
                        Cla.HelpMessage = "Invalid port specified, try a number between 1024 and 65536. Default is 8080.";
                    }
                }
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--port specified more than once.";
            }
        }

        void ParseViewDOP()
        {
            var dopArgs = from arg in NonFileArgs
                          where arg.StartsWith("--viewdop", StringComparison.OrdinalIgnoreCase) ||
                                arg.StartsWith("viewdop", StringComparison.OrdinalIgnoreCase)
                          select arg;

            if (dopArgs.Count() == 0)
            {
                Cla.ViewDOP = Environment.ProcessorCount * 2;
            }
            else if (dopArgs.Count() == 1)
            {
                string dop = dopArgs.ElementAt(0).After("=", StringComparison.OrdinalIgnoreCase);
                if (String.IsNullOrWhiteSpace(dop))
                {
                    Cla.Help = true;
                    Cla.HelpMessage = "No viewdop specified.";
                }
                else
                {
                    int n;
                    if (Int32.TryParse(dop, out n))
                    {
                        if (n <= 0)
                        {
                            Cla.Help = true;
                            Cla.HelpMessage = "Invalid viewdop specified - must be at least 1.";
                        }
                        else
                        {
                            Cla.ViewDOP = n;
                        }
                    }
                    else
                    {
                        Cla.Help = true;
                        Cla.HelpMessage = "Invalid viewdop specified.";
                    }
                }
            }
            else
            {
                Cla.Help = true;
                Cla.HelpMessage = "--viewdop specified more than once.";
            }
        }

        void ParseFiles()
        {
            foreach (var file in FileArgs.Select(f => f.Trim()))
                Cla.AddFile(file);
        }
        #endregion

        IEnumerable<string> NonFileArgs
        {
            get
            {
                return OriginalArgs.TakeWhile(a => a != "--");
            }
        }

        IEnumerable<string> FileArgs
        {
            get
            {
                if (OriginalArgs.Contains("--"))
                    return OriginalArgs.SkipWhile(s => s != "--").Skip(1);
                else
                    return Enumerable.Empty<string>();
            }
        }

        // Inner class so we can return a read-only interface.
        class CommandLineArgs : ICommandLineArgs
        {
            public bool Help { get; set; }
            public bool FullHelp { get; set; }
            public string HelpMessage { get; set; }
            public LoggingLevel LoggingLevel { get; set; }
            public string GenProjectFile { get; set; }
            public string SettingsFile { get; set; }
            public bool Clean { get; set; }
            public bool Serve { get; set; }
            public bool Watch { get; set; }
            public IEnumerable<string> BuildSteps { get { return _BuildSteps; } }
            public IEnumerable<string> Files { get { return _Files; } }
            public short Port { get; set; }
            public int ViewDOP { get; set; }

            List<string> _BuildSteps = new List<string>();
            List<string> _Files = new List<string>();

            public void AddBuildStep(string step)
            {
                _BuildSteps.Add(step);
            }

            public void AddFile(string file)
            {
                _Files.Add(file);
            }

            public void ClearBuildSteps()
            {
                _BuildSteps.Clear();
            }
        }
    }
}
