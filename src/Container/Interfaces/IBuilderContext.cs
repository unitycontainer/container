using System;

namespace Unity.Policy
{
    public interface IBuilderContext 
    {
        /// <summary>
        /// Type being resolved.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Name of the registered type
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Resolve type/object/dependency using current context
        /// </summary>
        /// <param name="type">Type of requested object</param>
        /// <param name="name">Name of registration</param>
        /// <exception cref="ResolutionFailedException">Throws if requested object could not be created</exception>
        /// <returns>Returns resolved object or throws an <see cref="ResolutionFailedException"/> exception</returns>
        object? Resolve(Type type, string? name);
    }
}
