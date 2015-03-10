using System;
using System.Diagnostics;
using System.IO;
using Lithogen.Core;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Lithogen.TaskShim
{
    /// <summary>
    /// This Shim is loaded by Visual Studio when a build is started and then never unloaded until you
    /// close Visual Studio. Therefore, it has to be a simple DLL that defers all the work onto the
    /// Lithogen build process (which is controlled by npm).
    /// </summary>
    public class LithogenBuildTask : Task
    {
        #region Task parameters
        /// <summary>
        /// The importance of messages logged by the build. Defaults to HIGH (chatty).
        /// </summary>
        [Required]
        public string MessageImportance { get; set; }

        /// <summary>
        /// Where Lithogen (the exes, node etc.) is installed.
        /// </summary>
        [Required]
        public string InstallationDirectory { get; set; }

        /// <summary>
        /// The full path of the Lithogen program.
        /// </summary>
        [Required]
        public string LithogenExePath { get; set; }

        /// <summary>
        /// The full path of the Node program.
        /// </summary>
        [Required]
        public string NodeExePath { get; set; }

        /// <summary>
        /// The full path of the NPM program.
        /// </summary>
        [Required]
        public string NpmExePath { get; set; }

        /// <summary>
        /// The directory that contains CSS, Less, SASS etc.
        /// </summary>
        public string ContentDirectory { get; set; }

        /// <summary>
        /// The directory that contains images.
        /// </summary>
        public string ImagesDirectory { get; set; }

        /// <summary>
        /// The directory that contains JavaScript.
        /// Optional, a default will be assumed if this is not set.
        /// </summary>
        public string ScriptsDirectory { get; set; }

        /// <summary>
        /// The directory that contains the Views.
        /// Optional, a default will be assumed if this is not set.
        /// </summary>
        public string ViewsDirectory { get; set; }

        /// <summary>
        /// The directory that contains the partial views (layouts).
        /// </summary>
        public string PartialsDirectory { get; set; }

        /// <summary>
        /// The full path of the solution file. This will not be set if you run MSBuild
        /// from the command line, it is only set within Visual Studio. So it is best to
        /// avoid creating tasks that rely on it.
        /// </summary>
        public string SolutionFile { get; set; }

        /// <summary>
        /// The full path of the project file (.csproj) that you want Lithogen to build.
        /// </summary>
        public string ProjectFile { get; set; }

        /// <summary>
        /// The build configuration - Debug, Release etc.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// The target directory of the project that is being built.
        /// Typically this is the bin folder.
        /// </summary>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// The directory where the website will be written.
        /// </summary>
        [Required]
        public string LithogenWebsiteDirectory { get; set; }

        /// <summary>
        /// The path where the Xml config file should be written.
        /// </summary>
        [Required]
        public string XmlConfigFile { get; set; }

        /// <summary>
        /// The path where the JSON config file should be written.
        /// </summary>
        [Required]
        public string JsonConfigFile { get; set; }

        /// <summary>
        /// The path where the Lithogen log file should be written.
        /// </summary>
        [Required]
        public string LogFile { get; set; }

        /// <summary>
        /// List of directories to load assemblies from (mainly for model resolution).
        /// </summary>
        public ITaskItem[] AssemblyLoadDirectories { get; set; }

        /// <summary>
        /// The URL used by the built-in web server.
        /// </summary>
        [Required]
        public string ServeUrl { get; set; }

        /// <summary>
        /// The URL used to implement "live reload" functionality.
        /// </summary>
        [Required]
        public string ReloadUrl { get; set; }
        #endregion

        Settings Settings;
        LithogenLogger Logger;

        string ToAbsolute(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return null;
            else
                return Path.GetFullPath(path);
        }

        public override bool Execute()
        {
            DateTime start = DateTime.Now;
            Logger = new LithogenLogger(Log, MessageImportanceValidator.Validate(this.MessageImportance));
            Logger.LogMessageWithoutPrefix("Starting Lithogen task shim.");

            InstallationDirectory = ToAbsolute(InstallationDirectory);
            InstallationDirectory = ValidateDirectoryExists(InstallationDirectory, "LithogenInstallationDirectory", mandatory: true);

            LithogenExePath = ToAbsolute(LithogenExePath);
            LithogenExePath = ValidateFileExists(LithogenExePath, "LithogenExePath", mandatory: true);

            NodeExePath = ToAbsolute(NodeExePath);
            NodeExePath = ValidateFileExists(NodeExePath, "LithogenNodeExePath", mandatory: true);

            NpmExePath = ToAbsolute(NpmExePath);
            NpmExePath = ValidateFileExists(NpmExePath, "LithogenNpmExePath", mandatory: true);

            ContentDirectory = ToAbsolute(ContentDirectory);
            ContentDirectory = ValidateDirectoryExists(ContentDirectory, "LithogenContentDirectory", mandatory: false);

            ImagesDirectory = ToAbsolute(ImagesDirectory);
            ImagesDirectory = ValidateDirectoryExists(ImagesDirectory, "LithogenImagesDirectory", mandatory: false);

            ScriptsDirectory = ToAbsolute(ScriptsDirectory);
            ScriptsDirectory = ValidateDirectoryExists(ScriptsDirectory, "LithogenScriptsDirectory", mandatory: false);

            ViewsDirectory = ToAbsolute(ViewsDirectory);
            ViewsDirectory = ValidateDirectoryExists(ViewsDirectory, "LithogenViewsDirectory", mandatory: false);

            SolutionFile = ToAbsolute(SolutionFile);
            SolutionFile = ValidateFileExists(SolutionFile, "SolutionFile", mandatory: false);

            ProjectFile = ToAbsolute(ProjectFile);
            ProjectFile = ValidateFileExists(ProjectFile, "ProjectFile", mandatory: false);

            Configuration = Configuration == null ? null : Configuration.Trim();

            TargetDirectory = ToAbsolute(TargetDirectory);
            TargetDirectory = ValidateNonBlank(TargetDirectory, "TargetDirectory");

            LithogenWebsiteDirectory = ToAbsolute(LithogenWebsiteDirectory);
            LithogenWebsiteDirectory = ValidateNonBlank(LithogenWebsiteDirectory, "LithogenWebsiteDirectory");

            XmlConfigFile = ToAbsolute(XmlConfigFile);
            XmlConfigFile = ValidateNonBlank(XmlConfigFile, "XmlConfigFile");

            JsonConfigFile = ToAbsolute(JsonConfigFile);
            JsonConfigFile = ValidateNonBlank(JsonConfigFile, "JsonConfigFile");

            LogFile = ToAbsolute(LogFile);
            LogFile = ValidateNonBlank(LogFile, "LogFile");

            if (Logger.HasLoggedErrors)
                return false;

            CreateSettings();
            WriteXmlSettingsFile();
            WriteJsonSettingsFile();

            ExecuteLithogen();

            var elapsed = (DateTime.Now - start).TotalSeconds;
            Logger.LogMessageWithoutPrefix("Lithogen task shim finished in {0:0.00} seconds total time.", elapsed);

            return !Logger.HasLoggedErrors;
        }

        string ValidateNonBlank(string value, string name)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                Logger.LogErrorWithoutPrefix("{0} must be set.", name);
                return null;
            }

            return value.Trim();
        }

        string ValidateFileExists(string filename, string name, bool mandatory)
        {
            if (String.IsNullOrWhiteSpace(filename))
            {
                if (mandatory)
                    Logger.LogErrorWithoutPrefix(name + " must be set. Without it we cannot run the build.");
                return null;
            }

            filename = filename.Trim();
            if (!File.Exists(filename))
            {
                if (mandatory)
                    Logger.LogErrorWithoutPrefix("The file " + filename + " specified for " + name + " does not exist. Without it we cannot run the build.");
                else
                    Logger.LogMessageWithoutPrefix("The file " + filename + " specified for " + name + " does not exist. Ignoring, it is optional.");
                return null;
            }
            else
            {
                Logger.LogMessageWithoutPrefix("{0} found at {1}", name, filename);
                return filename;
            }
        }

        string ValidateDirectoryExists(string directory, string name, bool mandatory)
        {
            if (String.IsNullOrWhiteSpace(directory))
            {
                if (mandatory)
                    Logger.LogErrorWithoutPrefix(name + " must be set. Without it we cannot run the build.");
                return null;
            }
                
            directory = directory.Trim();
            if (!Directory.Exists(directory))
            {
                if (mandatory)
                    Logger.LogErrorWithoutPrefix("The directory " + directory + " specified for " + name + " does not exist. Without it we cannot run the build.");
                else
                    Logger.LogMessageWithoutPrefix("The directory " + directory + " specified for " + name + " does not exist. Ignoring, it is optional.");
                return directory;
            }
            else
            {
                Logger.LogMessageWithoutPrefix("{0} found at {1}", name, directory);
                return directory;
            }
        }

        void CreateSettings()
        {
            Settings = new Settings()
            {
                MessageImportance = this.MessageImportance,
                InstallationDirectory = this.InstallationDirectory,
                LithogenExePath = this.LithogenExePath,
                NodeExePath = this.NodeExePath,
                NpmExePath = this.NpmExePath,

                ContentDirectory = this.ContentDirectory,
                ImagesDirectory = this.ImagesDirectory,
                ScriptsDirectory = this.ScriptsDirectory,
                ViewsDirectory = this.ViewsDirectory,
                PartialsDirectory = this.PartialsDirectory,

                SolutionFile = this.SolutionFile,
                ProjectFile = this.ProjectFile,
                Configuration = this.Configuration,

                TargetDirectory = this.TargetDirectory,
                LithogenWebsiteDirectory = this.LithogenWebsiteDirectory,

                XmlConfigFile = this.XmlConfigFile,
                JsonConfigFile = this.JsonConfigFile,
                LogFile = this.LogFile,

                ServeUrl = this.ServeUrl,
                ReloadUrl = this.ReloadUrl,

                ViewDOP = Environment.ProcessorCount * 2
            };

            foreach (var item in this.AssemblyLoadDirectories)
                Settings.AssemblyLoadDirectoriesSurrogate.Add(item.ItemSpec);

            Settings.Validate();
        }

        void WriteXmlSettingsFile()
        {
            Settings.WriteXmlSettingsFile(XmlConfigFile);
            Logger.LogMessageWithoutPrefix("XmlConfigFile written to " + XmlConfigFile);
        }

        void WriteJsonSettingsFile()
        {
            Settings.WriteJsonSettingsFile(JsonConfigFile);
            Logger.LogMessageWithoutPrefix("JsonConfigFile written to " + JsonConfigFile);
        }

        void ExecuteLithogen()
        {
            using (var p = new Process())
            {
                p.StartInfo.FileName = LithogenExePath;
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(p.StartInfo.FileName);
                if (MessageImportanceValidator.Validate(this.MessageImportance) == Microsoft.Build.Framework.MessageImportance.High)
                    p.StartInfo.Arguments = "--log=verbose ";
                else
                    p.StartInfo.Arguments = "";
                p.StartInfo.Arguments += " --settings=\"" + XmlConfigFile + "\"";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += LogSingleLineMessage;
                p.ErrorDataReceived += LogSingleLineError;
                p.EnableRaisingEvents = true;

                Logger.LogMessageWithoutPrefix("Attempting to start {0}", p.StartInfo.FileName);
                p.Start();

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
                p.CancelOutputRead();
                p.CancelErrorRead();
            }
        }

        void LogSingleLineMessage(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data))
                Logger.LogMessageWithoutPrefix(e.Data);
        }

        void LogSingleLineError(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(e.Data))
                Logger.LogErrorWithoutPrefix(e.Data);
        }
    }
}
