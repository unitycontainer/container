using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Unity.Storage
{
    public partial class HashScope
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


        /// <inheritdoc />
        public override RegistrationManager GetCache(in Contract contract, Func<RegistrationManager> factory)
        {
            var meta = Meta;
            var position = meta[((uint)contract.HashCode) % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;

                if (ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                    candidate.Contract.Name == contract.Name)
                {
                    // Found existing
                    Debug.Assert(null != candidate.Manager);
                    return candidate.Manager!;
                }

                position = meta[position].Location;
            }

            var version = Revision;

            lock (SyncRoot)
            {
                var target = ((uint)contract.HashCode) % Meta.Length;

                // Check if created already
                if (version != Revision)
                { 
                    position = Meta[target].Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position].Internal;
                        if (ReferenceEquals(candidate.Contract.Type, contract.Type) &&
                            candidate.Contract.Name == contract.Name)
                        {
                            // Found existing
                            return candidate.Manager!;
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
                var registration = factory();

                Data[Index] = new Entry(in contract, registration, 0);
                Meta[Index].Location = bucket.Position;
                bucket.Position = Index;

                return registration;
            }
        }

        #endregion


        #region Contains

        /// <inheritdoc />
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
            while (null != (scope = Unsafe.As<HashScope>(scope.Next)));

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
            while (null != (scope = Unsafe.As<HashScope>(scope.Next)));

            return false;
        }

        #endregion


        #region Child Scope

        /// <inheritdoc />
        public override Scope CreateChildScope(int capacity) => new HashScope(this, capacity);

        #endregion
    }
}
