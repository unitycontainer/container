using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        private Registry<NamedType, InternalRegistration> _registry;
        private Metadata _metadata;

        #endregion


        #region Registrations Manipulation

        private InternalRegistration InitAndAdd(ref NamedType key, InternalRegistration registration)
        {
            lock (_syncRoot)
            {
                if (Register == InitAndAdd)
                {
                    _registry = new Registry<NamedType, InternalRegistration>();
                    _metadata = new Metadata();

                    Register = AddOrReplace;
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
                    if (candidate.Key.Type != key.Type)
                    {
                        collisions++;
                        continue;
                    }

                    // Replace registration
                    var existing = candidate.Value;

                    candidate.Value = registration;
                    candidate.Value.AddRef();

                    return existing;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry<NamedType, InternalRegistration>(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Key = key;
                entry.Value = registration;
                entry.Value.AddRef();

                var position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
                _metadata.Add(key.Type, position);

                return null;
            }
        }

        private InternalRegistration GetOrAdd(int hashCode, Type type, string name, InternalRegistration factory)
        {

            lock (_syncRoot)
            {
                var collisions = 0;
                var targetBucket = hashCode % _registry.Buckets.Length;

                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    return candidate.Value;
                }

                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry<NamedType, InternalRegistration>(_registry);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Key.Type = type;
                entry.Key.Name = name;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = CreateRegistration(type, name, factory);
                entry.Value.AddRef();
                _registry.Buckets[targetBucket] = _registry.Count++;

                return entry.Value;
            }
        }

        #endregion
    }

    #region Registry Query Methods

    internal static class RegistryExtensions
    {
        #region Get

        #if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public static InternalRegistration Get(this Registry<NamedType, InternalRegistration> registry, Type type, string name)
        {
            var hashCode = NamedType.GetHashCode(type, name);
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return entry.Value;
            }

            return null;
        }

        #if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public static InternalRegistration Get(this Registry<NamedType, InternalRegistration> registry, ref NamedType key)
        {
            var hashCode = key.GetHashCode();
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != key.Type) continue;
                return entry.Value;
            }

            return null;
        }

        #if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public static InternalRegistration Get(this Registry<NamedType, InternalRegistration> registry, int hashCode, Type type)
        {
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return entry.Value;
            }

            return null;
        }

        #endregion


        #region Contains

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Contains(this Registry<NamedType, InternalRegistration> registry, Type type, string name)
        {
            var hashCode = NamedType.GetHashCode(type, name);
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return true;
            }

            return false;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Contains(this Registry<NamedType, InternalRegistration> registry, ref NamedType key)
        {
            var hashCode = key.GetHashCode();
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != key.Type) continue;
                return true;
            }

            return false;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Contains(this Registry<NamedType, InternalRegistration> registry, int hashCode, Type type)
        {
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Key.Type != type) continue;
                return true;
            }

            return false;
        }

        #endregion
    }

    #endregion
}
