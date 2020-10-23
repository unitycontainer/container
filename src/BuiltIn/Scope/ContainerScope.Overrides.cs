using System;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Add

        /// <inheritdoc />
        public override void Add(Type type, RegistrationManager manager, bool reserved = false)
        {
            var hash = type.GetHashCode();
            ref var bucket = ref Meta[((uint)hash) % Meta.Length];
            var position = bucket.Position;
            int pointer = reserved ? -1 : 0;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Internal.Contract.Type, type) &&
                    candidate.Internal.Contract.Name == null)
                {
                    if (null == candidate.Internal.Manager)
                    {
                        candidate.Internal.Manager = manager;
                        Revision += 1;
                        return;
                    }

                    // move pointer no next into default
                    pointer = candidate.Next;
                    candidate.Next = 0;

                    goto RegisterNew;
                }

                position = Meta[position].Next;
            }

            // Add new registration
            RegisterNew: Index++;
            Data[Index] = new Entry(hash, type, manager, pointer);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            Revision += 1;
        }

        /// <inheritdoc />
        public override RegistrationManager? Add(in Contract contract, RegistrationManager manager)
        {
            ref var bucket = ref Meta[((uint)contract.HashCode) % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                    candidate.Contract.Name == contract.Name)
                {
                    var replacement = candidate.Manager;
                    candidate.Manager = manager;
                    Revision += 1;

                    return replacement;
                }

                position = Meta[position].Next;
            }

            ref var @default = ref Data[GetDefault(contract.Type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(in contract, manager, @default.Next);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            @default.Next = Index;
            Revision += 1;
            return null;
        }

        /// <inheritdoc />
        public override RegistrationManager? Add(Type type, string name, RegistrationManager manager)
        {
            var hash = Contract.GetHashCode(type, name);
            ref var bucket = ref Meta[((uint)hash) % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (ReferenceEquals(candidate.Contract.Type, type) &&
                    candidate.Contract.Name == name)
                {
                    var replacement = candidate.Manager;
                    candidate.Manager = manager;
                    Revision += 1;

                    return replacement;
                }

                position = Meta[position].Next;
            }

            ref var @default = ref Data[GetDefault(type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(hash, type, name, manager, @default.Next);
            Meta[Index].Next = bucket.Position;
            bucket.Position = Index;
            @default.Next = Index;
            Revision += 1;
            return null;
        }

        /// <inheritdoc />
        public override void Add(in ReadOnlySpan<RegistrationDescriptor> span)
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

        #endregion


        #region Get

        /// <inheritdoc />
        public override RegistrationManager? Get(in Contract contract)
        {
            var meta = Meta;
            var target = ((uint)contract.HashCode) % meta.Length;
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

        public override RegistrationManager? Get(in Contract contract, RegistrationCategory cutoff)
        {
            var meta = Meta;
            var target = ((uint)contract.HashCode) % meta.Length;
            var position = meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;

                if (null != candidate.Manager && cutoff < candidate.Manager.Category && 
                    ReferenceEquals(candidate.Contract.Type, contract.Type) && candidate.Contract.Name == contract.Name)
                    return candidate.Manager;

                position = meta[position].Next;
            }

            return null;
        }

        /// <inheritdoc />
        public override RegistrationManager? Get(in Contract contract, in Contract generic)
        {
            var meta  = Meta;
            var position = meta[((uint)generic.HashCode) % meta.Length].Position;

            // Search for generic factory

            while (position > 0)
            {
                ref var factory = ref Data[position];
                if (ReferenceEquals(factory.Internal.Contract.Type, generic.Type) &&
                    factory.Internal.Contract.Name == generic.Name)
                {
                    // Found generic factory

                    lock (SyncRoot)
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
                            target = ((uint)contract.HashCode) % Meta.Length;
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

        public override RegistrationManager GetCache(in Contract contract)
        {
            var meta = Meta;
            var position = meta[((uint)contract.HashCode) % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;

                if (ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                    candidate.Contract.Name == contract.Name)
                {
                    if (null == candidate.Manager)
                        candidate.Manager = new InternalRegistrationManager();

                    return candidate.Manager;
                }

                position = meta[position].Next;
            }

            var version = Revision;

            lock (SyncRoot)
            {
                var target = ((uint)contract.HashCode) % Meta.Length;

                // Check if contract is created already
                if (version != Revision)
                { 
                    position = Meta[target].Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position].Internal;
                        if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                            candidate.Contract.Name == contract.Name)
                        {
                            // Found existing
                            if (null == candidate.Manager)
                                candidate.Manager = new InternalRegistrationManager();

                            return candidate.Manager;
                        }

                        position = Meta[position].Next;
                    }
                }

                // Nothing is found, add new and expand if required
                if (Data.Length <= ++Index)
                {
                    Expand(Index);
                    target = ((uint)contract.HashCode) % Meta.Length;
                }

                // Clone manager
                var manager = new InternalRegistrationManager();

                ref var bucket = ref Meta[target];
                Data[Index] = new Entry(in contract, manager, 0);
                Meta[Index].Next = bucket.Position;
                bucket.Position = Index;

                return manager;
            }
        }

        #endregion


        #region Contains

        public override bool Contains(Type type)
        {
            var hash = (uint)type.GetHashCode();
            var scope = this;

            do
            {
                var meta = scope.Meta;
                var target = hash % meta.Length;
                var position = meta[target].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope.Data[position].Internal;
                    if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, type) &&
                        null == candidate.Contract.Name)
                        return true;

                    position = meta[position].Next;
                }
            }
            while (null != (scope = Unsafe.As<ContainerScope>(scope.Next)));

            return false;
        }

        /// <inheritdoc />
        public override bool Contains(in Contract contract)
        {
            var scope = this;
            do
            { 
                var meta = scope.Meta;
                var bucket = ((uint)contract.HashCode) % meta.Length;
                var position = meta[bucket].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope.Data[position].Internal;
                    if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                        candidate.Contract.Name == contract.Name)
                        return true;

                    position = meta[position].Next;
                }
            }
            while (null != (scope = Unsafe.As<ContainerScope>(scope.Next)));

            return false;
        }

        #endregion


        #region Child Scope

        /// <inheritdoc />
        public override Scope CreateChildScope(int capacity) => new ContainerScope(this, capacity);
        #endregion
    }
}
