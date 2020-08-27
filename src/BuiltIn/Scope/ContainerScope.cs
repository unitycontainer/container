using System;
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
            // Registrations
            _contractMeta = new Metadata[Prime.Numbers[_contractPrime]];
        }

        // Child constructor
        protected ContainerScope(Scope scope, int capacity)
            : base(scope, capacity)
        {
            // Registrations
            _contractMeta = new Metadata[Prime.Numbers[_contractPrime]];
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
