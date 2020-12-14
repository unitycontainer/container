using System;
using System.Collections.Generic;
using Unity.BuiltIn;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        internal Scope _scope;
        internal readonly Defaults _policies;
        private  readonly UnityContainer[] _ancestry;

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

            _policies = new Defaults();
            _context  = new PrivateExtensionContext(this);
            _ancestry = new[] { this }; // TODO: Ancestry must be revisited

            // Registration Scope
            _scope = new ContainerScope(capacity);
            _scope.Setup(_policies);

            var manager = new ContainerLifetimeManager(this);
            _scope.BuiltIn(typeof(IUnityContainer),      manager);
            _scope.BuiltIn(typeof(IUnityContainerAsync), manager);
            _scope.BuiltIn(typeof(IServiceProvider),     manager);

            // Add internal factories
            _policies.Set<PipelineFactory>(BuildPipelineUnregistered);
            _policies.Set<PipelineFactory>(typeof(IEnumerable<>), ResolveUnregisteredEnumerable);

            // Setup Built-In Components
            BuiltInComponents.Setup(_context);
        }

        /// <summary>
        /// Child container constructor
        /// </summary>
        /// <param name="parent">Parent <see cref="UnityContainer"/></param>
        /// <param name="name">Name of this container</param>
        protected UnityContainer(UnityContainer parent, string? name, int capacity)
        {
            Name   = name;
            Parent = parent;

            _policies = parent.Root._policies;
            _ancestry = parent._ancestry.CreateClone(this);// TODO: Ancestry must be revisited

            // Registration Scope
            _scope = parent._scope.CreateChildScope(capacity);

            var manager = new ContainerLifetimeManager(this);
            _scope.BuiltIn(typeof(IUnityContainer),      manager);
            _scope.BuiltIn(typeof(IUnityContainerAsync), manager);
            _scope.BuiltIn(typeof(IServiceProvider),     manager);
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

            _scope.Dispose();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
