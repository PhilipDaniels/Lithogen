using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;
using Lithogen.Engine;
using Lithogen.Engine.CommandLine;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace Lithogen
{
    class Program
    {
        const string LOG_PREFIX = "Lithogen.exe: ";
        public static SimpleInjector.Container Container;
        static ILogger TheLogger;
        public static ISettings TheSettings;
        static object CommandHandlerPadlock;
        static DirectoryWatcher Watcher;
        static object WatcherPadlock;
        static WebServer MainWebServer;
        static WebServer LiveReloadWebServer;

        static int Main(string[] args)
        {
            CommandHandlerPadlock = new object();
            WatcherPadlock = new object();
            ILogger logger = null;

            try
            {
                var parser = new CommandLineParser(args);
                var parsedArgs = parser.Parse(inServerMode: false, currentLoggingLevel: LoggingLevel.Normal);

                if (parsedArgs.Help)
                {
                    Console.Error.WriteLine(CommandLineParser.ShortHelpText);
                    return 1;
                }
                else if (parsedArgs.FullHelp)
                {
                    Console.Error.WriteLine(CommandLineParser.FullHelpText);
                    Console.Error.Flush();
                    return 1;
                }

                if (parsedArgs.GenProjectFile != null)
                {
                    GenerateSettingsFiles(parsedArgs.GenProjectFile);
                    return 0;
                }

                if (!File.Exists(parsedArgs.SettingsFile))
                {
                    Console.Error.WriteLine("Fatal error. XmlConfigFile not found: " + parsedArgs.SettingsFile);
                    return 1;
                }

                var settings = Settings.LoadFromFile(parsedArgs.SettingsFile);
                settings.Validate();
                TheSettings = settings;
                settings = null;

                Environment.SetEnvironmentVariable("LithogenXml", TheSettings.XmlConfigFile.Replace(@"\", @"\\"));
                Environment.SetEnvironmentVariable("LithogenJson", TheSettings.JsonConfigFile.Replace(@"\", @"\\"));

                TheLogger = new RedirectingLogger(TheSettings.LogFile);
                TheLogger.LoggingLevel = parsedArgs.LoggingLevel;
                TheLogger.LogMessage(LOG_PREFIX + "arguments = " + parser.OriginalArgsString);

                Container = DI.ContainerConstructor.CreateAndConfigureContainer(TheSettings, TheLogger);

                CheckForNpmDependencies();

                EnterBuildLoop(parsedArgs);

                return 0;
            }
            catch (Exception ex)
            {
                // We may come here if the command line args are invalid.
                if (logger != null)
                    logger.LogError(ex.ToString());
                else
                    Console.Error.WriteLine(ex.ToString());
                return 1;
            }
        }

        static void EnterBuildLoop(ICommandLineArgs parsedArgs)
        {
            var generator = Container.GetInstance<ICommandStreamGenerator>();
            IEnumerable<ICommand> commands = generator.GetCommands(parsedArgs);
            ProcessCommands(commands);

            if (parsedArgs.Serve)
            {
                if (parsedArgs.Watch)
                    InitiateFileWatching();

                StartWebServers(parsedArgs.Port);

                Console.WriteLine();
                Console.WriteLine("Entering server mode. Your website is on {0}", TheSettings.ServeUrl);
                Console.WriteLine("Type 'stop' or Ctrl-C to stop.");

                while (true)
                {
                    Console.Write("> ");
                    string userCommands = Console.ReadLine();
                    if (userCommands.Equals("stop", StringComparison.OrdinalIgnoreCase))
                        break;

                    var parser = new CommandLineParser(userCommands);
                    parsedArgs = parser.Parse(inServerMode: true, currentLoggingLevel: TheLogger.LoggingLevel);

                    if (parsedArgs.Help)
                    {
                        Console.Error.WriteLine(CommandLineParser.ShortHelpText);
                        Console.Error.Flush();
                    }
                    else if (parsedArgs.FullHelp)
                    {
                        Console.Error.WriteLine(CommandLineParser.FullHelpText);
                        Console.Error.Flush();
                    }
                    else
                    {
                        commands = generator.GetCommands(parsedArgs);
                        ProcessCommands(commands);
                    }
                }
            }

            TerminateFileWatching();
            TerminateWebServers();

            ProcessCommand(new BuildCompleteCommand(TheSettings.LithogenWebsiteDirectory));
        }

        static void StartWebServers(short port)
        {
            MainWebServer = new WebServer(TheSettings.ServeUrl);
            MainWebServer.RegisterModule(new StaticFilesModule(TheSettings.LithogenWebsiteDirectory));
            MainWebServer.Module<StaticFilesModule>().UseRamCache = false;
            MainWebServer.Module<StaticFilesModule>().DefaultExtension = ".html";
            MainWebServer.RunAsync();

            if (!String.IsNullOrWhiteSpace(TheSettings.ReloadUrl))
            {
                var urls = new[] { TheSettings.ReloadUrl, TheSettings.ReloadUrl.Replace("/livereload.js", "") };
                LiveReloadWebServer = new WebServer(urls);

                var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); 
                LiveReloadWebServer.RegisterModule(new StaticFilesModule(dir));
                LiveReloadWebServer.Module<StaticFilesModule>().UseRamCache = true;

                LiveReloadWebServer.RegisterModule(new WebSocketsModule());
                LiveReloadWebServer.Module<WebSocketsModule>().RegisterWebSocketsServer<LiveReloadServer>("/livereload");

                LiveReloadWebServer.RunAsync();
            }
        }

        static void TerminateWebServers()
        {
            try
            {
                if (LiveReloadWebServer != null)
                    LiveReloadWebServer.Dispose();
                if (MainWebServer != null)
                    MainWebServer.Dispose();
            }
            catch { }
        }

        static void InitiateFileWatching()
        {
            var filesToIgnore = new string[] { TheSettings.LogFile };

            var directoriesToIgnore = new string[]
            {
                TheSettings.LithogenWebsiteDirectory,
                Path.Combine(TheSettings.ProjectDirectory, "bin"),
                Path.Combine(TheSettings.ProjectDirectory, "obj")
            };

            Watcher = new DirectoryWatcher
                (
                TheSettings.ProjectDirectory,
                DirectoryWatcher.DefaultTimerPeriodMilliseconds,
                filesToIgnore,
                directoriesToIgnore
                );

            Watcher.ChangedFiles += Watcher_ChangedFiles;
            Watcher.Start();
        }

        static void Watcher_ChangedFiles(object sender, DirectoryWatcherEventArgs e)
        {
            lock (WatcherPadlock)
            {
                // Alas, we get events for directories (which we don't care about). Although the DirectoryWatcher
                // de-duplicates events, we may still get duplicates from our perspective, because whatever happens
                // to a file we are just going to build it or clean it. So we have a further de-dupe step here.
                var filtered = (from n in e.FileSystemEvents
                                where File.Exists(n.FullPath)
                                select new FileNotification(ConvertWatcherType(n.ChangeType), n.FullPath)
                                ).Distinct();

                if (filtered.Any())
                {
                    var generator = Container.GetInstance<ICommandStreamGenerator>();
                    var commands = generator.GetFileCommands(filtered);
                    Console.WriteLine();
                    ProcessCommands(commands);
                    Console.Write("> ");
                }
            }
        }

        static void TerminateFileWatching()
        {
            if (Watcher != null)
            {
                Watcher.Stop();
                Watcher.Dispose();
                Watcher = null;
            }
        }

        static FileNotificationType ConvertWatcherType(WatcherChangeTypes type)
        {
            switch (type)
            {
                case WatcherChangeTypes.Created:
                    return FileNotificationType.Build;
                case WatcherChangeTypes.Deleted:
                    return FileNotificationType.Clean;
                case WatcherChangeTypes.Changed:
                    return FileNotificationType.Build;
                case WatcherChangeTypes.Renamed:
                    return FileNotificationType.Build;
                default:
                    throw new ArgumentException("Unexpected watcher type " + type.ToString());
            }
        }

        static void ProcessCommands(IEnumerable<ICommand> commands)
        {
            if (commands.Any())
            {
                lock (CommandHandlerPadlock)
                {
                    foreach (var command in commands)
                    {
                        // Per instructions at http://stackoverflow.com/questions/23582234/simple-injector-usage-for-generic-command-handler
                        // TODO: How to push this inside a CommandDespatcher class?

                        if (command is ICommandTriple)
                        {
                            dynamic triple = command;

                            ProcessCommand(triple.PreCommand);
                            if (!triple.PreCommand.Handled)
                            {
                                ProcessCommand(triple.Command);
                                ProcessCommand(triple.PostCommand);
                            }
                        }
                        else
                        {
                            ProcessCommand(command);
                        }
                    }
                }
            }
        }

        static void ProcessCommand(ICommand command)
        {
            var type = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            // Not all commands always have handlers, there are no default handlers for Pre and Post commands for example.
            var registration = Container.GetRegistration(type, false);
            if (registration != null)
            {
                dynamic handler = registration.GetInstance();
                handler.Handle((dynamic)command);
            }
        }

        /// <summary>
        /// Construct a "default/example" settings object and use that to write XML and JSON settings file.
        /// </summary>
        /// <param name="projectFile">Path to project file. Must exist.</param>
        static void GenerateSettingsFiles(string projectFile)
        {
            projectFile.ThrowIfNullOrWhiteSpace("projectFile");

            if (!File.Exists(projectFile))
            {
                Console.Error.WriteLine("The project file " + projectFile + " does not exist.");
                return;
            }

            var settings = new Settings();
            settings.ProjectFile = projectFile;
            settings.MessageImportance = "Normal";
            settings.LithogenExePath = typeof(Program).Assembly.Location;
            settings.InstallationDirectory = Path.GetDirectoryName(settings.LithogenExePath);
            settings.NodeExePath = Path.Combine(settings.InstallationDirectory, @"node\node.exe");
            settings.NpmExePath = Path.Combine(settings.InstallationDirectory, @"node\npm.cmd");
            settings.ContentDirectory = Path.Combine(settings.ProjectDirectory, @"content");
            settings.ImagesDirectory = Path.Combine(settings.ProjectDirectory, @"images");
            settings.ScriptsDirectory = Path.Combine(settings.ProjectDirectory, @"scripts");
            settings.ViewsDirectory = Path.Combine(settings.ProjectDirectory, @"views");
            settings.PartialsDirectory = Path.Combine(settings.ProjectDirectory, @"views\shared");
            settings.SolutionFile = "";
            settings.Configuration = @"Debug";
            settings.TargetDirectory = Path.Combine(settings.ProjectDirectory, @"bin");
            settings.LithogenWebsiteDirectory = Path.Combine(settings.TargetDirectory, @"Lithogen.website");
            settings.XmlConfigFile = Path.Combine(settings.TargetDirectory, @"Lithogen.xml");
            settings.JsonConfigFile = Path.Combine(settings.TargetDirectory, @"Lithogen.json");
            settings.LogFile = Path.Combine(settings.TargetDirectory, @"Lithogen.log");
            settings.AssemblyLoadDirectoriesSurrogate.Add(settings.TargetDirectory);
            settings.ViewDOP = Environment.ProcessorCount * 2;
            settings.ServeUrl = @"http://localhost:8080/";
            settings.ReloadUrl = @"http://localhost:35729/";

            settings.WriteXmlSettingsFile(settings.XmlConfigFile);
            settings.WriteJsonSettingsFile(settings.JsonConfigFile);
        }

        static void CheckForNpmDependencies()
        {
            string dir = Path.GetDirectoryName(TheSettings.NodeExePath);
            string packageJsonFilename = Path.Combine(dir, "package.json");

            var npm = Container.GetInstance<INpm>();
            if (npm.InstallIsRequired(packageJsonFilename))
            {
                TheLogger.LogMessage("Downloading npm packages, please wait...");
                npm.PerformInstall(TheSettings.NodeExePath, packageJsonFilename);
            }
        }
    }
}
