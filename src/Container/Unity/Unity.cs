using Unity.Builder;
using Unity.BuiltIn;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer : IDisposable
    {
        #region Fields

        internal Scope Scope;
        internal readonly Policies<BuilderContext> Policies;

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
            Policies = new Policies<BuilderContext>();
            
            // Setup Scope
            var manager = new InternalLifetimeManager(this);
            Scope = new ContainerScope(capacity);
            Scope.BuiltIn(typeof(IUnityContainer),      manager);
            Scope.BuiltIn(typeof(IServiceProvider),     manager);

            // Initialize Built-In Components
            UnityDefaultBehaviorExtension.Initialize(Policies);
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

            var manager = new InternalLifetimeManager(this);
            Scope.BuiltIn(typeof(IUnityContainer),      manager);
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
