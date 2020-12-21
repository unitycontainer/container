using System;

namespace Unity.Extension
{
    public interface IResolveContext : IPolicyList
    {
        /// <summary>Reference to container.</summary>
        UnityContainer Container { get; }

        /// <summary>
        /// Type being resolved.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Name of the registered type
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Resolve type/object/dependency using current context
        /// </summary>
        /// <param name="type">Type of requested object</param>
        /// <param name="name">Name of registration</param>
        /// <returns></returns>
        object? Resolve(Type type, string name);
    }
}
