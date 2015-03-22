using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;
using YamlDotNet.Serialization.NamingConventions;

namespace Lithogen.Engine.Configuration
{
    /// <summary>
    /// Gets the configuration in effect for a view file.
    /// </summary>
    public class ConfigurationResolver : IConfigurationResolver
    {
        const string LOG_PREFIX = "ConfigurationResolver: ";
        const string CONFIG_FILENAME = "_config.lit";
        readonly ILogger TheLogger;
        readonly ISettings TheSettings;
        Dictionary<string, DirectoryConfiguration> EffectiveConfigurationsByDirectory;

        public ConfigurationResolver(ILogger logger, ISettings settings)
        {
            TheLogger = logger.ThrowIfNull("logger");
            TheSettings = settings.ThrowIfNull("settings");
            EffectiveConfigurationsByDirectory = new Dictionary<string, DirectoryConfiguration>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the configuration in effect for a view file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public IDirectoryConfiguration GetConfiguration(string fileName)
        {
            fileName.ThrowIfNullOrWhiteSpace("fileName");

            if (!fileName.StartsWith(TheSettings.ViewsDirectory, StringComparison.OrdinalIgnoreCase))
            {
                string msg = String.Format(CultureInfo.InvariantCulture, "The filename {0} must be within the views directory {1}", fileName, TheSettings.ViewsDirectory);
                throw new ArgumentOutOfRangeException(msg);
            }

            string dir = Path.GetDirectoryName(fileName);
            DirectoryConfiguration dc = GetConfigurationForDirectory(dir);
            return dc;
        }

        /// <summary>
        /// Checks to see whether there is a file processor for a particular extension.
        /// Whether there is depends on the configuration in the directory.
        /// </summary>
        /// <param name="fileName">The file to check.</param>
        /// <returns>True if the extension maps to a file processor, false otherwise.</returns>
        public bool IsMappedExtension(string fileName)
        {
            string dir = Path.GetDirectoryName(fileName);
            string ext = FileUtils.GetCleanExtension(fileName);
            return IsMappedExtension(dir, ext);
        }

        /// <summary>
        /// Checks to see whether there is a file processor for a particular extension.
        /// Whether there is depends on the configuration in the directory.
        /// </summary>
        /// <param name="directory">The directory in which to check.</param>
        /// <param name="extension">The file extension.</param>
        /// <returns>True if the extension maps to a file processor, false otherwise.</returns>
        public bool IsMappedExtension(string directory, string extension)
        {
            IDirectoryConfiguration config = GetConfigurationForDirectory(directory);
            extension = FileUtils.CleanExtension(extension);
            return config.ExtensionMappings.ContainsKey(extension);
        }

        DirectoryConfiguration GetConfigurationForDirectory(string dir)
        {
            DirectoryConfiguration config;
            lock (EffectiveConfigurationsByDirectory)
            {
                if (EffectiveConfigurationsByDirectory.TryGetValue(dir, out config))
                    return config;
            }

            if (dir.Equals(TheSettings.ProjectDirectory, StringComparison.OrdinalIgnoreCase))
            {
                // Termination condition. We never go further than this.
                string cfgFilename = GetConfigFilename(dir);
                if (cfgFilename == null)
                    config = LoadDefaultConfig();
                else
                    config = LoadConfigFromFile(cfgFilename);

                lock (EffectiveConfigurationsByDirectory)
                {
                    EffectiveConfigurationsByDirectory[dir] = config;
                }
                return config;
            }
            else
            {
                string parentDir = Path.GetDirectoryName(dir);
                DirectoryConfiguration parentConfig = GetConfigurationForDirectory(parentDir);

                string cfgFilename = GetConfigFilename(dir);
                if (cfgFilename == null)
                {
                    // Nothing here. Just take the parent.
                    config = parentConfig;
                }
                else
                {
                    // Something here. Apply these settings over the parent settings and store.
                    config = LoadConfigFromFile(cfgFilename);
                    ApplyDefaultsFromParent(config, parentConfig);
                }

                lock (EffectiveConfigurationsByDirectory)
                {
                    EffectiveConfigurationsByDirectory[dir] = config;
                }
                return config;
            }
        }

        static string GetConfigFilename(string dir)
        {
            string filename = Path.Combine(dir, CONFIG_FILENAME);
            if (File.Exists(filename))
                return filename;
            else
                return null;
        }

        static void ApplyDefaultsFromParent(DirectoryConfiguration config, DirectoryConfiguration parentConfig)
        {
            // Update config to reflect its overrides of the parent.
            // Only allow processors to be merged at the Views level.
            foreach (var m in parentConfig.ExtensionMappings)
            {
                IExtensionConfiguration extConfig;
                if (!config.ExtensionMappings.TryGetValue(m.Key, out extConfig))
                    config.ExtensionMappings[m.Key] = m.Value;
                else
                    // m.Value is the parent.
                    extConfig.ApplyDefaultsFromParent(m.Value);
            }
        }

        DirectoryConfiguration LoadDefaultConfig()
        {
            string filename = "Configuration." + CONFIG_FILENAME;
            string contents = Assembly.GetExecutingAssembly().GetResourceAsString(filename);
            return LoadConfigFromString("builtin", contents);
        }

        DirectoryConfiguration LoadConfigFromFile(string filename)
        {
            filename.ThrowIfFileDoesNotExist("filename");

            string contents = File.ReadAllText(filename);
            TheLogger.LogMessage(LOG_PREFIX + "Loaded configuration from " + filename);
            return LoadConfigFromString(filename, contents);
        }

        DirectoryConfiguration LoadConfigFromString(string filename, string s)
        {
            try
            {
                using (var tr = new StringReader(s))
                {
                    var deser = new YamlDotNet.Serialization.Deserializer(null, new CamelCaseNamingConvention());
                    // Load from the string.
                    var yamlMappings = deser.Deserialize<YamlMappings>(tr);
                    DirectoryConfiguration config = Convert(yamlMappings);
                    return config;
                }
            }
            catch (Exception ex)
            {
                TheLogger.LogError(LOG_PREFIX + "Could not load config from {0}, message is {1}", filename, ex.Message);
                // We cannot (easily) recover.
                throw;
            }
        }

        static DirectoryConfiguration Convert(YamlMappings mappings)
        {
            var dc = new DirectoryConfiguration();

            var dcmap = from mapping in mappings.Mappings
                        from extension in mapping.Extensions.Distinct()
                        orderby extension
                        select new
                        {
                            Extension = extension.ToLowerInvariant(),
                            Config = new ExtensionConfiguration()
                            {
                                Processors = mapping.Processors == null ? null : mapping.Processors.Distinct(),
                                DefaultPublish = mapping.Publish,
                                DefaultLayout = mapping.Layout,
                                DefaultModelName = mapping.Model,
                                DefaultExtOut = mapping.ExtOut
                            }
                        };

            foreach (var x in dcmap)
            {
                dc.ExtensionMappings.Add(x.Extension, x.Config);
            }

            return dc;
        }
    }
}
