using System;
using System.Diagnostics;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Contains

        /// <inheritdoc />
        public override bool Contains(in Contract contract)
        {
            var meta = Meta;
            var bucket = (uint)contract.HashCode % meta.Length;
            var position = meta[bucket].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                    candidate.Contract.Name == contract.Name)
                    return true;

                position = meta[position].Next;
            }

            return Next?.Contains(in contract) ?? false;
        }

        #endregion


        #region Insertion

        /// <inheritdoc />
        public override void Add(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            lock (Sync)
            {
                for (var i = 0; span.Length > i; i++)
                {
                    ref readonly RegistrationDescriptor descriptor = ref span[i];
                    
                    if (null == descriptor.Name)
                    {
                        // Expand registry if required
                        var required = START_INDEX + Index + descriptor.RegisterAs.Length;
                        if (required >= Data.Length) Expand(required);

                        AddDefault(in descriptor);
                    }
                    else
                    {
                        // Expand registry if required
                        var required = 2 + Index + descriptor.RegisterAs.Length * 2;
                        if (required >= Data.Length) Expand(required);

                        AddContract(in descriptor);
                    }
                }
            }
        }

        /// <inheritdoc />
        internal override void SetInternal(RegistrationManager manager, params Type[] registerAs)
        {
            // Iterate and register types
            foreach (var type in registerAs)
            {
                Debug.Assert(null != type);
                AddDefault(type!, manager);
                Revision--;
            }
        }

        public override void AddAsync(object? state)
            => throw new NotImplementedException("This feature requires 'Unity.Professional' extension");

        #endregion


        #region Get

        /// <inheritdoc />
        public override RegistrationManager? Get(in Contract contract)
        {
            var meta = Meta;
            var target = (uint)contract.HashCode % meta.Length;
            var position = meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                    candidate.Contract.Name == contract.Name)
                    return candidate.Manager;

                position = meta[position].Next;
            }

            return null;
        }

        /// <inheritdoc />
        public override RegistrationManager? Get(in Contract contract, in Contract generic)
        {
            var meta  = Meta;
            var position = meta[(uint)generic.HashCode % meta.Length].Position;

            // Search for generic factory

            while (position > 0)
            {
                ref var factory = ref Data[position];
                if (ReferenceEquals(factory.Internal.Contract.Type, generic.Type) &&
                    factory.Internal.Contract.Name == generic.Name)
                {
                    // Found generic factory

                    lock (Sync)
                    {
                        // Check if contract is created already

                        var target = (uint)contract.HashCode % Meta.Length;
                        position = Meta[target].Position;

                        while (position > 0)
                        {
                            ref var candidate = ref Data[position].Internal;
                            if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                                candidate.Contract.Name == contract.Name)
                            {
                                // Found existing
                                return candidate.Manager;
                            }

                            position = Meta[position].Next;
                        }

                        // Nothing is found, add new and expand if required
                        if (Data.Length <= ++Index)
                        {
                            Expand(Index);
                            target = (uint)contract.HashCode % Meta.Length;
                        }

                        // Clone manager
                        var manager = factory.Registration.LifetimeManager.Clone();

                        ref var bucket = ref Meta[target];
                        Data[Index] = new Entry(contract.HashCode, contract.Type, factory.Registration.Name, manager);
                        Meta[Index].Next = bucket.Position;
                        bucket.Position = Index;

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
    }
}
