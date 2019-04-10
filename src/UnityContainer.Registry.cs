using System;
using System.Diagnostics;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int CollisionsCutPoint = 5;

        #endregion


        #region Fields

        private Registry _registry;
        private Metadata _metadata;

        #endregion


        #region Registrations Manipulation

        private InternalRegistration InitAndAdd(ref NamedType key, InternalRegistration registration)
        {
            lock (_syncRoot)
            {
                if (Register == InitAndAdd)
                {
                    _registry = new Registry();
                    _metadata = new Metadata();

                    Register = AddOrReplace;
                    GetRegistration = GetOrAdd;
                }
            }

            return Register(ref key, registration);
        }

        private InternalRegistration AddOrReplace(ref NamedType key, InternalRegistration registration)
        {
            var hashCode = key.GetHashCode() & 0x7FFFFFFF;
            var targetBucket = hashCode % _registry.Buckets.Length;
            var collisions = 0;

            lock (_syncRoot)
            {
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key.Type != key.Type)
                    {
                        collisions++;
                        continue;
                    }

                    // Replace registration
                    var existing = candidate.Registration;

                    candidate.Registration = registration;
                    candidate.Registration.AddRef();

                    return existing;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Key = key;
                entry.Registration = registration;
                entry.Registration.AddRef();

                var position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
                _metadata.Add(key.Type, position);

                return null;
            }
        }

        private InternalRegistration GetOrAdd(ref NamedType key)
        {
            var hashCode = key.GetHashCode() & 0x7FFFFFFF;
            var targetBucket = hashCode % _registry.Buckets.Length;
            var registry = _registry;

            // Check for existing without squaring the lock first
            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.HashCode != hashCode ||
                    candidate.Key.Type != key.Type)
                {
                    continue;
                }

                // Found a registration
                return candidate.Registration;
            }

            // Nothing found so get the lock and add a new registration

            // Do the double-check lock to verify it was not yet added 
            lock (_syncRoot)
            {
                var collisions = 0;

                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key.Type != key.Type)
                    {
                        collisions++;
                        continue;
                    }

                    return candidate.Registration;
                }

                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Key = key;
                entry.Registration = CreateRegistration(ref key);
                entry.Registration.AddRef();
                _registry.Buckets[targetBucket] = _registry.Count++;

                return entry.Registration;
            }
        }

        #endregion


        #region Verification

        internal bool RegistrationExists(ref NamedType key)
        {
            var hashCode = key.GetHashCode() & 0x7FFFFFFF;

            Type type = null;
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = key.Type.GetTypeInfo();
            if (info.IsGenericType) type = info.GetGenericTypeDefinition();
#else
            if (key.Type.IsGenericType) type = key.Type.GetGenericTypeDefinition();
#endif
            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                var registry = container._registry;

                if (null == registry) continue;

                // Look for exact match
                var targetBucket = hashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key.Type != key.Type)
                    {
                        continue;
                    }

                    return true;    // Found a registration
                }

                if (null == type) continue;

                var metadate = container._metadata;

                // Look for generic factory
                targetBucket = hashCode % metadate.Buckets.Length;
                for (var i = metadate.Buckets[targetBucket]; i >= 0; i = metadate.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key.Type != key.Type)
                    {
                        continue;
                    }

                    return true;    // Found a registration
                }


            }

            return false;
        }


        #endregion
    }
}
