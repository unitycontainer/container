using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        #region Adding Registrations

        /// <inheritdoc />
        public override void BuiltIn(Type type, RegistrationManager manager)
        {
            var hash = type.GetHashCode();
            ref var bucket = ref Meta[((uint)hash) % Meta.Length];

            Index++;
            Data[Index] = new Entry(hash, type, manager, -1);
            Meta[Index].Location = bucket.Position;
            bucket.Position = Index;
        }

        /// <inheritdoc />
        public override RegistrationManager? Register(Type type, string? name, RegistrationManager manager)
        {
            lock (SyncRoot)
            {
                var required = Index + 3;
                if (Data.Length <= required) Expand(required);

                return name is null ? Add(type, manager)
                                    : Add(type, name, name.GetHashCode(), manager);
            }
        }

        /// <inheritdoc />
        public override void Register(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            lock (SyncRoot)
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

        /// <inheritdoc />
        public override RegistrationManager? GetBoundGeneric(in Contract contract, in Contract generic)
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
                    Interlocked.CompareExchange(ref candidate.Manager, 
                        manager ?? new ContainerLifetimeManager(RegistrationCategory.Cache), null);

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
                                candidate.Manager = manager ?? new ContainerLifetimeManager(RegistrationCategory.Cache);

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
                var registration = manager ?? new ContainerLifetimeManager(RegistrationCategory.Cache);

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
