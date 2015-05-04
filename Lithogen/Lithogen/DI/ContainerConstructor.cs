using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using BassUtils;
using Lithogen.Core;
using Lithogen.Core.Interfaces;
using Lithogen.Engine;
using Lithogen.Engine.Configuration;
using Lithogen.Engine.Implementations;
using SimpleInjector.Extensions;

namespace Lithogen.DI
{
    static class ContainerConstructor
    {
        public static SimpleInjector.Container CreateAndConfigureContainer(ISettings settings, ILogger logger)
        {
            const string LOG_PREFIX = "ConfigureIoC: "; 
            
            SimpleInjector.Container container = new SimpleInjector.Container();
            //container.Options.RegisterParameterConvention(new SettingsConvention());

            container.Options.AllowOverridingRegistrations = true;
            logger.LogMessage(LOG_PREFIX + "IoC Container created.");

            RegisterSingletons(container, logger, settings);
            // Any type in Engine will do.
            RegisterImplementations(container, logger, typeof(CachingModelFactory).Assembly, true);

            // Load all plugins, may include overrides of base types and new file processors.
            string pluginDir = Path.Combine(settings.ProjectDirectory, @"Lithogen\Plugins");
            LoadPlugins(container, logger, pluginDir);

            // Load all assemblies into the current domain. This is needed to get RazorEngine to
            // work (and probably other view engines too, when we get to them).
            var assemblyLoader = new AssemblyLoader(logger);
            assemblyLoader.LoadAssemblies(settings.AssemblyLoadDirectories);

            // Look for any overriding implementations from the website itself.
            logger.LogMessage(LOG_PREFIX + "Looking for implementations of Lithogen services in assemblies from your website...");
            foreach (var asm in assemblyLoader.LoadedAssemblies.OrderBy(asm => asm.FullName))
                RegisterImplementations(container, logger, asm, true);

            container.Verify();
            logger.LogMessage(LOG_PREFIX + "IoC Container verified.");

            return container;
        }

        static void RegisterSingletons(SimpleInjector.Container container, ILogger logger, ISettings settings)
        {
            // Because Lithogen uses multi-threaded view processing any type you register as
            // a singleton must be either immutable or thread-safe.
            container.RegisterSingle<ILogger>(logger);                                       // OK. Uses lock().
            container.RegisterSingle<IModelFactory, ModelFactory>();                         // OK. Actually OK, though it may add a type to the dictionary more than once (highly unlikely).
            container.RegisterSingle<IPartialCache, PartialCache>();                         // OK. Thread safe using lock(). Loaded at the start of a build if it is empty. See also FlushCacheCommand().
            container.RegisterSingle<IPartialResolver, SimplePartialResolver>();             // OK. Thread safe. Has no mutable state.
            container.RegisterSingle<IConfigurationResolver, ConfigurationResolver>();       // OK. Thread safe using lock().
            container.RegisterSingle<IProcessorFactory, SimpleInjectorProcessorFactory>();   // OK. Configured once at startup then immutable. Implements IFreeze.
            container.RegisterSingle<ISettings>(settings);                                   // OK. Interface is read only.

            container.RegisterDecorator(typeof(IModelFactory), typeof(CachingModelFactory), SimpleInjector.Lifestyle.Singleton);    // OK. Uses lock().
            
            // Anybody who wants us to inject a MemoryCache will get a global singleton instance.
            // If classes want their own instance they can create it themselves.
            // Client responsibility to use in a thread safe manner.
            container.RegisterSingle<MemoryCache>(MemoryCache.Default);                      
        }

