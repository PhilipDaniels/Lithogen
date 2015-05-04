using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;

namespace Lithogen.DI
{
    /// <summary>
    /// The AssemblyLoader is used to load all assemblies found in the
    /// list of directories specified in Settings.AssemblyLoadDirectories.
    /// It loads them into the current domain.
    /// </summary>
    public class AssemblyLoader
    {
        const string LOG_PREFIX = "AssemblyLoader: ";
        readonly ILogger TheLogger;
        readonly List<Assembly> _LoadedAssemblies;

        public AssemblyLoader(ILogger logger)
        {
            TheLogger = logger.ThrowIfNull("logger");
            _LoadedAssemblies = new List<Assembly>();
        }

        /// <summary>
        /// Returns the list of loaded assemblies.
        /// </summary>
        public IEnumerable<Assembly> LoadedAssemblies
        {
            get { return _LoadedAssemblies; }
        }

        /// <summary>
        /// Loads assemblies from the specified directories.
        /// Any exceptions propagate upwards.
        /// </summary>
        public void LoadAssemblies(IEnumerable<string> directories)
        {
            LoadAssembliesImpl(directories, true);
        }

        /// <summary>
        /// Loads assemblies from the specified directories.
        /// Any exceptions are logged and squashed.
        /// </summary>
        public void LoadAsssembliesSafe(IEnumerable<string> directories)
        {
            LoadAssembliesImpl(directories, false);
        }

        void LoadAssembliesImpl(IEnumerable<string> directories, bool throwExceptions)
        {
            if (directories == null || directories.Count() == 0)
            {
                TheLogger.LogMessage(LOG_PREFIX + "No directories to load from.");
                return;
            }
                

            try
            {
                foreach (var dir in directories)
                {
                    if (!Directory.Exists(dir))
                    {
                        TheLogger.LogMessage(LOG_PREFIX + "The directory {0} does not exist - ignoring.", dir);
                        continue;
                    }

                    var dlls = Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly);
                    foreach (var dll in dlls)
                    {
                        try
                        {
                            if (AssemblyIsAlreadyLoaded(dll))
                            {
                                TheLogger.LogMessage(LOG_PREFIX + "Skipping {0} because it is already loaded.", dll);
                            }
                            else
                            {
                                var asm = Assembly.LoadFrom(dll);
                                TheLogger.LogMessage(LOG_PREFIX + "Loaded {0} from {1}", asm.FullName, dll);
                                _LoadedAssemblies.Add(asm);
                            }
                        }
                        catch (Exception ex)
                        {
                            TheLogger.LogMessage(LOG_PREFIX + "Error while loading {0}: {1}", dll, ex.ToString());
                            if (throwExceptions)
                                throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TheLogger.LogMessage(LOG_PREFIX + "Exception: " + ex.ToString());
                if (throwExceptions)
                    throw;
            }
        }

        bool AssemblyIsAlreadyLoaded(string path)
        {
            var nameOfAssemblyToLoad = AssemblyName.GetAssemblyName(path);

            foreach (var asmName in AppDomain.CurrentDomain.GetAssemblies().OrderBy(asm => asm.FullName).Select(asm => asm.GetName()))
            {
                if (asmName.ToString() == nameOfAssemblyToLoad.ToString())
                    return true;
            }

            return false;
        }
    }
}
