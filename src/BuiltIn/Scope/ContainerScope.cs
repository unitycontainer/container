using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope : Scope
    {
        #region Fields

        protected Metadata[] _contractMeta;

        #endregion


        #region Constructors

        // Root constructor
        internal ContainerScope(int capacity)
            : base(capacity)
        {
            var size = Prime.GetNext(capacity * ReLoadFactor);

            // Registrations
            _contractMeta = new Metadata[size];
            _contractMeta.BufferLength(_contractData.Length);
        }

        // Child constructor
        protected ContainerScope(Scope scope, int capacity)
            : base(scope, capacity)
        {
            var size = Prime.GetNext(capacity * ReLoadFactor);

            // Registrations
            _contractMeta = new Metadata[size];
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
