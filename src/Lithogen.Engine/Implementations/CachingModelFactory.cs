using Lithogen.Core;
using Lithogen.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithogen.Engine.Implementations
{
    /// <summary>
    /// The CachingModelFactory can be used as a decorator around any IModelFactory.
    /// It caches instances of models that are marked with the <code>ImmutableObjectAttribute</code>.
    /// </summary>
    public class CachingModelFactory : IModelFactory
    {
        const string LOG_PREFIX = "CachingModelFactory: ";
        readonly ILogger TheLogger;
        readonly IModelFactory InnerModelFactory;
        // TODO: Should probably make these into "Concurrents".
        readonly Dictionary<Type, object> CachedModelInstances;
        readonly HashSet<Type> NonCachedModelInstances;

        public CachingModelFactory(ILogger logger, IModelFactory innerModelFactory)
        {
            TheLogger = logger.ThrowIfNull("logger");
            InnerModelFactory = innerModelFactory.ThrowIfNull("innerModelFactory");
            CachedModelInstances = new Dictionary<Type, object>();
            NonCachedModelInstances = new HashSet<Type>();
        }

        /// <summary>
        /// Gets the <c>Type</c> object for the specified <paramref name="modelTypeName"/>.
        /// The name search is done based on the FullName of the type.
        /// </summary>
        /// <param name="modelTypeName">The name of the type of the model. Must exist in a loaded assembly.</param>
        /// <returns>Type object.</returns>
        public Type GetModelType(string modelTypeName)
        {
            return InnerModelFactory.GetModelType(modelTypeName);
        }

        /// <summary>
        /// Creates a new instance of the specified <paramref name="modelType"/>.
        /// If the type is marked with <code>ImmutableObjectAttribute</code> then the instance
        /// will be cached for future calls.
        /// </summary>
        /// <param name="modelType">The type of the model. Must exist in a loaded assembly.</param>
        /// <returns>New (or previously cached) object.</returns>
        public object CreateModelInstance(Type modelType)
        {
            modelType.ThrowIfNull("modelType");

            object instance;
            if (CachedModelInstances.TryGetValue(modelType, out instance))
                return instance;

            instance = InnerModelFactory.CreateModelInstance(modelType);

            // Have we seen this type before, and decided it was non-cached?
            // If so we don't need to ask it for its attributes.
            if (NonCachedModelInstances.Contains(modelType))
                return instance;

            // Does the user want to cache instances of this?
            var attributes = (ImmutableObjectAttribute[])modelType.GetCustomAttributes(typeof(ImmutableObjectAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                lock (CachedModelInstances)
                {
                    if (!CachedModelInstances.ContainsKey(modelType))
                    {
                        TheLogger.LogMessage(LOG_PREFIX + "Caching instance of {0} for future calls.", modelType.FullName);
                        CachedModelInstances[modelType] = instance;
                    }
                }
            }
            else
            {
                // Not sure that this is actually necessary, but it can't hurt robustness.
                // Add is not guaranteed thread safe.
                lock (NonCachedModelInstances)
                {
                    NonCachedModelInstances.Add(modelType);
                }
            }

            return instance;
        }

        /// <summary>
        /// Creates a new instance of the specified <paramref name="modelTypeName"/>.
        /// If the type is marked with <code>ImmutableObjectAttribute</code> then the instance
        /// will be cached for future calls.
        /// </summary>
        /// <param name="modelTypeName">The name of the type of the model. Must exist in a loaded assembly.</param>
        /// <returns>New (or previously cached) object.</returns>
        public object CreateModelInstance(string modelTypeName)
        {
            Type modelType = InnerModelFactory.GetModelType(modelTypeName);
            return CreateModelInstance(modelType);
        }
    }
}
