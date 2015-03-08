using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lithogen.Core;
using Lithogen.Core.Commands;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.CommandLine
{
    /// <summary>
    /// Turns a set of command line arguments into a command stream.
    /// </summary>
    public class CommandStreamGenerator : ICommandStreamGenerator
    {
        readonly string LOG_PREFIX = "CommandStreamGenerator: ";
        readonly ILogger TheLogger;
        readonly ISettings TheSettings;
        readonly IFileClassifier FileClassifier;
        readonly IStandardBuildSteps StandardBuildSteps;
        readonly ISideBySide SideBySide;
        readonly IViewFileNameFilter ViewFileNameFilter;

        public CommandStreamGenerator
            (
            ILogger logger,
            ISettings settings,
            IFileClassifier fileClassifier,
            IStandardBuildSteps standardBuildSteps,
            ISideBySide sideBySide,
            IViewFileNameFilter viewFileNameFilter
            )
        {
            TheLogger = logger.ThrowIfNull("logger");
            TheSettings = settings.ThrowIfNull("settings");
            FileClassifier = fileClassifier.ThrowIfNull("fileClassifier");
            StandardBuildSteps = standardBuildSteps.ThrowIfNull("standardBuildSteps");
            SideBySide = sideBySide.ThrowIfNull("sideBySide");
            ViewFileNameFilter = viewFileNameFilter.ThrowIfNull("viewFileNameFilter");
        }

        /// <summary>
        /// Gets the commands implied by a Lithogen command line.
        /// </summary>
        /// <param name="args">Command line arguments object.</param>
        /// <returns>List of commands.</returns>
        public IEnumerable<ICommand> GetCommands(ICommandLineArgs args)
        {
            args.ThrowIfNull("args"); 
            
            var cmds = new List<ICommand>();

            if (TheLogger.LoggingLevel != args.LoggingLevel)
            {
                TheLogger.LoggingLevel = args.LoggingLevel;
                TheLogger.LogMessage(LOG_PREFIX + "Logging level set to " + args.LoggingLevel);
            }
            if (TheSettings.ViewDOP != args.ViewDOP && args.ViewDOP > 0)
            {
                TheSettings.ViewDOP = args.ViewDOP;
                TheLogger.LogMessage(LOG_PREFIX + "ViewDOP set to " + args.ViewDOP);
            }

            if (args.Clean)
                cmds.Add(MakeCleanCommand());

            cmds.AddRange(GetBuildCommands(args.BuildSteps));
            cmds.AddRange(GetFileCommands(args.Files));

            return cmds;
        }

        /// <summary>
        /// Gets the build/clean commands implied by a set of files.
        /// </summary>
        /// <param name="files">The set of files.</param>
        /// <returns>List of commands.</returns>
        public IEnumerable<ICommand> GetFileCommands(IEnumerable<string> files)
        {
            files.ThrowIfNull("files");

            var notifications = from f in files
                                let notificationType = File.Exists(f) ? FileNotificationType.Build : FileNotificationType.Clean
                                select new FileNotification(notificationType, f);

            return GetFileCommands(notifications);
        }

        /// <summary>
        /// Gets the build/clean commands implied by a set of files.
        /// </summary>
        /// <param name="files">The set of files.</param>
        /// <returns>List of commands.</returns>
        public IEnumerable<ICommand> GetFileCommands(IEnumerable<FileNotification> files)
        {
            files.ThrowIfNull("files");

            var cmds = new List<ICommand>();

            var fileList = files.ToList();
            while (fileList.Count > 0)
            {
                var file = fileList[0];
                fileList.RemoveAt(0);
                FileClass fc = FileClassifier.Classify(file.Filename);

                // Anything happening in the content or scripts folders initiates a complete rebuild of them,
                // so we can crush this down to a single command.
                if (fc == FileClass.Content || fc == FileClass.Script)
                {
                    string contentDir = (fc == FileClass.Content) ? TheSettings.ContentDirectory : null;
                    string scriptsDir = (fc == FileClass.Script) ? TheSettings.ScriptsDirectory : null;

                    int contentRemoved = fileList.RemoveAll(n => FileClassifier.Classify(n.Filename) == FileClass.Content);
                    if (contentRemoved > 0)
                        contentDir = TheSettings.ContentDirectory;

                    int scriptsRemoved = fileList.RemoveAll(n => FileClassifier.Classify(n.Filename) == FileClass.Script);
                    if (scriptsRemoved > 0)
                        scriptsDir = TheSettings.ScriptsDirectory;

                    cmds.Add(MakeBuildAssetsCommand(contentDir, scriptsDir));
                }
                else if (fc == FileClass.Image)
                {
                    // We can cope with a single image copy/delete.
                    if (file.NotificationType == FileNotificationType.Build)
                        cmds.Add(MakeBuildImageCommand(file.Filename));
                    else
                        cmds.Add(MakeFileDeleteCommand(file.Filename));
                }
                else if (fc == FileClass.View)
                {
                    if (file.NotificationType == FileNotificationType.Build)
                    {
                        if (SideBySide.IsSideBySideFile(file.Filename))
                        {
                            string mainFile = SideBySide.GetMainFile(file.Filename);
                            if (mainFile != null)
                                cmds.Add(MakeBuildViewCommand(mainFile));
                        }
                        else
                        {
                            if (!ViewFileNameFilter.ShouldIgnore(file.Filename))
                                cmds.Add(MakeBuildViewCommand(file.Filename));
                        }
                    }
                    else
                    {
                        // VS2013 creates temporary files when it saves a file, which causes this message
                        // to be displayed repeatedly...
                        //cmds.Add(new MessageCommand("Deletion of view files is not yet supported."));
                    }
                }
                else if (fc == FileClass.Partial)
                {
                    // A change to a partial file necessitates a reload of the partial cache
                    // and a full rebuild of all views.
                    cmds.Add(new FlushPartialCacheCommand());
                    cmds.Add(MakeBuildViewsCommand());
                }
                else if (fc == FileClass.Unknown)
                {
                    cmds.Add(MakeUnknownFileCommand(file.Filename));
                }
            }

            return cmds;
        }

        IEnumerable<ICommand> GetBuildCommands(IEnumerable<string> buildSteps)
        {
            buildSteps.ThrowIfNull("buildSteps");

            var cmds = new List<ICommand>();

            if (buildSteps.Any())
            {
                cmds.Add(MakeCreateDirectoryCommand());

                // npm, node, csiv
                // The build steps have already been normalized by the parser. But we have to
                // respect the order, and potentially we can issue one BuildAssets command
                // rather than two (if both Content and Scripts are specified).
                var buildStepList = buildSteps.ToList();
                while (buildStepList.Count > 0)
                {
                    string step = buildStepList[0];
                    buildStepList.RemoveAt(0);
                    if (StandardBuildSteps.MatchNode(step))
                    {
                        string nodeArgs = step.Substring("node ".Length).Trim();
                        cmds.Add(MakeNodeCommand(nodeArgs));
                    }
                    else if (StandardBuildSteps.MatchNpm(step))
                    {
                        string npmArgs = step.Substring("npm ".Length).Trim();
                        string packageJsonFile = Path.Combine(TheSettings.ProjectDirectory, "package.json");
                        if (File.Exists(packageJsonFile))
                            cmds.Add(MakeNpmCommand(npmArgs, packageJsonFile));
                    }
                    else if (StandardBuildSteps.MatchImages(step))
                    {
                        if (Directory.Exists(TheSettings.ImagesDirectory))
                            cmds.Add(MakeBuildImagesCommand());
                    }
                    else if (StandardBuildSteps.MatchViews(step))
                    {
                        if (Directory.Exists(TheSettings.ViewsDirectory))
                        {
                            cmds.Add(new FlushPartialCacheCommand());
                            cmds.Add(MakeBuildViewsCommand());
                        }
                    }
                    else
                    {
                        string contentDir = null;
                        if (StandardBuildSteps.MatchContent(step) || buildStepList.Contains(StandardBuildSteps.ContentStepName))
                            contentDir = TheSettings.ContentDirectory;

                        string scriptsDir = null;
                        if (StandardBuildSteps.MatchScripts(step) || buildStepList.Contains(StandardBuildSteps.ScriptsStepName))
                            scriptsDir = TheSettings.ScriptsDirectory;

                        if (contentDir != null || scriptsDir != null)
                        {
                            cmds.Add(MakeBuildAssetsCommand(contentDir, scriptsDir));
                            buildStepList.RemoveAll(s => s == StandardBuildSteps.ContentStepName);
                            buildStepList.RemoveAll(s => s == StandardBuildSteps.ScriptsStepName);
                        }
                    }
                }
            }

            return cmds;
        }

        ICommand MakeCleanCommand()
        {
            var cmd = new CleanCommand(TheSettings.LithogenWebsiteDirectory);

            var triple = new CommandTriple<CleanCommand>();
            triple.PreCommand = new PreCleanCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostCleanCommand(cmd);

            return triple;
        }

        ICommand MakeBuildAssetsCommand(string contentDir, string scriptsDir)
        {
            var cmd = new BuildAssetsCommand
                (
                TheSettings.NodeExePath,
                contentDir,
                scriptsDir,
                TheSettings.LithogenWebsiteDirectory,
                TheSettings.ProjectDirectory
                );

            var triple = new CommandTriple<BuildAssetsCommand>();
            triple.PreCommand = new PreBuildAssetsCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostBuildAssetsCommand(cmd);

            return triple;
        }

        ICommand MakeCreateDirectoryCommand()
        {
            var cmd = new CreateDirectoryCommand(TheSettings.LithogenWebsiteDirectory);

            var triple = new CommandTriple<CreateDirectoryCommand>();
            triple.PreCommand = new PreCreateDirectoryCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostCreateDirectoryCommand(cmd);
                
            return triple;
        }

        ICommand MakeNodeCommand(string nodeArgs)
        {
            var cmd = new NodeCommand(TheSettings.NodeExePath, nodeArgs);

            var triple = new CommandTriple<NodeCommand>();
            triple.PreCommand = new PreNodeCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostNodeCommand(cmd);

            return triple;
        }

        ICommand MakeNpmCommand(string npmArgs, string packageJsonFile)
        {
            var cmd = new NpmCommand(TheSettings.NodeExePath, npmArgs, packageJsonFile);

            var triple = new CommandTriple<NpmCommand>();
            triple.PreCommand = new PreNpmCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostNpmCommand(cmd);

            return triple;
        }

        ICommand MakeBuildImageCommand(string filename)
        {
            var cmd = new BuildImageCommand(filename);

            var triple = new CommandTriple<BuildImageCommand>();
            triple.PreCommand = new PreBuildImageCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostBuildImageCommand(cmd);

            return triple;
        }

        ICommand MakeBuildImagesCommand()
        {
            var cmd = new BuildImagesCommand(TheSettings.LithogenWebsiteDirectory, TheSettings.ImagesDirectory);

            var triple = new CommandTriple<BuildImagesCommand>();
            triple.PreCommand = new PreBuildImagesCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostBuildImagesCommand(cmd);

            return triple;
        }

        ICommand MakeBuildViewCommand(string mainFile)
        {
            var cmd = new BuildViewCommand(mainFile);

            var triple = new CommandTriple<BuildViewCommand>();
            triple.PreCommand = new PreBuildViewCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostBuildViewCommand(cmd);

            return triple;
        }

        ICommand MakeBuildViewsCommand()
        {
            var cmd = new BuildViewsCommand(TheSettings.ViewsDirectory);

            var triple = new CommandTriple<BuildViewsCommand>();
            triple.PreCommand = new PreBuildViewsCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostBuildViewsCommand(cmd);

            return triple;
        }

        ICommand MakeFileDeleteCommand(string filename)
        {
            var cmd = new FileDeleteCommand(filename);

            var triple = new CommandTriple<FileDeleteCommand>();
            triple.PreCommand = new PreFileDeleteCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostFileDeleteCommand(cmd);

            return triple;
        }

        ICommand MakeUnknownFileCommand(string filename)
        {
            var cmd = new UnknownFileCommand(filename);

            var triple = new CommandTriple<UnknownFileCommand>();
            triple.PreCommand = new PreUnknownFileCommand(cmd);
            triple.Command = cmd;
            triple.PostCommand = new PostUnknownFileCommand(cmd);

            return triple;
        }
    }
}
