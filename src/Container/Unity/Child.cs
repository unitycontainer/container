namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private UnityContainer? _parent;

        #endregion


        #region Constructors

        protected UnityContainer(UnityContainer parent)
        {
            // Parent
            _parent = parent;

            // Extension Management
            _context    = parent._context;
            _extensions = parent._extensions;

            // Container Scope
            _scope = parent._scope.CreateChildScope();
        }

        #endregion
    }
}
