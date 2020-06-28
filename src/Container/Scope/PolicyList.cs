using System;
using Unity.Policy;

namespace Unity.Scope
{
    public partial class RegistrationScope : IPolicyList
    {
        /// <inheritdoc />
        public void Clear(Type? type, string? name, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public object? Get(Type type, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public object? Get(Type? type, string? name, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Set(Type type, Type policyInterface, object policy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Set(Type? type, string? name, Type policyInterface, object policy)
        {
            throw new NotImplementedException();
        }
    }
}
