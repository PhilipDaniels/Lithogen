using System;

// Cannot be a service because is a decorator.
// The registration by convention blows up.

namespace Lithogen.Engine
{
    /// <summary>
    /// The job of the model factory is to create instances of the classes
    /// used in @model directives in Razor .cshtml files.
    /// </summary>
    public interface IModelFactory
    {
        /// <summary>
        /// Gets the <c>Type</c> object for the specified <paramref name="modelTypeName"/>.
        /// The name search is done based on the FullName of the type.
        /// </summary>
        /// <param name="modelTypeName">The name of the type of the model. Must exist in a loaded assembly.</param>
        /// <returns>Type object.</returns>
        Type GetModelType(string modelTypeName);

        /// <summary>
        /// Creates a new instance of the specified <paramref name="modelType"/>.
        /// </summary>
        /// <param name="modelType">The type of the model. Must exist in a loaded assembly.</param>
        /// <returns>New object.</returns>
        object CreateModelInstance(Type modelType);

        /// <summary>
        /// Creates a new instance of the specified <paramref name="modelTypeName"/>.
        /// </summary>
        /// <param name="modelTypeName">The name of the type of the model. Must exist in a loaded assembly.</param>
        /// <returns>New object.</returns>
        object CreateModelInstance(string modelTypeName);
    }
}
