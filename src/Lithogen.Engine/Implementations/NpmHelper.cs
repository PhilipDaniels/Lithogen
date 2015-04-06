using System;
using System.Globalization;
using System.IO;
using BassUtils;
using EdgeJs;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    public class NpmHelper : INpmHelper
    {
        readonly IEdgeHelper EdgeHelper;

        public NpmHelper(IEdgeHelper edgeHelper)
        {
            EdgeHelper = edgeHelper.ThrowIfNull("edgeHelper");
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

            var edge = Edge.Func(@"
                return function(data, callback) {
                    var hooker = require('./../node/hooker');
                    hooker.hookStreams(data);
                    console.log('Performing npm install...');

                    // Based on http://stackoverflow.com/questions/15957529/can-i-install-a-npm-package-from-javascript-running-in-node-js
                    // and the README at https://github.com/npm/npm#using-npm-programmatically
                    var npm = require('./../node/npm/bin/npm-cli.js');

                    npm.load(function(err) {
                        npm.commands.install(function(er, data) {
                            if (er) console.log('Got an install error');
                            else console.log('Install appears to have worked correctly.');
                        });
                        npm.registry.log.on('log', function (message) {
                            console.log(message);
                        });
                    });

                    callback(null, result);
                }");

            var payload = EdgeHelper.MakeHookedExpando();
            edge(payload);
            
            //using (var p = ProcessRunner.MakeProcess(nodeExePath, @"node_modules\npm\bin\npm-cli.js install"))
            //{
            //    ProcessRunner.Execute(p);
            //}

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
