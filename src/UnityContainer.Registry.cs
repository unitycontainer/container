using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Extensions;
using Unity.Policy;
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

        public string All { get; } = "ALL NAMES";

        #endregion


        #region Fields

        private readonly object _syncRegistry = new object();
        private readonly object _syncMetadata = new object();
        private Registry<NamedType, InternalRegistration> _registry;
        private Registry<Type, int[]> _metadata;

        #endregion


        #region Defaults

        internal InternalRegistration Defaults => _root._registry.Entries[0].Value;

        #endregion


        #region Registration Methods

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Func<Type, string, InternalRegistration, InternalRegistration> Register;

        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //private Func<IEnumerable<Type>, string, InternalRegistration, IEnumerable<InternalRegistration>> RegisterAsync;

        #endregion


        #region Check if registered

        internal bool IsRegistered(Type type)
        {
            var hashCode = type?.GetHashCode() ?? 0 & UnityContainer.HashMask;

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                if (container._metadata.Contains(hashCode, type)) return true;
            }

            return false;
        }

        internal bool IsRegistered(ref BuilderContext context)
        {
            Type generic = null;
            int targetBucket, hashGeneric = -1, hashDefault = -1;
            int hashExact = NamedType.GetHashCode(context.Type, context.Name) & 0x7FFFFFFF;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = context.Type.GetTypeInfo();
            if (info.IsGenericType)
            {
                generic = info.GetGenericTypeDefinition();
                hashDefault = NamedType.GetHashCode(generic, null) & 0x7FFFFFFF;
                hashGeneric = (null != context.Name) ? NamedType.GetHashCode(generic, context.Name) & 0x7FFFFFFF : hashDefault;
            }
#else
            if (context.Type.IsGenericType)
            {
                generic = context.Type.GetGenericTypeDefinition();
                hashDefault = NamedType.GetHashCode(generic, null) & 0x7FFFFFFF;
                hashGeneric = (null != context.Name) ? NamedType.GetHashCode(generic, context.Name) & 0x7FFFFFFF : hashDefault;
            }
#endif

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                var registry = container._registry;

                // Check for exact match
                targetBucket = hashExact % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key.Type != context.Type) continue;

                    // Found a registration
                    return true;
                }

                // Skip to parent if not generic
                if (null == generic) continue;

                // Check for factory with same name
                targetBucket = hashGeneric % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key.Type != generic) continue;

                    // Found a factory
                    return true;
                }

                // Skip to parent if not generic
                if (hashGeneric == hashDefault) continue;

                // Check for default factory
                targetBucket = hashDefault % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key.Type != generic) continue;

                    // Found a factory
                    return true;
                }

            }

            return false;
        }

        #endregion


        #region Registry Manipulation

        private InternalRegistration InitAndAdd(Type type, string name, InternalRegistration registration)
        {
            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<NamedType, InternalRegistration>();
                if (null == _metadata)
                {
                    _metadata = new Registry<Type, int[]>();

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
                    if (candidate.HashCode != hashCode || candidate.Key.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    // Swap the registration
                    var existing = candidate.Value;

                    candidate.Value = registration;
                    candidate.Value.AddRef();

                    // Replaced registration
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
                    if (candidate.HashCode != hashCode || candidate.Key != type)
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
                    _metadata = new Registry<Type, int[]>(_metadata);
                    targetBucket = hashCode % _metadata.Buckets.Length;
                }

                // Create new metadata entry
                ref var entry = ref _metadata.Entries[_metadata.Count];
                entry.Next = _metadata.Buckets[targetBucket];
                entry.HashCode = hashCode;
                entry.Key = type;
                entry.Value = new int[] { 2, position };
                _metadata.Buckets[targetBucket] = _metadata.Count++;
            }

            // Nothing to replace
            return null;
        }

        private InternalRegistration GetOrAdd(int hashCode, Type type, string name, InternalRegistration factory)
        {
            lock (_syncRegistry)
            {
                var collisions = 0;
                var targetBucket = hashCode % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key.Type != type)
                    {
                        collisions++;
                        continue;
                    }

                    return candidate.Value;
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
                entry.Key.Type = type;
                entry.Key.Name = name;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = CreateRegistration(type, factory);
                entry.Value.AddRef();
                _registry.Buckets[targetBucket] = _registry.Count++;

                return entry.Value;
            }
        }

        #endregion


        #region Creating Registrations

        internal InternalRegistration GetRegistration(Type type, string name)
        {
            Type generic = null;
            int targetBucket, hashGeneric = -1, hashDefault = -1;
            int hashExact = NamedType.GetHashCode(type, name) & 0x7FFFFFFF;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
            if (info.IsGenericType)
            {
                generic = info.GetGenericTypeDefinition();
                hashDefault = NamedType.GetHashCode(generic, null) & 0x7FFFFFFF;
                hashGeneric = (null != name) ? NamedType.GetHashCode(generic, name) & 0x7FFFFFFF : hashDefault;
            }
#else
            if (type.IsGenericType)
            {
                generic = type.GetGenericTypeDefinition();
                hashDefault = NamedType.GetHashCode(generic, null) & 0x7FFFFFFF;
                hashGeneric = (null != name) ? NamedType.GetHashCode(generic, name) & 0x7FFFFFFF : hashDefault;
            }
#endif

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._metadata) continue;

                var registry = container._registry;

                // Check for exact match
                targetBucket = hashExact % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashExact || candidate.Key.Type != type)
                        continue;

                    // Found a registration
                    return candidate.Value;
                }

                // Skip to parent if not generic
                if (null == generic) continue;

                // Check for factory with same name
                targetBucket = hashGeneric % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashGeneric || candidate.Key.Type != generic)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(hashExact, type, name, candidate.Value);
                }

                // Skip to parent if not generic
                if (hashGeneric == hashDefault) continue;

                // Check for default factory
                targetBucket = hashDefault % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.HashCode != hashDefault || candidate.Key.Type != generic)
                        continue;

                    // Found a factory
                    return container.GetOrAdd(hashExact, type, name, candidate.Value);
                }

            }

            return _root.GetOrAdd(hashExact, type, name, null);
        }

        private InternalRegistration CreateRegistration(Type type, InternalRegistration factory)
        {
            var registration = new InternalRegistration(factory);

            registration.BuildChain = _strategiesChain.Where(strategy => strategy.RequiredToBuildType(this, type, registration, null))
                                                      .ToArray();

            if (registration.LifetimeManager is IDisposable)
                LifetimeContainer.Add(registration.LifetimeManager);

            return registration;
        }

        #endregion


        #region Registration manipulation

        private IEnumerable<IPolicySet> AddOrReplaceRegistrations(IEnumerable<Type> interfaces, string name, ContainerRegistration registration)
        {
            int count = 0;

            if (null != interfaces)
            {
                foreach (var type in interfaces)
                {
                    // Add or replace existing 
                    var previous = Register(type, name, registration);
                    if (null != previous) yield return previous;

                    count++;
                }
            }

            if (0 == count)
            {
                // TODO: Move to diagnostic
                if (null == registration.Type) throw new ArgumentNullException(nameof(interfaces));

                // Add or replace existing 
                var previous = Register(registration.Type, name, registration);
                if (null != previous) yield return previous;
            }
        }

        #endregion


        #region Policy manipulation

        internal object GetPolicy(Type type, Type policyInterface)
        {
            var hashCode = type?.GetHashCode() ?? 0 & HashMask;

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._registry) continue;

                // Skip to parent if nothing found
                var registration = container._registry.Get(hashCode, type);
                if (null == registration) continue;

                // Get the policy
                return registration.Get(policyInterface);
            }

            return null;
        }

        internal object GetPolicy(Type type, string name, Type policyInterface)
        {
            var hashCode = NamedType.GetHashCode(type, name) & HashMask;

            // Iterate through containers hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registrations
                if (null == container._registry) continue;

                // Skip to parent if nothing found
                var registration = container._registry.Get(hashCode, type);
                if (null == registration) continue;

                // Get the policy
                return registration.Get(policyInterface);
            }

            return null;
        }

        private void SetPolicy(Type type, Type policyInterface, object policy)
        {
            var hashCode = type?.GetHashCode() ?? 0 & HashMask;

            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<NamedType, InternalRegistration>();

                var targetBucket = hashCode % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key.Type != type)
                    {
                        continue;
                    }

                    candidate.Value.Set(policyInterface, policy);
                    return;
                }

                // Expand only if no more space
                if (_registry.Count >= _registry.Entries.Length)
                {
                    _registry = new Registry<NamedType, InternalRegistration>(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Key.Type = type;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = new InternalRegistration(policyInterface, policy);
                _registry.Buckets[targetBucket] = _registry.Count++;
            }
        }

        private void SetPolicy(Type type, string name, Type policyInterface, object policy)
        {
            var hashCode = NamedType.GetHashCode(type, name) & HashMask;

            lock (_syncRegistry)
            {
                if (null == _registry) _registry = new Registry<NamedType, InternalRegistration>();

                var targetBucket = hashCode % _registry.Buckets.Length;

                // Check for the existing 
                for (var i = _registry.Buckets[targetBucket]; i >= 0; i = _registry.Entries[i].Next)
                {
                    ref var candidate = ref _registry.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key.Type != type)
                    {
                        continue;
                    }

                    candidate.Value.Set(policyInterface, policy);
                    return;
                }

                // Expand only if no more space
                if (_registry.Count >= _registry.Entries.Length)
                {
                    _registry = new Registry<NamedType, InternalRegistration>(_registry);
                    targetBucket = hashCode % _registry.Buckets.Length;
                }

                // Add registration
                ref var entry = ref _registry.Entries[_registry.Count];
                entry.HashCode = hashCode;
                entry.Key.Type = type;
                entry.Key.Name = name;
                entry.Next = _registry.Buckets[targetBucket];
                entry.Value = new InternalRegistration(policyInterface, policy);
                _registry.Buckets[targetBucket] = _registry.Count++;
            }
        }

        internal ResolveDelegate<BuilderContext> GetResolverPolicy(Type type, string name)
        {
            var hashExact = NamedType.GetHashCode(type, name) & HashMask;
            var hashAll = type?.GetHashCode() ?? 0 & HashMask;

            // Iterate though hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for exact entry
                var policy = container._registry.Get(hashExact, type)?
                                                .Get(typeof(ResolveDelegate<BuilderContext>));
                if (null != policy) return (ResolveDelegate<BuilderContext>)policy;

                // Check for 'Cover it All' entry
                policy = container._registry.Get(hashAll, type)?
                                            .Get(typeof(ResolveDelegate<BuilderContext>));

                if (null != policy) return (ResolveDelegate<BuilderContext>)policy;
            }

            // Nothing found
            return default;
        }

        internal ResolveDelegateFactory GetFactoryPolicy(Type type)
        {
            var hashCode = type?.GetHashCode() ?? 0 & HashMask;
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for 'Cover it All' entry
                var policy = container._registry
                                      .Get(hashCode, type)?
                                      .Get(typeof(ResolveDelegateFactory));

                // Skip to parent if nothing
                if (null != policy) return (ResolveDelegateFactory)policy;
            }

            // Nothing found
            return null;
        }

        internal ResolveDelegateFactory GetFactoryPolicy(Type type, string name)
        {
            var hashExact = NamedType.GetHashCode(type, name) & HashMask;
            var hashNull = NamedType.GetHashCode(type, null) & HashMask;
            var hashAll = type?.GetHashCode() ?? 0 & HashMask;

            // Iterate though hierarchy
            for (var container = this; null != container; container = container._parent)
            {
                // Skip if no local registrations
                if (null == container._registry) continue;

                // Check for exact entry
                var policy = container._registry.Get(hashExact, type)?
                                                .Get(typeof(ResolveDelegateFactory))
                // Check for 'Cover it All' entry
                          ?? container._registry.Get(hashAll, type)?
                                                .Get(typeof(ResolveDelegateFactory));
                // Return if found
                if (null != policy) return (ResolveDelegateFactory)policy;

                // Skip if name is 'null'
                if (hashExact == hashNull) continue;

                // Check for name 'null' entry
                policy = container._registry.Get(hashNull, type)?
                                            .Get(typeof(ResolveDelegateFactory));

                if (null != policy) return (ResolveDelegateFactory)policy;
            }

            // Nothing found
            return null;
        }

        #endregion
    }
}
