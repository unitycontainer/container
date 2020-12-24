using System;
using System.Collections.Generic;
using Unity.BuiltIn;
using Unity.Container;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        internal Scope Scope;
        internal readonly Defaults Policies;

        #endregion


        #region Constructors

        /// <summary>
        /// Create <see cref="UnityContainer"/> container
        /// </summary>
        /// <param name="name">Name of the container</param>
        /// <param name="capacity">Preallocated capacity</param>
        public UnityContainer(string name, int capacity)
        {
            Name = name;
            Root = this;

            // Setup Defaults
            Policies = new Defaults();
            
            // TODO: Setup extension points
            Policies.Set<ResolverFactory<PipelineContext>>(typeof(IEnumerable<>), ResolveUnregisteredEnumerable);

            // Setup Scope
            var manager = new ContainerLifetimeManager(this);
            Scope = new ContainerScope(capacity);
            Scope.BuiltIn(typeof(IUnityContainer),      manager);
            Scope.BuiltIn(typeof(IUnityContainerAsync), manager);
            Scope.BuiltIn(typeof(IServiceProvider),     manager);

            // Initialize Extensions
            _context = new PrivateExtensionContext(this);

            // Initialize Default Extensions
            UnityDefaultBehaviorExtension.Initialize(_context);
            UnityDefaultStrategiesExtension.Initialize(_context);
        }

        /// <summary>
        /// Child container constructor
        /// </summary>
        /// <param name="parent">Parent <see cref="UnityContainer"/></param>
        /// <param name="name">Name of this container</param>
        protected UnityContainer(UnityContainer parent, string? name, int capacity)
        {
            Name   = name;
            Root   = parent.Root;
            Parent = parent;
            Policies = parent.Root.Policies;

            // Registration Scope
            Scope = parent.Scope.CreateChildScope(capacity);

            var manager = new ContainerLifetimeManager(this);
            Scope.BuiltIn(typeof(IUnityContainer),      manager);
            Scope.BuiltIn(typeof(IUnityContainerAsync), manager);
            Scope.BuiltIn(typeof(IServiceProvider),     manager);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~UnityContainer() => Dispose(disposing: false);

        #endregion


        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            // Explicit Dispose
            if (disposing)
            {
                _registering = null;
                _childContainerCreated = null;
            }

            Scope.Dispose();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
