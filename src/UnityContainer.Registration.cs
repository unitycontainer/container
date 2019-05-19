using System;
using System.Diagnostics;
using System.Reflection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Check if registered

        internal bool IsRegistered(Type type)
        {
            var key = new HashKey(type);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                var metadata = container._metadata; ;
                var targetBucket = key.HashCode % metadata.Buckets.Length;

                for (var i = metadata.Buckets[targetBucket]; i >= 0; i = metadata.Entries[i].Next)
                {
                    if (metadata.Entries[i].Key != key) continue;
                    return true;
                }

                return false;
            }

            return false;
        }

        #endregion


        #region Getting Registration During Resolution

        internal ImplicitRegistration GetRegistration(Type type, string? name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
            return info.IsGenericType ? GetGenericRegistration(type, name, info) : GetSimpleRegistration(type, name);
#else
            return type.IsGenericType ? GetGenericRegistration(type, name) : GetSimpleRegistration(type, name);
#endif
        }

        private ImplicitRegistration GetSimpleRegistration(Type type, string? name)
        {
            var key = new HashKey(type, name);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry;

                // Check for exact match
                for (var i = registry.Buckets[key.HashCode % registry.Buckets.Length]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    // Found a registration
                    if (!(candidate.Value is ImplicitRegistration))
                        candidate.Value = container.CreateRegistration(type, name, candidate.Value);

                    return (ImplicitRegistration)candidate.Value;
                }
            }

            Debug.Assert(null != _root);

            return _root.GetOrAdd(ref key, type, name, null);
        }


#if NETSTANDARD1_0 || NETCOREAPP1_0
        private ImplicitRegistration GetGenericRegistration(Type type, string? name, TypeInfo info)
#else
        private ImplicitRegistration GetGenericRegistration(Type type, string? name)
#endif
        {
            bool initialize = true;
            Type? generic = null;
            int targetBucket;
            int hashGeneric = 0, hashDefault = 0;
            int typeGeneric = 0, typeDefault = 0;
            int nameGeneric = 0, nameDefault = 0;
            var keyExact = new HashKey(type, name);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                Debug.Assert(null != container._registry);
                var registry = container._registry;

                // Check for exact match
                targetBucket = keyExact.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyExact) continue;

                    // Found a registration
                    if (!(candidate.Value is ImplicitRegistration))
                        candidate.Value = container.CreateRegistration(type, name, candidate.Value);

                    return (ImplicitRegistration)candidate.Value;
                }

                // Generic registrations
                if (initialize)
                {
                    initialize = false;

#if NETSTANDARD1_0 || NETCOREAPP1_0
                    generic = info.GetGenericTypeDefinition();
#else
                    generic = type.GetGenericTypeDefinition();
#endif
                    var keyGeneric = new HashKey(generic, name);
                    hashGeneric = keyGeneric.HashCode;
                    typeGeneric = keyGeneric.HashType;
                    nameGeneric = keyGeneric.HashName;

                    if (null != generic)
                    {
                        var keyDefault = new HashKey(generic);
                        hashDefault = keyDefault.HashCode;
                        typeDefault = keyDefault.HashType;
                        nameDefault = keyDefault.HashName;
                    }
                }

                // Check for factory with same name
                targetBucket = hashGeneric % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key.HashType != typeGeneric || 
                        candidate.Key.HashName != nameGeneric)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(ref keyExact, type, name, candidate.Value);
                }

                // Check for default factory
                targetBucket = hashDefault % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key.HashType != typeDefault || 
                        candidate.Key.HashName != nameDefault)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(ref keyExact, type, name, candidate.Value);
                }
            }

            Debug.Assert(null != _root);

            return _root.GetOrAdd(ref keyExact, type, name, null);
        }

        #endregion


        #region Registration manipulation

        private ImplicitRegistration? InitAndAdd(Type type, string? name, ImplicitRegistration registration)
        {
            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<IPolicySet>();
                if (null == _metadata)
                {
                    _metadata = new Registry<int[]>();

                    Register = AddOrReplace;
                }
            }

            return Register(type, name, registration);
        }

        private ImplicitRegistration? AddOrReplace(Type type, string? name, ImplicitRegistration registration)
        {
            int position = -1;
            var collisions = 0;
            var key = new HashKey(type, name);
            var meta = new HashKey(type);

            Debug.Assert(null != _registry);
            Debug.Assert(null != _metadata);

            registration.AddRef();

            // Registry
            lock (_syncRegistry)
            {
                var targetBucket = key.HashCode % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    // Swap the registration
                    var existing = candidate.Value as ImplicitRegistration;

                    if (null == existing) registration.Add(candidate.Value);
                    candidate.Value = registration;

                    // Replaced registration
                    return existing;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry<IPolicySet>(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Type = type;
                entry.Value = registration;
                position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;

                collisions = 0;

                // Metadata
                targetBucket = meta.HashCode % _metadata.Buckets.Length;

                for (var i = _metadata.Buckets[targetBucket]; i >= 0; i = _metadata.Entries[i].Next)
                {
                    ref var candidate = ref _metadata.Entries[i];
                    if (candidate.Key != meta || candidate.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    // Expand if required
                    if (candidate.Value.Length == candidate.Value[0])
                    {
                        var source = candidate.Value;
                        candidate.Value = new int[source.Length * 2];
                        Array.Copy(source, candidate.Value, source[0]);
                    }

                    // Add to existing
                    candidate.Value[candidate.Value[0]++] = position;

                    // Nothing to replace
                    return null;
                }

                // Expand if required
                if (_metadata.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _metadata = new Registry<int[]>(_metadata);
                    targetBucket = meta.HashCode % _metadata.Buckets.Length;
                }

                // Create new metadata entry
                ref var metadata = ref _metadata.Entries[_metadata.Count];
                metadata.Next = _metadata.Buckets[targetBucket];
                metadata.Key = meta;
                metadata.Type = type;
                metadata.Value = new int[] { 2, position };
                _metadata.Buckets[targetBucket] = _metadata.Count++;
            }

            // Nothing to replace
            return null;
        }

        private ImplicitRegistration GetOrAdd(Type type, string? name, IPolicySet? factory = null)
        {
            var key = new HashKey(type, name);
            return GetOrAdd(ref key, type, name, factory);
        }

        private ImplicitRegistration GetOrAdd(ref HashKey key, Type type, string? name, IPolicySet? factory)
        {
            Debug.Assert(null != _registry);

            lock (_syncRegistry)
            {
                var collisions = 0;
                var targetBucket = key.HashCode % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.Key != key)
                    {
                        collisions++;
                        continue;
                    }

                    if (!(candidate.Value is ImplicitRegistration))
                        candidate.Value = CreateRegistration(type, name, candidate.Value);

                    return (ImplicitRegistration)candidate.Value;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry<IPolicySet>(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Add registration
                var registration = CreateRegistration(type, name, factory);
                registration.AddRef();
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Type = type;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = registration;
                _registry.Buckets[targetBucket] = _registry.Count++;

                return (ImplicitRegistration)entry.Value;
            }
        }

        #endregion


        #region Creating Implicit Registration

        private ImplicitRegistration CreateRegistration(Type type, string? name, IPolicySet? set)
        {
            var registration = set is ImplicitRegistration factory 
                             ? new ImplicitRegistration(this, name, factory)
                             : new ImplicitRegistration(this, name, set);

            registration.Processors = Context.TypePipelineCache;

            if (registration.LifetimeManager is IDisposable) LifetimeContainer.Add(registration.LifetimeManager);

            return registration;
        }

        #endregion
    }
}
