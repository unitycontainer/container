using Unity.Lifetime;
using Unity.Policy;

namespace Unity.Extension
{
    /// <summary>
    /// The <see cref="ExtensionContext"/> class provides the means for extension objects
    /// to manipulate the internal state of the <see cref="IUnityContainer"/>.
    /// </summary>
    public abstract partial class ExtensionContext
    {
        /// <summary>
        /// The instance of extended container
        /// </summary>
        /// <value>The instance of <see cref="UnityContainer"/></value>
        public abstract UnityContainer Container { get; }

        /// <summary>
        /// The collection of disposable objects this container is responsible for
        /// </summary>
        /// <value>The <see cref="ICollection{IDisposable}"/> of <see cref="IDisposable"/> objects</value>
        public abstract ILifetimeContainer Lifetime { get; }

        /// <summary>
        /// The policies this container uses.
        /// </summary>
        /// <remarks>The <see cref="IPolicyList"/> the that container uses to build objects.</remarks>
        public abstract IPolicies Policies { get; }
    }
}
