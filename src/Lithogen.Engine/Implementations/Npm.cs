using System;
using System.Globalization;
using System.IO;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class Npm : INpm
    {
        readonly IProcessRunner ProcessRunner;

        public Npm(IProcessRunner processRunner)
        {
            ProcessRunner = processRunner.ThrowIfNull("processRunner");
        }

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

        public void PerformInstall(string nodeExePath, string packageJsonFileName)
        {
            packageJsonFileName.ThrowIfFileDoesNotExist("packageJsonFileName");
            nodeExePath.ThrowIfFileDoesNotExist("nodeExePath");

            using (var p = ProcessRunner.MakeProcess(nodeExePath, @"node_modules\npm\bin\npm-cli.js install"))
            {
                ProcessRunner.Execute(p);
            }

            string lastRunFilename = LastRunFileName(packageJsonFileName);
            string msg = String.Format(CultureInfo.InvariantCulture, "Lithogen last run 'npm install' at the LastModifiedTime of this file.");
            File.WriteAllText(lastRunFilename, msg);
        }

        public string LastRunFileName(string packageJsonFileName)
        {
            return Path.ChangeExtension(packageJsonFileName, "lastrun");
        }
    }
}
