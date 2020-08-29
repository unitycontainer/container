using System;
using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope : Scope
    {
        #region Fields

        protected Metadata[] ContractsMeta;

        #endregion


        #region Constructors

        // Root constructor
        internal ContainerScope(int capacity)
            : base(capacity)
        {
            // Registrations
            ContractsMeta = new Metadata[Prime.Numbers[ContractsPrime]];
        }

        // Child constructor
        protected ContainerScope(Scope scope, int capacity)
            : base(scope, capacity)
        {
            // Registrations
            ContractsMeta = new Metadata[Prime.Numbers[ContractsPrime]];
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope, scope.Sync)
        {
            // Registrations
            ContractsMeta = scope.ContractsMeta;
        }

        #endregion
    }
}
