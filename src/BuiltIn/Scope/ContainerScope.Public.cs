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
            int required = START_INDEX;
            ContractUnion union = default;

            // Calculate required storage
            for (var i = 0; span.Length > i; i++) required += span[i].RegisterAs.Length;

            lock (_syncRoot)
            {
                // Expand registry if required
                required = _contractCount + required;
                if (required >= _contractData.Length) Expand(required);

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
        public override bool Contains(in Contract contract)
        {
            var meta = _contractMeta;
            var bucket = (uint)contract.HashCode % meta.Length;
            var position = meta[bucket].Position;

            while (position > 0)
            {
                ref var candidate = ref _contractData[position];
                if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                    candidate._contract.Name == contract.Name)
                    return true;

                position = meta[position].Next;
            }

            return Next?.Contains(in contract) ?? false;
        }

        #endregion

        
        #region Get

        public override RegistrationManager? Get(in Contract contract)
        {
            var meta = _contractMeta;
            var target = (uint)contract.HashCode % meta.Length;
            var position = meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref _contractData[position];
                if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                    candidate._contract.Name == contract.Name)
                    return candidate._manager;

                position = meta[position].Next;
            }

            return null;
        }

        public override bool Get(in Contract contract, out RegistrationManager? manager)
        {
            var meta = _contractMeta;
            var target = (uint)contract.HashCode % meta.Length;
            var position = meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref _contractData[position];
                if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                    candidate._contract.Name == contract.Name)
                {
                    manager = candidate._manager;
                    return true;
                }

                position = meta[position].Next;
            }

            manager = null;
            return false;
        }

        public override RegistrationManager? Get(in Contract contract, in Contract generic)
        {
            var count = _contractCount;
            var meta  = _contractMeta;
            var position = meta[(uint)contract.HashCode % meta.Length].Position;

            // Search for exact match

            while (position > 0)
            {
                ref var candidate = ref _contractData[position];
                if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                    candidate._contract.Name == contract.Name)
                    return candidate._manager;

                position = meta[position].Next;
            }

            // Search for generic factory

            position = meta[(uint)generic.HashCode % meta.Length].Position;
            while (position > 0)
            {
                ref var factory = ref _contractData[position];
                if (ReferenceEquals(factory._contract.Type, generic.Type) &&
                    factory._contract.Name == generic.Name)
                {
                    // Found generic factory

                    lock (_syncRoot)
                    {
                        var target = (uint)contract.HashCode % _contractMeta.Length;
                        
                        // Check again if count has changed

                        if (count != _contractCount)
                        { 
                            position = _contractMeta[target].Position;

                            while (position > 0)
                            {
                                ref var candidate = ref _contractData[position];
                                if (ReferenceEquals(candidate._contract.Type, contract.Type) && 
                                    candidate._contract.Name == contract.Name)
                                {
                                    // Found existing
                                    return candidate._manager;
                                }

                                position = _contractMeta[position].Next;
                            }
                        }

                        // Nothing is found, add new

                        count = _contractCount + 1;

                        // Expand if required

                        if (_contractData.Length <= count)
                        {
                            Expand();
                            target = (uint)contract.HashCode % _contractMeta.Length;
                        }

                        // Clone manager
                        var manager = factory.LifetimeManager.Clone();

                        ref var bucket = ref _contractMeta[target];
                        _contractData[count] = new ContainerRegistration(contract.With(factory.Name), manager);
                        _contractMeta[count].Next = bucket.Position;
                        bucket.Position = count;
                        _contractCount  = count; // changes last once everything is in place

                        return manager;
                    }
                }

                position = meta[position].Next;
            }

            return null;
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
