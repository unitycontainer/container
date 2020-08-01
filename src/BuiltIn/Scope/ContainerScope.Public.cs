using System;
using System.Runtime.InteropServices;
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
            ContractUnion union = default;

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
                        union.AsStruct.Name = null;

                        // Anonymous contracts
                        foreach (var type in descriptor.RegisterAs)
                        {
                            if (null == type) continue;

                            union.AsStruct.Type = type;
                            union.AsStruct.HashCode = type.GetHashCode();

                            Add(in union.Contract, descriptor.Manager);
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

                            union.AsStruct.Type = type;
                            union.AsStruct.Name = nameInfo.Name;
                            union.AsStruct.HashCode = Contract.GetHashCode(type.GetHashCode(), (int)nameInfo.Hash);

                            var position = Add(in union.Contract, descriptor.Manager);
                            if (0 != position) nameInfo.Register(position);
                        }
                    }
                }
            }
        }

        public override void AddAsync(object? state)
            => throw new NotImplementedException("This feature requires 'Unity.Professional' extension");

        #endregion


        #region Contains

        /// <inheritdoc />
        public override bool Contains(Type type, string? name)
        {
            var hash = (uint)Contract.GetHashCode(type, name);
            var bucket = hash % _contractMeta.Length;
            var position = _contractMeta[bucket].Position;

            while (position > 0)
            {
                ref var candidate = ref _contractData[position];
                if (candidate._contract.Type == type &&
                    candidate._contract.Name == name)
                    return true;

                position = _contractMeta[position].Next;
            }

            return false;
        }

        #endregion


        #region Child Scope

        /// <inheritdoc />
        public override Scope CreateChildScope(int capacity) => new ContainerScope(this, capacity);

        #endregion


        #region Contract Proxy

        [StructLayout(LayoutKind.Explicit)]
        private struct ContractUnion
        {
            [FieldOffset(0)] internal Contract Contract;
            [FieldOffset(0)] internal AsStruct AsStruct;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AsStruct
        {
            public int HashCode;
            public Type Type;
            public string? Name;
        }

        #endregion
    }
}
