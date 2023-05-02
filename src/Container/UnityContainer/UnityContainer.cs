using Unity.Container;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public sealed partial class UnityContainer
    {
        #region Fields

        internal Scope Scope;
        internal ILifetimeContainer LifetimeContainer;
        internal readonly Policies Policies;

        private PrivateExtensionContext? _context;
        private event RegistrationEvent? _registering;
        private event ChildCreatedEvent? _childContainerCreated;
        private List<IUnityContainerExtensionConfigurator>? _extensions;

        #endregion


        #region Constructors

        /// <summary>
        /// Create <see cref="UnityContainer"/> container
        /// </summary>
        /// <param name="name">Name of the container</param>
        /// <param name="capacity">Pre-allocated capacity</param>
        public UnityContainer(string name, int capacity)
        {
            Name = name;
            Root = this;
            Policies = new Policies();
            LifetimeContainer = new LifetimeContainer();

            // Setup Scope
            var manager = new InternalLifetimeManager(this);
            Scope = new HashScope(capacity);
            Scope.BuiltIn(typeof(IUnityContainer),  manager);
            Scope.BuiltIn(typeof(IServiceProvider), manager);

            // Initialize Built-In Components
            UnityDefaultBehaviorExtension.Initialize(Policies);
        }

        /// <summary>
        /// Child container constructor
        /// </summary>
        /// <param name="parent">Parent <see cref="UnityContainer"/></param>
        /// <param name="name">Name of this container</param>
        private UnityContainer(UnityContainer parent, string? name, int capacity)
        {
            Name   = name;
            Root   = parent.Root;
            Parent = parent;
            Policies = parent.Root.Policies;
            LifetimeContainer = new LifetimeContainer();

            // Registration Scope
            Scope = parent.Scope.CreateChildScope(capacity);

            var manager = new InternalLifetimeManager(this);
            Scope.BuiltIn(typeof(IUnityContainer),  manager);
            Scope.BuiltIn(typeof(IServiceProvider), manager);
        }

        /// <summary>
        /// Creates container with name 'root' and allocates 37 slots for contracts
        /// </summary>
        public UnityContainer() : this(DEFAULT_ROOT_NAME, DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container and allocates 37 slots for contracts
        /// </summary>
        /// <param name="name">Name of the container</param>
        public UnityContainer(string name) : this(name, DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container with name 'root'
        /// </summary>
        /// <param name="capacity">Pre-allocated capacity</param>
        public UnityContainer(int capacity) : this(DEFAULT_ROOT_NAME, capacity)
        { }

        #endregion


        #region Finalizer

        /// <summary>
        /// Finalizer
        /// </summary>
        ~UnityContainer() => Dispose(disposing: false);
        
        #endregion


        #region Events

        private event RegistrationEvent Registering
        {
            add
            {
                // TODO: Registration propagation?
                //if (null != Parent && _registering is null)
                //    Parent.Registering += OnParentRegistering;

                _registering += value;
            }

            remove
            {
                _registering -= value;

                //if (_registering is null && null != Parent)
                //    Parent.Registering -= OnParentRegistering;
            }
        }

        private event ChildCreatedEvent ChildContainerCreated
        {
            add => _childContainerCreated += value;
            remove => _childContainerCreated -= value;
        }

        #endregion
    }
}
