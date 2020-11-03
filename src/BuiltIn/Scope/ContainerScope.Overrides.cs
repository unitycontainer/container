using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Setup

        public override void Setup(Defaults defaults)
        {
            defaults.Set<Func<Scope, Type[], Metadata[]>>(typeof(Array),       ArrayToMeta);
            defaults.Set<Func<Scope, Type[], Metadata[]>>(typeof(IEnumerable), EnumToMeta);
        }

        #endregion


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
                    if (candidate.Internal.Manager is null)
                    {
                        candidate.Internal.Manager = manager;
                        Revision += 1;
                        return;
                    }

                    // move pointer no next into default
                    pointer = candidate.Next;
                    candidate.Next = 0;

                    goto register;
                }

                position = Meta[position].Location;
            }

            // Add new registration
            register: Index++;
            Data[Index] = new Entry(hash, type, manager, pointer);
            Meta[Index].Location = bucket.Position;
            bucket.Position = Index;
            if (!reserved) Revision += 1;
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

                position = Meta[position].Location;
            }

            ref var @default = ref Data[GetDefault(contract.Type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(in contract, manager, @default.Next);
            Meta[Index].Location = bucket.Position;
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

                position = Meta[position].Location;
            }

            ref var @default = ref Data[GetDefault(type)];

            // Add new registration
            Index++;
            Data[Index] = new Entry(hash, type, name, manager, @default.Next);
            Meta[Index].Location = bucket.Position;
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

                if (descriptor.Name is null)
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
            var position = meta[((uint)contract.HashCode) % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (ReferenceEquals(candidate.Contract.Type, contract.Type) && candidate.Contract.Name == contract.Name)
                    return candidate.Manager;

                position = meta[position].Location;
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

                position = meta[position].Location;
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

                            position = Meta[position].Location;
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
                        Meta[Index].Location = bucket.Position;
                        bucket.Position = Index;

                        return manager;
                    }
                }

                position = meta[position].Location;
            }

            return null;
        }

        public override RegistrationManager GetCache(in Contract contract, RegistrationManager? manager = null)
        {
            var meta = Meta;
            var position = meta[((uint)contract.HashCode) % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;

                if (ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                    candidate.Contract.Name == contract.Name)
                {
                    if (candidate.Manager is null)
                        candidate.Manager = manager ?? new InternalRegistrationManager();

                    return candidate.Manager;
                }

                position = meta[position].Location;
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
                            if (candidate.Manager is null)
                                candidate.Manager = manager ?? new InternalRegistrationManager();

                            return candidate.Manager;
                        }

                        position = Meta[position].Location;
                    }
                }

                // Nothing is found, add new and expand if required
                if (Data.Length <= ++Index)
                {
                    Expand(Index);
                    target = ((uint)contract.HashCode) % Meta.Length;
                }

                ref var bucket = ref Meta[target];
                var registration = manager ?? new InternalRegistrationManager();

                Data[Index] = new Entry(in contract, registration, 0);
                Meta[Index].Location = bucket.Position;
                bucket.Position = Index;

                return registration;
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
                    if (ReferenceEquals(candidate.Contract.Type, type) && null == candidate.Contract.Name)
                        return true;

                    position = meta[position].Location;
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

                    position = meta[position].Location;
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
