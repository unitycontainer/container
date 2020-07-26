using System;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Add

        /// <inheritdoc />
        internal override void Add(RegistrationManager manager, params Type[] registerAs)
        {
            // Expand if required
            var required = _contractCount + registerAs.Length;
            if (required >= _contractMeta.MaxIndex()) Expand(required);

            // Iterate and register types
            foreach (var type in registerAs)
            {
                // Skip invalid types
                if (null == type) continue;

                Add(new Contract(type), manager);
                _version--;
            }
        }

        /// <inheritdoc />
        public override void Add(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            int required = 0;

            // Calculate required storage
            for (var i = 0; span.Length > i; i++)
                required += span[i].RegisterAs.Length;

            lock (_syncRoot)
            {
                // Expand registry if required
                required = _contractCount + required;
                if (required >= _contractMeta.MaxIndex()) Expand(required);

                for (var i = 0; span.Length > i; i++)
                {
                    ref readonly RegistrationDescriptor descriptor = ref span[i];

                    if (null == descriptor.Name)
                    {
                        // Anonymous contracts
                        foreach (var type in descriptor.RegisterAs)
                        {
                            if (null == type) continue;
                            Add(new Contract(type), descriptor.Manager);
                        }
                    }
                    else
                    {
                        // Named contracts
                        var nameInfo = GetNameInfo(descriptor.Name);

                        // Ensure required storage
                        nameInfo.Resize(descriptor.RegisterAs.Length);

                        // Register contracts
                        foreach (var type in descriptor.RegisterAs)
                        {
                            if (null == type) continue;

                            var hash = Contract.GetHashCode(type.GetHashCode(), (int)nameInfo.Hash);
                            var position = Add(new Contract(hash, type, nameInfo.Name), descriptor.Manager);
                            if (0 != position) nameInfo.Register(position);
                        }
                    }
                }
            }
        }

        public override void Add(in ReadOnlyMemory<RegistrationDescriptor> memory) 
            => throw new NotImplementedException(ASYNC_ERROR_MESSAGE);

        #endregion


        #region Contains

        /// <inheritdoc />
        public override bool Contains(Type type)
        {
            var scope = this;

            do
            {
                var bucket = (uint)type.GetHashCode() % scope._contractMeta.Length;
                var position = scope._contractMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._contractData[position];
                    if (null == candidate._contract.Name && candidate._contract.Type == type)
                        return true;

                    position = scope._contractMeta[position].Next;
                }

            } while ((scope = (ContainerScope?)scope.Parent) != null);

            return false;
        }

        /// <inheritdoc />
        public override bool Contains(Type type, string name)
        {
            var scope = this;

            do
            {
                if (0 == scope._namesCount) continue;

                var hash = type.GetHashCode(name);
                var bucket = hash % scope._contractMeta.Length;
                var position = scope._contractMeta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope._contractData[position];
                    if (candidate._contract.Type == type && candidate._contract.Name == name)
                        return true;

                    position = scope._contractMeta[position].Next;
                }

            } while ((scope = (ContainerScope?)scope.Parent) != null);

            return false;
        }

        #endregion


        #region Child Scope

        /// <inheritdoc />
        public override Scope CreateChildScope() => new ContainerScope((Scope)this);

        #endregion
    }
}
