using System;
using System.Collections.Generic;
using System.Linq;
using BassUtils;
using Lithogen.Core;

namespace Lithogen.Engine
{
    /// <summary>
    /// The job of the model factory is to create instances of classes.
    /// The assembly loader should be used to load all assemblies
    /// into the currrent app domain, first.
    /// The model types are typically used in @model directives in Razor .cshtml files.
    /// </summary>
    public class ModelFactory : IModelFactory
    {
        public ModelFactory()
        {
            LocatedModelTypes = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Gets the <c>Type</c> object for the specified <paramref name="modelTypeName"/>.
        /// The name search is done based on the FullName of the type.
        /// </summary>
        /// <param name="modelTypeName">The name of the type of the model. Must exist in a loaded assembly.</param>
        /// <returns>Type object.</returns>
        public Type GetModelType(string modelTypeName)
        {
            modelTypeName.ThrowIfNullOrWhiteSpace("modelTypeName");

            Type modelType;
            if (LocatedModelTypes.TryGetValue(modelTypeName, out modelType))
                return modelType;

            var types = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        // See http://vagif.bloggingabout.net/2010/07/02/net-4-0-and-notsupportedexception-complaining-about-dynamic-assemblies/
                        // for this bizarre clause. Without it, we blow up on the call to FullName in SingleOrDefault, below.
                        where !(assembly is System.Reflection.Emit.AssemblyBuilder) && assembly.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder"
                        orderby assembly.FullName
                        from t in assembly.GetExportedTypes()
                        where !t.IsAbstract &&
                              !t.IsInterface &&
                              !t.IsGenericType &&
                              !t.IsGenericTypeDefinition
                        orderby t.FullName
                        select t;

            modelType = types.SingleOrDefault(t => t.FullName == modelTypeName);
            if (modelType != null)
                LocatedModelTypes[modelTypeName] = modelType;

            return modelType;
        }
        Dictionary<string, Type> LocatedModelTypes;

        /// <summary>
        /// Creates a new instance of the specified <paramref name="modelType"/>.
        /// </summary>
        /// <param name="modelType">The type of the model. Must exist in a loaded assembly.</param>
        /// <returns>New object.</returns>
        public object CreateModelInstance(Type modelType)
        {
            modelType.ThrowIfNull("modelType");

            return Activator.CreateInstance(modelType);
        }

        /// <summary>
        /// Creates a new instance of the specified <paramref name="modelTypeName"/>.
        /// </summary>
        /// <param name="modelTypeName">The name of the type of the model. Must exist in a loaded assembly.</param>
        /// <returns>New object.</returns>
        public object CreateModelInstance(string modelTypeName)
        {
            modelTypeName.ThrowIfNullOrWhiteSpace("modelTypeName");

            Type t = GetModelType(modelTypeName);
            return CreateModelInstance(t);
        }
    }
}
