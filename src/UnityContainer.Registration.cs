using System;
using System.Collections.Generic;
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
                    if (metadata.Entries[i].HashKey != key) continue;
                    return true;
                }

                return false;
            }

            return false;
        }

        #endregion


        #region Getting Registration During Resolution

        internal IRegistration GetRegistration(Type type, string? name)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
            return info.IsGenericType ? GetGenericRegistration(type, name, info) : GetSimpleRegistration(type, name);
#else
            return type.IsGenericType ? GetGenericRegistration(type, name) : GetSimpleRegistration(type, name);
#endif
        }

        private IRegistration GetSimpleRegistration(Type type, string? name)
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
                    if (!(candidate.Policies is IRegistration))
                        candidate.Policies = container.CreateRegistration(type, name, candidate.Policies);

                    return (IRegistration)candidate.Policies;
                }
            }

            Debug.Assert(null != _root);

            return _root.GetOrAdd(ref key, type, name, null);
        }


#if NETSTANDARD1_0 || NETCOREAPP1_0
        private IRegistration GetGenericRegistration(Type type, string? name, TypeInfo info)
#else
        private IRegistration GetGenericRegistration(Type type, string? name)
#endif
        {
            bool initGenerics = true;
            Type? generic = null;
            int targetBucket;
            var keyExact = new HashKey(type, name);
            var keyGeneric = new HashKey();
            var keyDefault = new HashKey();

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
                    if (!(candidate.Policies is IRegistration))
                        candidate.Policies = container.CreateRegistration(type, name, candidate.Policies);

                    return (IRegistration)candidate.Policies;
                }

                // Generic registrations
                if (initGenerics)
                {
                    initGenerics = false;

#if NETSTANDARD1_0 || NETCOREAPP1_0
                    generic = info.GetGenericTypeDefinition();
#else
                    generic = type.GetGenericTypeDefinition();
#endif
                    keyGeneric = new HashKey(generic, name);
                    keyDefault = new HashKey(generic);
                }

                // Check for factory with same name
                targetBucket = keyGeneric.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyGeneric)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(ref keyExact, type, name, candidate.Policies);
                }

                // Check for default factory
                targetBucket = keyDefault.HashCode % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != keyDefault)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(ref keyExact, type, name, candidate.Policies);
                }
            }

            Debug.Assert(null != _root);

            return _root.GetOrAdd(ref keyExact, type, name, null);
        }

        #endregion


        #region Registration manipulation

        private ExplicitRegistration? InitAndAdd(Type type, string? name, ExplicitRegistration registration)
        {
            lock (_syncLock)
            {
                if (null == _registry) _registry = new Registry();
                if (null == _metadata)
                {
                    _metadata = new Metadata();

                    Register = AddOrReplace;
                }
            }

            return Register(type, name, registration);
        }

        private ExplicitRegistration? AddOrReplace(Type type, string? name, ExplicitRegistration registration)
        {
            var collisions = 0;
            var key = new HashKey(type, name);
            var metaKey = new HashKey(type);

            Debug.Assert(null != _registry);
            Debug.Assert(null != _metadata);

            registration.AddRef();

            // Registry
            lock (_syncLock)
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
                    var existing = candidate.Policies as ExplicitRegistration;

                    if (null == existing)
                    {
                        candidate.IsExplicit = true;
                        registration.Add(candidate.Policies);
                    }
                    candidate.Policies = registration;

                    // Replaced registration
                    return existing;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Type = type;
                entry.IsExplicit = true;
                entry.Policies = registration;
                int position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;

                collisions = 0;

                // Metadata
                targetBucket = metaKey.HashCode % _metadata.Buckets.Length;

                for (var i = _metadata.Buckets[targetBucket]; i >= 0; i = _metadata.Entries[i].Next)
                {
                    ref var candidate = ref _metadata.Entries[i];
                    if (candidate.HashKey != metaKey || candidate.Type != type)
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
                    _metadata = new Metadata(_metadata);
                    targetBucket = metaKey.HashCode % _metadata.Buckets.Length;
                }

                // Create new metadata entry
                ref var metadata = ref _metadata.Entries[_metadata.Count];
                metadata.Next = _metadata.Buckets[targetBucket];
                metadata.HashKey = metaKey;
                metadata.Type = type;
                metadata.Value = new int[] { 2, position };
                _metadata.Buckets[targetBucket] = _metadata.Count++;
            }

            // Nothing to replace
            return null;
        }

        private IEnumerable<IPolicySet> AddOrReplace(IEnumerable<Type> type, string? name, ExplicitRegistration registration)
        {
            throw new NotImplementedException();
        }

        private IRegistration GetOrAdd(ref HashKey key, Type type, string? name, IPolicySet? factory = null)
        {
            Debug.Assert(null != _registry);

            lock (_syncLock)
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

                    if (!(candidate.Policies is IRegistration))
                        candidate.Policies = CreateRegistration(type, name, candidate.Policies);

                    return (IRegistration)candidate.Policies;
                }

                // Expand if required
                if (_registry.RequireToGrow || CollisionsCutPoint < collisions)
                {
                    _registry = new Registry(_registry);
                    targetBucket = key.HashCode % _registry.Buckets.Length;
                }

                // Add registration
                var registration = CreateRegistration(type, name, factory);
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.Key = key;
                entry.Type = type;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Policies = registration;
                _registry.Buckets[targetBucket] = _registry.Count++;

                return (IRegistration)entry.Policies;
            }
        }

        #endregion


        #region Creating Implicit Registration

        private IRegistration CreateRegistration(Type type, string? name, IPolicySet? set)
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
