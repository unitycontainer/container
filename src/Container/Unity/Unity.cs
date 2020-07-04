using Unity.Container;
using Unity.Pipeline;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        internal readonly int _level;
        private  readonly string?  _name;
        internal readonly Defaults _policies;
        internal readonly UnityContainer  _root;
        internal readonly UnityContainer? _parent;
        
        internal ContainerScope _scope;

        #endregion


        #region Constructors

        /// <summary>
        /// Default <see cref="UnityContainer"/> constructor
        /// </summary>
        public UnityContainer(string? name = "root")
        {
            // Singletons
            _root = this;
            _policies = new Defaults();

            // Each Container
            _name = name;
            _scope = new ContainerScope(this);

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
            _level  = parent._level + 1;

            // Each Container
            _name  = name ?? GetHashCode().ToString();
            _scope = parent._scope.CreateChildScope(this);

            // Child Container Specific
        }

        #endregion
    }
}