        public static void RegisterImplementations(SimpleInjector.Container container, ILogger logger, Assembly assembly, bool logMessages)
        {
            const string LOG_PREFIX = "RegisterImplementations: ";

            try
            {
                // Register all command handlers. This only works because of the way we resolve
                // command handlers, which is done in Lithogen itself (see Program.cs).
                container.RegisterManyForOpenGeneric(typeof(ICommandHandler<>), assembly);

                var implementers = GetLithogenImplementers(assembly).ToList();
                foreach (var impl in implementers)
                {
                    bool isRegisteredAsSingleton = container.IsRegisteredAsSingleton(impl.InterfaceType);

                    int implCount = impl.Implementers.Count();

                    if (implCount == 0)
                    {
                        // This can happen for interfaces of private classes, for example ICommandLineArgs.
                    }
                    else if (isRegisteredAsSingleton)
                    {
                        if (implCount != 1)
                        {
                            string msg = String.Format("There must be exactly one implementer of {0} because it is registered as a singleton.", impl.InterfaceType);
                            throw new InvalidOperationException(msg);
                        }

                        var concreteType = impl.Implementers.ElementAt(0);
                        container.RegisterSingle(impl.InterfaceType, concreteType);
                        if (logMessages)
                            logger.LogMessage(LOG_PREFIX + "{0} -> {1} (Singleton).", impl.InterfaceType.FullName, concreteType.FullName);
                    }
                    else if (implCount == 1)
                    {
                        var concreteType = impl.Implementers.ElementAt(0);
                        container.Register(impl.InterfaceType, concreteType);
                        if (logMessages)
                            logger.LogMessage(LOG_PREFIX + "{0} -> {1}.", impl.InterfaceType.FullName, concreteType.FullName);
                    }
                    else
                    {
                        var sortedImplementers = impl.Implementers.OrderBy(i => i.FullName);
                        container.RegisterAll(impl.InterfaceType, sortedImplementers);
                        if (logMessages)
                        {
                            string msg = String.Join(", ", from i in sortedImplementers select i.FullName);
                            logger.LogMessage(LOG_PREFIX + "All {0} {1} -> {2}", sortedImplementers.Count(), impl.InterfaceType.FullName, msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Dynamic loading is fraught with difficulties. Log the exception
                // and continue.
                logger.LogMessage(LOG_PREFIX + "Exception: " + ex.Message);
            }
        }

        static void LoadPlugins(SimpleInjector.Container container, ILogger logger, string pluginsDirectory)
        {
            const string LOG_PREFIX = "LoadPlugins: ";

            if (!Directory.Exists(pluginsDirectory))
            {
                logger.LogMessage(LOG_PREFIX + "The Plugins directory {0} does not exist.", pluginsDirectory);
                return;
            }

            var plugins = Directory.GetFiles(pluginsDirectory, "*.dll", SearchOption.TopDirectoryOnly);
            if (plugins.Count() == 0)
            {
                logger.LogMessage(LOG_PREFIX + "No Plugins found in {0} - the search looks for files named *.dll in the top directory only.", pluginsDirectory);
                return;
            }

            foreach (string plugin in plugins.OrderBy(p => p))
            {
                logger.LogMessage(LOG_PREFIX + "Registering plugins from " + plugin);

                var asm = Assembly.LoadFile(plugin);
                RegisterImplementations(container, logger, asm, true);
            }
        }

        #region Type queries/selections
        static Lazy<List<Type>> AllTypesInCore = new Lazy<List<Type>>(() => typeof(Settings).Assembly.GetTypes().ToList());

        static IEnumerable<LithogenImplementers> GetLithogenImplementers(Assembly assembly)
        {
            var types = from type in assembly.GetExportedTypes()
                        where !type.IsInterface &&
                              !type.IsAbstract &&
                              !type.IsGenericTypeDefinition
                        let lithogenInterfaces = type.GetInterfaces().Intersect(ImplementableInterfaces)
                        where lithogenInterfaces.Any()
                        orderby type.FullName
                        select new { ConcreteType = type, LithogenInterfaces = lithogenInterfaces };

            var implementers = from interfaceType in ImplementableInterfaces
                               let concreteTypes = (from t in types where t.LithogenInterfaces.Any(x => x == interfaceType) select t.ConcreteType)
                               where concreteTypes.Any()
                               select new LithogenImplementers() { InterfaceType = interfaceType, Implementers = concreteTypes };

            var xl = implementers.ToList();
            return xl;
        }

        static IEnumerable<Type> ImplementableInterfaces
        {
            get
            {
                return from t in AllTypesInCore.Value
                       where t.IsInterface &&
                             t.Namespace.Equals("Lithogen.Core.Interfaces", StringComparison.OrdinalIgnoreCase)
                       orderby t.FullName
                       select t;
            }
        }
        #endregion
    }
}
