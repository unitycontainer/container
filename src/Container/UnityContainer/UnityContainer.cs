using Unity.BuiltIn;
using Unity.Container;
using Unity.Extension;

namespace Unity
{
    public sealed partial class UnityContainer : IDisposable
    {
        #region Fields

        internal Scope Scope;
        internal readonly Policies Policies;

        private PrivateExtensionContext? _context;
        private event RegistrationEvent? _registering;
        private event ChildCreatedEvent? _childContainerCreated;
        private List<IUnityContainerExtensionConfigurator>? _extensions;

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

        // TODO: Find better place 
        private void OnParentRegistering(object container, in ReadOnlySpan<RegistrationDescriptor> registrations)
            => _registering?.Invoke(container, in registrations);

        private event ChildCreatedEvent ChildContainerCreated
        {
            add => _childContainerCreated += value;
            remove => _childContainerCreated -= value;
        }

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

            // Registration Scope
            Scope = parent.Scope.CreateChildScope(capacity);

            var manager = new InternalLifetimeManager(this);
            Scope.BuiltIn(typeof(IUnityContainer),  manager);
            Scope.BuiltIn(typeof(IServiceProvider), manager);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~UnityContainer() => Dispose(disposing: false);

        #endregion


        #region IDisposable

        private void Dispose(bool disposing)
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
