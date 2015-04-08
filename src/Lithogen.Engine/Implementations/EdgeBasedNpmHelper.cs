using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using BassUtils;
using EdgeJs;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.Engine.Implementations
{
    /*
    // Here for historical reference. I cannot get programmatic calling of npm to work.
    public class EdgeBasedNpmHelper : INpmHelper
    {
        readonly ISettings TheSettings;
        readonly IEdgeHelper EdgeHelper;

        public EdgeBasedNpmHelper(ISettings settings, IEdgeHelper edgeHelper)
        {
            TheSettings = settings.ThrowIfNull("settings");
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

            try
            {
                var js = @"
                    return function(data, callback) {
                        var hooker = require('$PROJECTNODEDIR$/hooker');
                        //hooker.hookStreams(data);
                        console.log('I am hooked');

                        var npmi = require('$PROJECTNODEDIR$/node_modules/npmi');
                        if (typeof(npmi) === 'undefined')
                        {
                            console.log('did not get npmi');
                        }
                        else
                        {
                            console.log('were good, npm version is ' + npmi.NPM_VERSION);
                            var options = {
                                name: 'mkdirp',
                                //path: '$PROJECTNODEDIR$',
                                npmLoad: { loglevel: 'verbose' }
                            };

                            npmi(options, function(err, result) {
                                console.log('in the cb');
                                if (err) {
                                    if (err.code === npmi.LOAD_ERR) console.log('npm load error');
                                    else if (err.code === npmi.INSTALL_ERR) console.log('npm install error');
                                    return console.log(err.message);
                                }
                                else {
                                    console.log(JSON.stringify(result));
                                }
                            });

                            console.log('were done!');

                            var result = {};
                            callback(null, result);
                        }

                        // Based on http://stackoverflow.com/questions/15957529/can-i-install-a-npm-package-from-javascript-running-in-node-js
                        // and the README at https://github.com/npm/npm#using-npm-programmatically
                        //var npm = require('$PROJECTNODEDIR$/node_modules/npm/bin/npm-cli');
                        //var npm = require('$PROJECTNODEDIR$/node_modules/npm/lib/npm');

                        //npm.load({}, function(err) {
                        //    if (err) throw new Error(err);
                        //    console.log('installing.....');

                        //    npm.commands.install([], function(er, data) {
                        //        console.log('install callback running...');
                        //        if (err) throw new Error(err);
                        //        console.log('Install appears to have worked correctly.');
                        //    });
                        //});
                    }";

                js = EdgeHelper.ReplaceLithogenNodeRoot(js);
                var edge = Edge.Func(js);

                var payload = EdgeHelper.MakeHookedExpando();
                dynamic result = edge(payload).Result;

                string lastRunFilename = LastRunFileName(packageJsonFileName);
                string msg = String.Format(CultureInfo.InvariantCulture, "Lithogen last ran 'npm install' at the LastModifiedTime of this file.");
                File.WriteAllText(lastRunFilename, msg);
            }
            catch (AggregateException aex)
            {
                // Try and produce error messages that will be useful to the person whose template just crashed...
                string msg = "";
                foreach (var e in aex.InnerExceptions)
                    msg += e.Message + Environment.NewLine;
                throw new ProcessorException(msg, aex);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("one or more", StringComparison.OrdinalIgnoreCase))
                {
                    string msg = ex.Message;
                    if (ex.InnerException != null && !String.IsNullOrEmpty(ex.InnerException.Message))
                        msg = ex.InnerException.Message;
                    throw new ProcessorException(msg, ex);
                }
                else
                {
                    throw;
                }
            }
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
    */
}
