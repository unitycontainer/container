using System;
using System.Collections.Generic;
using Unity.BuiltIn;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        private readonly int BUILT_IN_CONTRACT_COUNT;
        private readonly UnityContainer[] _ancestry;

        internal Scope _scope;

        internal readonly Defaults _policies;

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
            _ancestry = new[] { this };

            // Registration Scope
            _scope = new ContainerScope(capacity);
            _scope.Setup(_policies);

            var manager = new ContainerLifetimeManager(this);
            _scope.BuiltIn(typeof(IUnityContainer),      manager);
            _scope.BuiltIn(typeof(IUnityContainerAsync), manager);
            _scope.BuiltIn(typeof(IServiceProvider),     manager);
            BUILT_IN_CONTRACT_COUNT = _scope.Count;

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
            _ancestry = parent._ancestry.CreateClone(this);

            // Registration Scope
            _scope = parent._scope.CreateChildScope(capacity);

            var manager = new ContainerLifetimeManager(this);
            _scope.BuiltIn(typeof(IUnityContainer),      manager);
            _scope.BuiltIn(typeof(IUnityContainerAsync), manager);
            _scope.BuiltIn(typeof(IServiceProvider),     manager);
            BUILT_IN_CONTRACT_COUNT = _scope.Count;
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            // Child container dispose
            if (null != Parent)
            { 
                Parent.Registering -= OnParentRegistering;
                Parent._scope.Remove(this);
            }

            _registering = null;
            _childContainerCreated = null;

            _scope.Dispose();
        }

        #endregion
    }
}
