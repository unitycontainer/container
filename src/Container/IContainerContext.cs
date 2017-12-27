using System;
using Unity.Builder;

namespace Unity.Container
{
    /// <summary>
    /// An interface exposing internal UnutyContainer API to 
    /// implementation types. 
    /// </summary>
    /// <remarks>This interface is used to access loacl container's engine and services.</remarks>
    public interface IContainerContext 
    {
        /// <summary>
        /// Reference to current IUnityContainer
        /// </summary>
        IUnityContainer Container { get; }

        INamedType Registration(Type type, string name, bool create = false);
    }
}
