using Unity.Container;
using Unity.Pipeline;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        internal ContainerScope    _scope;
        internal readonly Defaults _policies;

        #endregion


        #region Constructors

        /// <summary>
        /// Default <see cref="UnityContainer"/> constructor
        /// </summary>
        public UnityContainer(string? name = "root")
        {
            Root = this;
            Name = name;

            _policies = new Defaults();
            _scope = new ContainerScope(this, 3, 2);
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
            Name   = name;
            Root   = parent.Root;
            Parent = parent;

            _policies = parent.Root._policies;
            _scope = parent._scope.CreateChildScope(this);

            // Child Container Specific
        }

        #endregion
    }
}
