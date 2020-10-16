using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
                ref var candidate = ref Data[position].Registration;
                if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                    candidate._contract.Name == contract.Name)
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

                        AddAnonymous(in descriptor);
                    }
                    else
                    {
                        // Expand registry if required
                        var required = START_INDEX + Index + descriptor.RegisterAs.Length * 2;
                        if (required >= Data.Length) Expand(required);

                        AddWithName(in descriptor);
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
                Set(new Contract(type), manager);
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
                ref var candidate = ref Data[position].Registration;
                if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                    candidate._contract.Name == contract.Name)
                    return candidate._manager;

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
                ref var factory = ref Data[position].Registration;
                if (ReferenceEquals(factory._contract.Type, generic.Type) &&
                    factory._contract.Name == generic.Name)
                {
                    // Found generic factory

                    lock (Sync)
                    {
                        // Check if contract is created already

                        var target = (uint)contract.HashCode % Meta.Length;
                        position = Meta[target].Position;

                        while (position > 0)
                        {
                            ref var candidate = ref Data[position].Registration;
                            if (ReferenceEquals(candidate._contract.Type, contract.Type) &&
                                candidate._contract.Name == contract.Name)
                            {
                                // Found existing
                                return candidate._manager;
                            }

                            position = Meta[position].Next;
                        }

                        // Nothing is found, add new

                        Index += 1;

                        // Expand if required

                        if (Data.Length <= Index)
                        {
                            Expand();
                            target = (uint)contract.HashCode % Meta.Length;
                        }

                        // Clone manager
                        var manager = factory.LifetimeManager.Clone();

                        ref var bucket = ref Meta[target];
                        Data[Index] = new Entry(contract.HashCode, contract.Type, factory.Name, manager);
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
