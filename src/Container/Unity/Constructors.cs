using Unity.Lifetime;
using Unity.Scope;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private string?          _name; 
        private UnityContainer   _root;
        private UnityContainer?  _parent;

        private LifetimeManager  _typeLifetime;
        private LifetimeManager  _factoryLifetime;
        private LifetimeManager  _instanceLifetime;

        private ScopeContext?        _scopeContext;
        private RegistrationScope    _scope;
        private RootExtensionContext _rootContext;

        #endregion


        #region Constructors

        /// <summary>
        /// Default <see cref="UnityContainer"/> constructor
        /// </summary>
        public UnityContainer(string? name = "root")
        {
            // Root
            _root = this;
            _name = name;
            _rootContext = new RootExtensionContext(this);

            // Default Lifetimes
            _typeLifetime = new TransientLifetimeManager();
            _factoryLifetime = _typeLifetime;
            _instanceLifetime = new ContainerControlledLifetimeManager();

            // Container Scope // TODO: replace
            _scope = new RegistrationScopeAsync();
        }


        protected UnityContainer(UnityContainer parent, string? name = null)
        {
            // Ancestry
            _parent      = parent;
            _name        = name;
            _root        = parent._root;
            _rootContext = parent._rootContext;

            // Lifetimes
            _typeLifetime     = parent._typeLifetime;
            _factoryLifetime  = parent._factoryLifetime;
            _instanceLifetime = parent._instanceLifetime;

            // Container Scope
            _scope = parent._scope.CreateChildScope();
        }

        #endregion
    }
}
