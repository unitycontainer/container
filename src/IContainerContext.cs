using Unity.Lifetime;
using Unity.Policy;

namespace Unity
{
    /// <summary>
    /// Container interface exposing engine to internal container parts 
    /// </summary>
    public interface IContainerContext
    {
        /// <summary>
        /// The container that this context is associated with.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> object.</value>
        IUnityContainer Container { get; }

        /// <summary>
        /// The <see cref="ILifetimeContainer"/> that this container uses.
        /// </summary>
        /// <value>The <see cref="ILifetimeContainer"/> is used to manage <see cref="IDisposable"/> objects that the container is managing.</value>
        ILifetimeContainer Lifetime { get; }

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        IPolicyList Policies { get; }

    }
}
