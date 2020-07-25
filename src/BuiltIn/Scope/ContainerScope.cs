using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope : Scope
    {
        #region Constants

        protected const string ASYNC_ERROR_MESSAGE = "This feature requires 'Unity.Professional' extension";

        #endregion


        #region Fields

        // Registrations
        protected Metadata[] _contractMeta;

        #endregion


        #region Constructors

        // Root constructor
        internal ContainerScope()
            : base()
        {
            // Registrations
            _contractMeta = new Metadata[Prime.Numbers[PRIME_ROOT_INDEX + 1]];
            _contractMeta.BufferLength(_contractData.Length);
        }

        // Child constructor
        protected ContainerScope(Scope scope)
            : base(scope)
        {
            // Registrations
            _contractMeta = new Metadata[Prime.Numbers[PRIME_CHILD_INDEX + 1]];
            _contractMeta.BufferLength(_contractData.Length);
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope, scope._syncRoot)
        {
            // Registrations
            _contractMeta = scope._contractMeta;
        }

        #endregion
    }
}
