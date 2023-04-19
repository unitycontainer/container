using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class HashScope : Scope
    {
        #region Fields

        protected Metadata[] Meta;

        #endregion


        #region Constructors

        // Root constructor
        internal HashScope(int capacity = 0)
            : base(capacity)
        {
            Meta = new Metadata[Storage.Prime.Numbers[Prime]];
        }

        // Child constructor
        protected HashScope(Scope scope, int capacity)
            : base(scope, capacity)
        {
            Meta = new Metadata[Storage.Prime.Numbers[Prime]];
        }

        // Copy constructor
        protected HashScope(HashScope scope)
            : base(scope, scope.SyncRoot)
        {
            Meta = scope.Meta;
        }

        #endregion
    }
}
