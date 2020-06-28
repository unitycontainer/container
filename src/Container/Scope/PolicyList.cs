using System;

namespace Unity.Scope
{
    public partial class RegistrationScope
    {
        /// <inheritdoc />
        public override void Clear(Type? type, string? name, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override object? Get(Type type, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override object? Get(Type? type, string? name, Type policyInterface)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Set(Type type, Type policyInterface, object policy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Set(Type? type, string? name, Type policyInterface, object policy)
        {
            throw new NotImplementedException();
        }
    }
}
