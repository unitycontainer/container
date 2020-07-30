using System;
using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Interface defining the behavior of the Unity dependency injection container.
    /// </summary>
    [CLSCompliant(true)]
    public partial interface IUnityContainerAsync : IUnityContainer
    {
        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <remarks>
        /// If the instance of the container is a child container, this property will hold a reference to
        /// the container that created this instance.
        /// </remarks>
        /// <value>The parent container, or null if this container doesn't have one.</value>
        new IUnityContainerAsync? Parent { get; }


        ValueTask RegisterAsync(params RegistrationDescriptor[] descriptors);


        ValueTask RegisterAsync(ReadOnlyMemory<RegistrationDescriptor> memory);


        /// <summary>
        /// Resolve an instance of the requested type from the container.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of object to get typeFrom the container.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        ValueTask<object?> ResolveAsync(Type type, string? name, params ResolverOverride[] overrides);


        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <param name="name">Name of the child container</param>
        /// <remarks>
        /// Unity allows creating scopes with the help of child container. A child container shares the 
        /// parent's configuration but can be configured with different settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        new IUnityContainerAsync CreateChildContainer(string? name = null);
    }
}
