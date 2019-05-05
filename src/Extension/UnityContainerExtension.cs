using System;

namespace Unity.Extension
{
    #pragma warning disable CS8618

    // Non-nullable field is uninitialized.
    /// <summary>
    /// Base class for all <see cref="IUnityContainer"/> extension objects.
    /// </summary>
    public abstract class UnityContainerExtension : IUnityContainerExtensionConfigurator
    {
        /// <summary>
        /// The container calls this method when the extension is added.
        /// </summary>
        /// <param name="context">A <see cref="ExtensionContext"/> instance that gives the
        /// extension access to the internals of the container.</param>
        public void InitializeExtension(ExtensionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Container = context.Container;
            Context = context;

            Initialize();
        }

        /// <summary>
        /// The container this extension has been added to.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> that this extension has been added to.</value>
        public UnityContainer Container { get; private set; }

        /// <summary>
        /// The <see cref="ExtensionContext"/> object used to manipulate
        /// the inner state of the container.
        /// </summary>
        protected ExtensionContext Context { get; private set; }

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="ExtensionContext"/> by adding strategies, policies, etc. to
        /// install it's functions into the container.</remarks>
        protected abstract void Initialize();
    }

    #pragma warning restore CS8618 // Non-nullable field is uninitialized.
}
