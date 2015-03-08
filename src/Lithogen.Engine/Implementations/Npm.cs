using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System;
using System.IO;

namespace Lithogen.Engine.Implementations
{
    public class Npm : INpm
    {
        readonly ILogger Logger;
        readonly IProcessRunner ProcessRunner;

        public Npm(ILogger logger, IProcessRunner processRunner)
        {
            Logger = logger.ThrowIfNull("logger");
            ProcessRunner = processRunner.ThrowIfNull("processRunner");
        }

        public bool InstallIsRequired(string packageJsonFilename)
        {
            packageJsonFilename.ThrowIfFileDoesNotExist("packageJsonFilename");
            string lastRunFilename = LastRunFilename(packageJsonFilename);
            if (!File.Exists(lastRunFilename))
                return true;

            var dateJson = File.GetLastWriteTimeUtc(packageJsonFilename);
            var dateLastRun = File.GetLastWriteTimeUtc(lastRunFilename);
            return dateJson > dateLastRun;
        }

        public void PerformInstall(string nodeExePath, string packageJsonFilename)
        {
            packageJsonFilename.ThrowIfFileDoesNotExist("packageJsonFilename");
            nodeExePath.ThrowIfFileDoesNotExist("nodeExePath");

            using (var p = ProcessRunner.MakeProcess(nodeExePath, @"node_modules\npm\bin\npm-cli.js install"))
            {
                ProcessRunner.Execute(p);
            }

            string lastRunFilename = LastRunFilename(packageJsonFilename);
            string msg = String.Format("Lithogen last run 'npm install' at the LastModifiedTime of this file.");
            File.WriteAllText(lastRunFilename, msg);
        }

        public string LastRunFilename(string packageJsonFilename)
        {
            return Path.ChangeExtension(packageJsonFilename, "lastrun");
        }
    }
}
