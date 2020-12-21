using System;
using System.Collections.Generic;
using Unity.BuiltIn;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        internal Scope Scope;
        internal readonly Defaults Policies;
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

            // Setup Defaults
            Policies = new Defaults();
            Policies.Set<PipelineFactory<PipelineContext>>(BuildPipelineUnregistered);
            Policies.Set<ContextualFactory<PipelineContext>>(BuildPipelineRegistered);
            Policies.Set<Func<UnityContainer, Type, Type>>(typeof(Array), GetArrayTargetType);
            Policies.Set<PipelineFactory<PipelineContext>>(typeof(IEnumerable<>), ResolveUnregisteredEnumerable);
            Policies.TypeChain.ChainChanged = OnBuildChainChanged;
            Policies.FactoryChain.ChainChanged = OnBuildChainChanged;
            Policies.InstanceChain.ChainChanged = OnBuildChainChanged;

            // Extension Context
            _context = new PrivateExtensionContext(this);

            // Setup Scope
            var manager = new ContainerLifetimeManager(this);
            Scope = new ContainerScope(capacity);
            Scope.Setup(Policies);                                // TODO: Requires revisiting
            Scope.BuiltIn(typeof(IUnityContainer),      manager);
            Scope.BuiltIn(typeof(IUnityContainerAsync), manager);
            Scope.BuiltIn(typeof(IServiceProvider),     manager);

            _ancestry = new[] { this };                             // TODO: Ancestry must be revisited

            // Setup Built-In Components
            Processors.Setup(_context);
            Components.Setup(_context);
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

            Policies = parent.Root.Policies;
            _ancestry = parent._ancestry.CreateClone(this);// TODO: Ancestry must be revisited

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
