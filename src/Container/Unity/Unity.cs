using System;
using Unity.BuiltIn;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        private readonly int _level;
        private readonly int DEFAULT_CONTRACTS;

        internal Scope   _scope;
        internal readonly Defaults _policies;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates container with name 'root' and allocates 37 slots for contracts
        /// </summary>
        public UnityContainer() : this(Defaults.DEFAULT_ROOT_NAME, Defaults.DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container and allocates 37 slots for contracts
        /// </summary>
        /// <param name="name">Name of the container</param>
        public UnityContainer(string name) : this(name, Defaults.DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container with name 'root'
        /// </summary>
        /// <param name="capacity">Preallocated capacity</param>
        public UnityContainer(int capacity) : this(Defaults.DEFAULT_ROOT_NAME, capacity)
        { }

        /// <summary>
        /// Create <see cref="UnityContainer"/> container
        /// </summary>
        /// <param name="name">Name of the container</param>
        /// <param name="capacity">Preallocated capacity</param>
        public UnityContainer(string name, int capacity)
        {
            Root = this;
            Name = name;

            _level    = 1;
            _policies = new Defaults();
            _context  = new PrivateExtensionContext(this);

            // Registration Scope
            _scope = new ContainerScope(capacity);
            _scope.Add(new ContainerLifetimeManager(this), typeof(IUnityContainer), 
                                                           typeof(IUnityContainerAsync), 
                                                           typeof(IServiceProvider));
            DEFAULT_CONTRACTS = _scope.Contracts;

            // Setup Default algorithms
            _policies.Set(typeof(ProducerFactory), (ProducerFactory)DefaultProducerFactory);

            _policies.Set(typeof(Defaults.ResolveUnregisteredDelegate),   (Defaults.ResolveUnregisteredDelegate)ResolveUnregistered);
            _policies.Set(typeof(Defaults.RegistrationProducerDelegate), (Defaults.RegistrationProducerDelegate)ResolveRegistration);
            _policies.Set(typeof(Defaults.ResolveArrayDelegate),                 (Defaults.ResolveArrayDelegate)ResolveArray);

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
            Root   = parent.Root;
            Parent = parent;

            _level    = parent._level + 1;
            _policies = parent.Root._policies;
            
            // Registration Scope
            _scope = parent._scope.CreateChildScope(capacity);
            _scope.Add(new ContainerLifetimeManager(this),
                typeof(IUnityContainer), 
                typeof(IUnityContainerAsync), 
                typeof(IServiceProvider));
            DEFAULT_CONTRACTS = _scope.Contracts;
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            // Child container dispose
            if (null != Parent) Parent.Registering -= OnParentRegistering;

            _registering = null;
            _childContainerCreated = null;

            _scope.Dispose();
        }

        #endregion
    }
}
