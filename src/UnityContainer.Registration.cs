using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Utility;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Check if registered

        internal bool IsRegistered(Type type)
        {
            var hashCode = type?.GetHashCode() ?? 0;

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                if (container._metadata.Contains(hashCode, type)) return true;
            }

            return false;
        }

        internal bool IsRegistered(ref BuilderContext context)
        {
            Type? generic = null;
            int targetBucket, hashGeneric = -1;
            int hashExact = NamedType.GetHashCode(context.Type, context.Name);

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = context.Type.GetTypeInfo();
            if (info.IsGenericType)
            {
                generic = info.GetGenericTypeDefinition();
                hashGeneric = NamedType.GetHashCode(generic, context.Name);
            }
#else
            if (context.Type.IsGenericType)
            {
                generic = context.Type.GetGenericTypeDefinition();
                hashGeneric = NamedType.GetHashCode(generic, context.Name);
            }
#endif

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata || null == container._registry) continue;

                var registry = container._registry;

                // Check for exact match
                targetBucket = (hashExact & HashMask) % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashExact || candidate.Type != context.Type) continue;

                    // Found a registration
                    return true;
                }

                // Skip to parent if not generic
                if (null == generic) continue;

                // Check for factory with same name
                targetBucket = (hashGeneric & HashMask) % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashGeneric || candidate.Type != generic) continue;

                    // Found a factory
                    return true;
                }
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
            int hashCode = NamedType.GetHashCode(type, name);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata || null == container._registry) continue;

                var registry = container._registry;

                // Check for exact match
                for (var i = registry.Buckets[(hashCode & HashMask) % registry.Buckets.Length]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Type != type ||
                      !(candidate.Value is ImplicitRegistration set) || set.Name != name)
                        continue;

                    // Found a registration
                    if (!(candidate.Value is ImplicitRegistration))
                        candidate.Value = container.CreateRegistration(type, name, candidate.Value);

                    return (ImplicitRegistration)candidate.Value;
                }
            }

            Debug.Assert(null != _root);

            return _root.GetOrAdd(hashCode, type, name, null);
        }


#if NETSTANDARD1_0 || NETCOREAPP1_0
        private ImplicitRegistration GetGenericRegistration(Type type, string? name, TypeInfo info)
#else
        private ImplicitRegistration GetGenericRegistration(Type type, string? name)
#endif
        {
            Type? generic = null;
            int targetBucket, hashGeneric = 0, hashDefault = 0;
            int hashExact = NamedType.GetHashCode(type, name);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata || null == container._registry) continue;

                var registry = container._registry;

                // Check for exact match
                targetBucket = (hashExact & HashMask) % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashExact || candidate.Type != type ||
                      !(candidate.Value is ImplicitRegistration set) || set.Name != name)
                        continue;

                    // Found a registration
                    if (!(candidate.Value is ImplicitRegistration))
                        candidate.Value = container.CreateRegistration(type, name, candidate.Value);

                    return (ImplicitRegistration)candidate.Value;
                }

                // Generic registrations
                if (null == generic)
                {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                    generic = info.GetGenericTypeDefinition();
                    hashGeneric = NamedType.GetHashCode(generic, name);
                    hashDefault = generic?.GetHashCode() ?? 0;
#else
                    generic = type.GetGenericTypeDefinition();
                    hashGeneric = NamedType.GetHashCode(generic, name);
                    hashDefault = generic?.GetHashCode() ?? 0;
#endif
                }

                // Check for factory with same name
                targetBucket = (hashGeneric & HashMask) % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashGeneric || candidate.Type != generic ||
                      !(candidate.Value is ImplicitRegistration set) || set.Name != name)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(hashExact, type, name, candidate.Value);
                }

                // Check for default factory
                targetBucket = (hashDefault & HashMask) % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashDefault || candidate.Type != generic ||
                      !(candidate.Value is ImplicitRegistration set) || set.Name != name)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(hashExact, type, name, candidate.Value);
                }
            }

            Debug.Assert(null != _root);

            return _root.GetOrAdd(hashExact, type, name, null);
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

            Debug.Assert(null != _registry);
            Debug.Assert(null != _metadata);

            // Registry
            lock (_syncRegistry)
            {
                registration.AddRef();
                var hashCode = NamedType.GetHashCode(type, name);
                var targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Type != type ||
                      !(candidate.Value is ImplicitRegistration set) || set.Name != name)
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
                    targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;
                }

                // Create new entry
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Type = type;
                entry.Value = registration;
                position = _registry.Count++;
                _registry.Buckets[targetBucket] = position;
            }

            collisions = 0;

            // Metadata
            lock (_syncMetadata)
            {
                var hashCode = type?.GetHashCode() ?? 0;
                var targetBucket = (hashCode & HashMask) % _metadata.Buckets.Length;

                for (var i = _metadata.Buckets[targetBucket]; i >= 0; i = _metadata.Entries[i].Next)
                {
                    ref var candidate = ref _metadata.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Type != type)
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
                    targetBucket = (hashCode & HashMask) % _metadata.Buckets.Length;
                }

                // Create new metadata entry
                ref var entry = ref _metadata.Entries[_metadata.Count];
                entry.Next = _metadata.Buckets[targetBucket];
                entry.HashCode = hashCode;
                entry.Type = type;
                entry.Value = new int[] { 2, position };
                _metadata.Buckets[targetBucket] = _metadata.Count++;
            }

            // Nothing to replace
            return null;
        }

        private ImplicitRegistration GetOrAdd(Type type, string? name, IPolicySet? factory = null)
            => GetOrAdd(NamedType.GetHashCode(type, name), type, name, factory);

        private ImplicitRegistration GetOrAdd(int hashCode, Type type, string? name, IPolicySet? factory)
        {
            Debug.Assert(null != _registry);

            lock (_syncRegistry)
            {
                var collisions = 0;
                var targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Type != type ||
                      !(candidate.Value is ImplicitRegistration set) || set.Name != name)
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
                    targetBucket = (hashCode & HashMask) % _registry.Buckets.Length;
                }

                // Add registration
                var registration = CreateRegistration(type, name, factory);
                registration.AddRef();
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
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
