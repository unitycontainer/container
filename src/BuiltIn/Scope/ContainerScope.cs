using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope : Scope
    {
        #region Fields

        protected Metadata[] Meta;

        #endregion


        #region Constructors

        // Root constructor
        internal ContainerScope(int capacity)
            : base(capacity)
        {
            Meta = new Metadata[Storage.Prime.Numbers[Prime]];
        }

        // Child constructor
        protected ContainerScope(Scope scope, int capacity)
            : base(scope, capacity)
        {
            Meta = new Metadata[Storage.Prime.Numbers[Prime]];
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope, scope.Sync)
        {
            Meta = scope.Meta;
        }

        #endregion
    }
}
