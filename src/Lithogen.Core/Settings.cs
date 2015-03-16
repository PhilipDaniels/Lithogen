using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Lithogen.Core
{
    /// <summary>
    /// Represents the settings that this invocation of Lithogen is working with.
    /// These settings are written to Lithogen.xml and Lithogen.json from where
    /// they are picked up by downstream programs.
    /// </summary>
    public class Settings : ISettings
    {
        /// <summary>
        /// The importance of messages logged by the build. Defaults to HIGH (chatty).
        /// </summary>
        public string MessageImportance { get; set; }

        /// <summary>
        /// Where Lithogen (the exes, node etc.) is installed.
        /// </summary>
        public string InstallationDirectory { get; set; }

        /// <summary>
        /// The full path of the Lithogen program.
        /// </summary>
        public string LithogenExePath { get; set; }

        /// <summary>
        /// The full path of the Node program.
        /// </summary>
        public string NodeExePath { get; set; }

        /// <summary>
        /// The full path of the NPM program.
        /// </summary>
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
        /// The directory that contains the Partial Views and Layouts.
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
        public string LithogenWebsiteDirectory { get; set; }

        /// <summary>
        /// The path where the Xml config file should be written.
        /// </summary>
        public string XmlConfigFile { get; set; }

        /// <summary>
        /// The path where the JSON config file should be written.
        /// </summary>
        public string JsonConfigFile { get; set; }

        /// <summary>
        /// The path where the Lithogen log file should be written.
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// The source website project's project directory.
        /// </summary>
        public string ProjectDirectory
        {
            get
            {
                return Path.GetDirectoryName(ProjectFile);
            }
            set
            {
                //throw new InvalidOperationException("Don't call set, it only exists to get Xml serialization to work.");
            }
        }

        // TODO: Take control of XmlSerialization ourselves and get rid of this nonsense.
        [XmlIgnore]
        public IEnumerable<string> AssemblyLoadDirectories { get { return _AssemblyLoadDirectoriesSurrogate; } }

        [JsonIgnore]
        [XmlElement, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public List<string> AssemblyLoadDirectoriesSurrogate
        {
            get
            {
                return _AssemblyLoadDirectoriesSurrogate;
            }
            set
            {
                throw new InvalidOperationException("Don't call set, it only exists to get Xml serialization to work.");
            }
        }
        List<string> _AssemblyLoadDirectoriesSurrogate;

        /// <summary>
        /// The URL used by the built-in web server.
        /// </summary>
        public string ServeUrl { get; set; }

        /// <summary>
        /// The URL used to implement "live reload" functionality.
        /// </summary>
        public string ReloadUrl
        {
            get
            {
                if (Configuration.Equals("Debug", StringComparison.OrdinalIgnoreCase) &&
                    !String.IsNullOrWhiteSpace(ServeUrl))
                {
                    // See https://github.com/livereload/livereload-js
                    // and http://feedback.livereload.com/
                    // and http://feedback.livereload.com/knowledgebase/articles/86174-livereload-protocol
                    var uri = new Uri(ServeUrl);
                    var builder = new UriBuilder();
                    builder.Port = 35729;
                    builder.Scheme = uri.Scheme;
                    builder.Host = uri.Host;
                    builder.Path = "livereload.js";
                    return builder.ToString();
                }
                else
                { 
                    return null;
                }
            }
            set
            {
                // Ignore, needed for serialization.
            }
        }


        /// <summary>
        /// Specifies the degree of parallelism to use when compiling views.
        /// </summary>
        public int ViewDOP
        {
            get
            {
                return _ViewDOP;
            }
            set
            {
                _ViewDOP = value.ThrowIfLessThanOrEqualTo(0, "ViewDOP");
            }

        }
        int _ViewDOP;

        public Settings()
        {
            _AssemblyLoadDirectoriesSurrogate = new List<string>();
            ViewDOP = Environment.ProcessorCount * 2;
        }

        /// <summary>
        /// Construct settings from XML that has been serialized into a file (typically Lithogen.xml,
        /// which is written by the task shim).
        /// </summary>
        /// <param name="fileName">The file to load.</param>
        public static Settings LoadFromFile(string fileName)
        {
            fileName.ThrowIfFileDoesNotExist("fileName");

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var bf = new XmlSerializer(typeof(Settings));
                var settings = (Settings)bf.Deserialize(fs);
                return settings;
            }
        }

        /// <summary>
        /// Validates the configuration.
        /// </summary>
        public void Validate()
        {
            // We don't actually need it, but if set it should exist.
            if (SolutionFile != null)
            {
                SolutionFile = SolutionFile.Trim();
                if (SolutionFile.Length == 0)
                {
                    SolutionFile = null;
                }
                if (SolutionFile != null && !File.Exists(SolutionFile))
                    throw new FileNotFoundException("The SolutionFile '" + SolutionFile + "' does not exist.");
            }

            //if (String.IsNullOrWhiteSpace(ProjectFile))
            //    throw new ArgumentException("ProjectFile must be set.");
            //ProjectFile = ProjectFile.Trim();
            //if (!File.Exists(ProjectFile))
            //    throw new FileNotFoundException("The ProjectFile '" + ProjectFile + "' does not exist.");
            //if (String.IsNullOrWhiteSpace(Configuration))
            //    throw new ArgumentException("Configuration must be set.");
            //else
            //    Configuration = Configuration.Trim();

            //if (String.IsNullOrWhiteSpace(OutputDirectoryBase))
            //    throw new ArgumentException("OutputDirectory must be set.");
        }

        /// <summary>
        /// Writes this settings object in XML format to <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The file to write to.</param>
        public void WriteXmlSettingsFile(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            FileUtils.EnsureDirectory(Path.GetDirectoryName(fileName));

            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(fs, this);
            }
        }

        /// <summary>
        /// Writes this settings object in JSON format to <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The file to write to.</param>
        public void WriteJsonSettingsFile(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            FileUtils.EnsureDirectory(Path.GetDirectoryName(fileName));
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }
    }
}
