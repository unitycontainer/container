using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        private const int CollisionsCutPoint = 5;
        internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));

        #endregion


        #region Fields

        private readonly object _syncRegistry = new object();
        private readonly object _syncMetadata = new object();
        private Registry<NamedType, InternalRegistration> _registry;
        private Registry<Type, Metadata>                  _metadata;

        #endregion


        #region Defaults

        internal InternalRegistration Defaults => _root._registry.Entries[0].Value;

        #endregion


        #region Registry Manipulation

        private InternalRegistration InitAndAdd(Type type, string name, InternalRegistration registration)
        {
            lock (_syncRegistry)
            {
                if (Register == InitAndAdd)
                {
                    _registry = new Registry<NamedType, InternalRegistration>();
                    _metadata = new Registry<Type, Metadata>();

                    Register = AddOrReplace;
                }
            }

            return Register(type, name, registration);
        }

        private InternalRegistration AddOrReplace(Type type, string name, InternalRegistration registration)
        {
            int position = -1;
            var collisions = 0;

            // Registry
            lock (_syncRegistry)
            {
                var hashCode = NamedType.GetHashCode(type, name) & HashMask;
                var targetBucket = hashCode % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key.Type != type)
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

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Key.Type = type;
                entry.Key.Name = name;
                entry.Value = registration;
                entry.Value.AddRef();
                position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
            }

            collisions = 0;

            // Metadata
            lock (_syncMetadata)
            {
                var hashCode = type?.GetHashCode() ?? 0 & HashMask;
                var targetBucket = hashCode % _metadata.Buckets.Length;

                for (var i = _metadata.Buckets[targetBucket]; i >= 0; i = _metadata.Entries[i].Next)
                {
                    ref var candidate = ref _metadata.Entries[i];
                    if (candidate.Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    // Expand if required
                    if (candidate.Value.Data.Length == candidate.Value.Count)
                    {
                        var source = candidate.Value.Data;
                        candidate.Value.Data = new int[candidate.Value.Data.Length * 2];
                        Array.Copy(source, candidate.Value.Data, candidate.Value.Count);
                    }

                    // Add to existing
                    candidate.Value.Data[candidate.Value.Count++] = position;

                    return null;
                }

                // Expand if required
                if (_metadata.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _metadata = new Registry<Type, Metadata>(_metadata);
                    targetBucket = hashCode % _metadata.Buckets.Length;
                }

                // Create new metadata entry
                ref var entry = ref _metadata.Entries[_metadata.Count];
                entry.Next = _metadata.Buckets[targetBucket];
                entry.HashCode = hashCode;
                entry.Key = type;
                entry.Value.Count = 1;
                entry.Value.Data = new int[] { position, -1 };
                _metadata.Buckets[targetBucket] = _metadata.Count++;
            }

            return null;
        }

        private InternalRegistration GetOrAdd(int hashCode, Type type, string name, InternalRegistration factory)
        {

            lock (_syncRegistry)
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


        #region Nested Types

        internal struct Metadata
        {
            public int Count;
            public int[] Data;
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
            var hashCode = NamedType.GetHashCode(type, name) & UnityContainer.HashMask;
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
        public static InternalRegistration Get(this Registry<NamedType, InternalRegistration> registry, Type type)
        {
            var hashCode = type?.GetHashCode() ?? 0 & UnityContainer.HashMask;
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
            var hashCode = key.GetHashCode() & UnityContainer.HashMask;
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


        #region Set

        internal static void Set(this Registry<NamedType, InternalRegistration> registry, InternalRegistration registration)
        {
            var targetBucket = 0 % registry.Buckets.Length;

            ref var entry = ref registry.Entries[0];
            entry.Next = registry.Buckets[targetBucket];
            entry.Value = registration;
            registry.Buckets[targetBucket] = 0;

            if (0 == registry.Count) registry.Count++;
        }

        internal static void Set(this Registry<NamedType, InternalRegistration> registry, Type type, InternalRegistration registration)
        {
            var hashCode = type.GetHashCode() & UnityContainer.HashMask;
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.Key.Type != type) continue;
                candidate.Value = registration;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Value = registration;
            registry.Buckets[targetBucket] = registry.Count++;
        }

        internal static void Set(this Registry<NamedType, InternalRegistration> registry, Type type, string name, InternalRegistration registration)
        {
            var hashCode = NamedType.GetHashCode(type, name) & UnityContainer.HashMask;
            var targetBucket = hashCode % registry.Buckets.Length;

            for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
            {
                ref var candidate = ref registry.Entries[i];
                if (candidate.Key.Type != type) continue;
                candidate.Value = registration;
                return;
            }

            ref var entry = ref registry.Entries[registry.Count];
            entry.HashCode = hashCode;
            entry.Next = registry.Buckets[targetBucket];
            entry.Key.Type = type;
            entry.Key.Name = name;
            entry.Value = registration;
            registry.Buckets[targetBucket] = registry.Count++;
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


        #region Metadata

        internal static int GetEntries<TElement>(this Registry<Type, UnityContainer.Metadata> metadata, int hashCode, out int[] data)
        {
            var targetBucket = hashCode % metadata.Buckets.Length;

            for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
            {
                if (metadata.Entries[i].Key != typeof(TElement)) continue;

                data = metadata.Entries[i].Value.Data;
                return metadata.Entries[i].Value.Count;
            }

            data = null;
            return 0;
        }

        internal static int GetEntries(this Registry<Type, UnityContainer.Metadata> metadata, int hashCode, Type type, out int[] data)
        {
            var targetBucket = hashCode % metadata.Buckets.Length;

            for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
            {
                if (metadata.Entries[i].Key != type) continue;

                data = metadata.Entries[i].Value.Data;
                return metadata.Entries[i].Value.Count;
            }

            data = null;
            return 0;
        }

        internal static IEnumerable<int> GetEntries(this Registry<Type, UnityContainer.Metadata> metadata, Type type)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & UnityContainer.HashMask;
            var targetBucket = hashCode % metadata.Buckets.Length;
            for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
            {
                if (metadata.Entries[i].Key != type) continue;

                var count = metadata.Entries[i].Value.Count;
                var data = metadata.Entries[i].Value.Data;

                for (var index = 0; index < count; index++)
                    yield return data[index];

                yield break;
            }
        }

        #endregion
    }

    #endregion
}
