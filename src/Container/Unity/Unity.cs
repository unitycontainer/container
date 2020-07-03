using Unity.Container;
using Unity.Pipeline;
using Unity.Scope;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private string? _name;
        private ContainerScope _scope;
        private UnityContainer _root;
        private UnityContainer? _parent;
        private Policies<PipelineProcessor, BuilderStage> _policies;

        #endregion


        #region Constructors

        /// <summary>
        /// Default <see cref="UnityContainer"/> constructor
        /// </summary>
        public UnityContainer(string? name = "root")
        {
            // Singletons
            _root = this;
            _policies = new Policies<PipelineProcessor, BuilderStage>();

            // Each Container
            _name = name;
            _scope = new ContainerScopeAsync(this);
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
        }

        #endregion
    }
}
