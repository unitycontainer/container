using System;
using System.Reflection;
using Unity.Policy;

namespace Unity.Resolution
{
    public interface IResolveContext : IPolicyList
    {
        /// <summary>Reference to container.</summary>
        /// <remarks>Reference to the container used to execute this build. </remarks>
        /// <returns> Interface for the hosting container</returns>
        IUnityContainer Container { get; }

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
        object Resolve(Type type, string name);
    }
}
