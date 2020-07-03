using Unity.Container;
using Unity.Pipeline;
using Unity.Scope;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields
        
        private readonly string?  _name;
        private readonly Policies _policies;
        private readonly UnityContainer  _root;
        private readonly UnityContainer? _parent;
        
        private ContainerScope _scope;

        #endregion


        #region Constructors

        /// <summary>
        /// Default <see cref="UnityContainer"/> constructor
        /// </summary>
        public UnityContainer(string? name = "root")
        {
            // Singletons
            _root = this;
            _policies = new Policies();

            // Each Container
            _name = name;
            _scope = new ContainerScopeAsync(this);

            // Root Container Specific
            _context = new PrivateExtensionContext(this);

            // Setup Processors
            ConstructorProcessor.SetupProcessor(_context);
                  FieldProcessor.SetupProcessor(_context);
               PropertyProcessor.SetupProcessor(_context);
                 MethodProcessor.SetupProcessor(_context);
        }

        /// <summary>
        /// Child container constructor
        /// </summary>
        /// <param name="parent">Parent <see cref="UnityContainer"/></param>
        /// <param name="name">Name of this container</param>
        protected UnityContainer(UnityContainer parent, string? name = null)
        {
            // Singletons
            _root     = parent._root;
            _policies = parent._root._policies;

            // Ancestry
            _parent = parent;

            // Each Container
            _name  = name;
            _scope = parent._scope.CreateChildScope(this);

            // Child Container Specific
        }

        #endregion
    }
}
