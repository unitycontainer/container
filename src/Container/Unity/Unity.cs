using System;
using Unity.BuiltIn;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        private readonly int _level;
        private readonly int BUILT_IN_CONTRACT_COUNT;

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
            var manager = new ContainerLifetimeManager(this);
            _scope = new ContainerScope(capacity);
            _scope.Add(typeof(IUnityContainer),      manager);
            _scope.Add(typeof(IUnityContainerAsync), manager);
            _scope.Add(typeof(IServiceProvider),     manager);
            BUILT_IN_CONTRACT_COUNT = _scope.Count;

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
            var manager = new ContainerLifetimeManager(this);
            _scope = parent._scope.CreateChildScope(capacity);
            _scope.Add(typeof(IUnityContainer),      manager);
            _scope.Add(typeof(IUnityContainerAsync), manager);
            _scope.Add(typeof(IServiceProvider),     manager);
            BUILT_IN_CONTRACT_COUNT = _scope.Count;
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
