using System;
using System.Globalization;
using System.IO;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class NpmHelper : INpmHelper
    {
        readonly ISettings TheSettings;
        readonly IProcessRunner ProcessRunner;

        public NpmHelper(ISettings settings, IProcessRunner processRunner)
        {
            TheSettings = settings.ThrowIfNull("settings");
            ProcessRunner = processRunner.ThrowIfNull("processRunner");
        }

        /// <summary>
        /// Checks to see if an "npm install" command is required by comparing the
        /// timestamp of the <paramref name="packageJsonFileName"/> (package.json)
        /// to a separately maintained "last run" file.
        /// </summary>
        /// <param name="packageJsonFileName">Full path of the package.json file.</param>
        /// <returns>True if an "npm install" command is required, false otherwise.</returns>
        public bool InstallIsRequired(string packageJsonFileName)
        {
            packageJsonFileName.ThrowIfFileDoesNotExist("packageJsonFileName");
            string lastRunFilename = LastRunFileName(packageJsonFileName);
            if (!File.Exists(lastRunFilename))
                return true;

            var dateJson = File.GetLastWriteTimeUtc(packageJsonFileName);
            var dateLastRun = File.GetLastWriteTimeUtc(lastRunFilename);
            return dateJson > dateLastRun;
        }

        /// <summary>
        /// Performs an "npm install" on the specified package.json file.
        /// </summary>
        /// <param name="packageJsonFileName">Full path of the package.json file.</param>
        public void PerformInstall(string packageJsonFileName)
        {
            packageJsonFileName.ThrowIfFileDoesNotExist("packageJsonFileName");

            using (var p = ProcessRunner.MakeProcess(TheSettings.NodeExePath, @"node_modules\npm\bin\npm-cli.js install"))
            {
                ProcessRunner.Execute(p);
            }

            string lastRunFilename = LastRunFileName(packageJsonFileName);
            string msg = String.Format(CultureInfo.InvariantCulture, "Lithogen last ran 'npm install' at the LastModifiedTime of this file.");
            File.WriteAllText(lastRunFilename, msg);
        }

        /// <summary>
        /// Returns the name of the "last run" filename for a particular package.json file.
        /// </summary>
        /// <param name="packageJsonFileName">Full path of the package.json file.</param>
        /// <returns>Name of the corresponding last run file.</returns>
        public string LastRunFileName(string packageJsonFileName)
        {
            packageJsonFileName.ThrowIfNullOrWhiteSpace("packageJsonFileName");

            return Path.ChangeExtension(packageJsonFileName, "lastrun");
        }
    }
}
