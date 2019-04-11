using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ContainerInitialCapacity = 37;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ListToHashCutPoint = 8;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public const string All = "ALL";

        #endregion


        #region Delegates

        private delegate InternalRegistration RegisterDelegate(ref NamedType key, InternalRegistration registration);

        #endregion


        #region Registration Methods

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private RegisterDelegate Register;

        #endregion


        #region Registration Fields

        // Register single type

        internal IPolicySet Defaults;
        private readonly object _syncRoot = new object();
        private  LinkedNode<Type, object> _validators;
        private Registrations _registrations;

        #endregion


        #region Check Registration

        private bool IsExplicitlyRegisteredLocally(Type type, string name)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                ref var candidate = ref _registrations.Entries[i];
                if (candidate.HashCode != hashCode ||
                    candidate.Key != type)
                {
                    continue;
                }

                var registry = candidate.Value;
                return registry?[name] is ContainerRegistration ||
                       (((IUnityContainer)_parent)?.IsRegistered(type, name) ?? false);
            }

            return ((IUnityContainer)_parent)?.IsRegistered(type, name) ?? false;
        }

        private bool IsTypeTypeExplicitlyRegisteredLocally(Type type)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                ref var candidate = ref _registrations.Entries[i];
                if (candidate.HashCode != hashCode ||
                    candidate.Key != type)
                {
                    continue;
                }

                return candidate.Value
                           .Values
                           .Any(v => v is ContainerRegistration) ||
                       (_parent?.IsTypeExplicitlyRegistered(type) ?? false);
            }

            return _parent?.IsTypeExplicitlyRegistered(type) ?? false;
        }

        protected bool IsTypeTypeExplicitlyRegisteredRecurcive(Type type)
        {
            if (null != _registrations)
            {
                var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
                var targetBucket = hashCode % _registrations.Buckets.Length;
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    ref var candidate = ref _registrations.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key != type)
                    {
                        continue;
                    }

                    return candidate.Value
                               .Values
                               .Any(v => v is ContainerRegistration) ||
                           (_parent?.IsTypeExplicitlyRegistered(type) ?? false);
                }
            }

            return _parent?.IsTypeExplicitlyRegistered(type) ?? false;
        }

        internal bool RegistrationExists(ref BuilderContext context)
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
                if (null == container._registry) continue;

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


        #region Registrations Collections

        private static RegistrationSet GetRegistrations(UnityContainer container)
        {
            var seed = null != container._parent ? GetRegistrations(container._parent)
                                                 : new RegistrationSet();

            if (null == container._registry) return seed;

            var registry = container._registry;

            for (var i = null == container._parent ? GetStartIndex() : 0; i < registry.Count; i++)
            {
                ref var entry = ref registry.Entries[i];
                if (entry.Value is ContainerRegistration containerRegistration)
                    seed.Add(entry.Key.Type, entry.Key.Name, containerRegistration);
            }

            return seed;

            int GetStartIndex()
            {
                int start = -1;
                while (++start < registry.Count)
                {
                    if (typeof(IUnityContainer) != container._registry.Entries[start].Key.Type)
                        continue;
                    return start;
                }

                return 0;
            }
        }

        private static RegistrationSet GetRegistrations(UnityContainer container, params Type[] types)
        {
            var seed = null != container._parent ? GetRegistrations(container._parent, types)
                                                 : new RegistrationSet();

            if (null == container._registry) return seed;

            var registry = container._registry;

            foreach (var type in types)
            {
                foreach (var i in container._metadata.Get(type))
                {
                    ref var entry = ref registry.Entries[i];
                    if (entry.Value is ContainerRegistration containerRegistration)
                        seed.Add(entry.Key.Type, entry.Key.Name, containerRegistration);
                }
            }

            return seed;
        }

        private static RegistrationSet GetNamedRegistrations(UnityContainer container, params Type[] types)
        {
            var seed = null != container._parent ? GetNamedRegistrations(container._parent, types)
                                                 : new RegistrationSet();

            if (null == container._registrations) return seed;

            var registry = container._registry;

            foreach (var type in types)
            {
                foreach (var i in container._metadata.Get(type))
                {
                    ref var entry = ref registry.Entries[i];
                    if (entry.Value is ContainerRegistration containerRegistration && !string.IsNullOrEmpty(entry.Key.Name))
                        seed.Add(entry.Key.Type, entry.Key.Name, containerRegistration);
                }
            }

            return seed;
        }

        #endregion


        #region Type of named registrations

        private IRegistry<string, IPolicySet> Get(Type type)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                ref var candidate = ref _registrations.Entries[i];
                if (candidate.HashCode != hashCode ||
                    candidate.Key != type)
                {
                    continue;
                }

                return candidate.Value;
            }

            return null;
        }

        #endregion


        #region Dynamic Registrations

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
                if (null == container._registry) continue;

                var registry = container._registry;

                // Check for exact match
                targetBucket = hashExact % registry.Buckets.Length;
                for (var i = registry.Buckets[targetBucket]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key.Type != type) continue;

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
                    if (candidate.Key.Type != generic) continue;

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
                    if (candidate.Key.Type != generic) continue;

                    // Found a factory
                    return container.GetOrAdd(hashExact, type, name, candidate.Value);
                }

            }

            return _root.GetOrAdd(hashExact, type, name, null);
        }

        private IPolicySet GetDynamicRegistration(Type type, string name)
        {
            var registration = _get(type, name);
            if (null != registration) return registration;

            var info = type.GetTypeInfo();
            return !info.IsGenericType
                ? _root.GetOrAdd(type, name)
                : GetOrAddGeneric(type, name, info.GetGenericTypeDefinition());
        }

        private InternalRegistration CreateRegistration(Type type, string name, InternalRegistration factory)
        {
            var registration = new InternalRegistration(type, name);

            if (null != factory)
            {
                registration.InjectionMembers = factory.InjectionMembers;
                registration.Map = factory.Map;
                var manager = factory.LifetimeManager;
                if (null != manager)
                {
                    var policy = manager.CreateLifetimePolicy();
                    registration.LifetimeManager = policy;
                    if (policy is IDisposable) LifetimeContainer.Add(policy);
                }
            }

            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        private IPolicySet CreateRegistration(Type type, string name)
        {
            // TODO: Verify constructor
            var registration = new InternalRegistration(type, name);

            if (type.GetTypeInfo().IsGenericType)
            {
                var factory = (InternalRegistration)_get(type.GetGenericTypeDefinition(), name);
                if (null != factory)
                {
                    registration.InjectionMembers = factory.InjectionMembers;
                    registration.Map = factory.Map;
                    var manager = factory.LifetimeManager;
                    if (null != manager)
                    {
                        var policy = manager.CreateLifetimePolicy();
                        registration.LifetimeManager = policy;
                        if (policy is IDisposable) LifetimeContainer.Add(policy);
                    }
                }
            }

            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        private IPolicySet CreateRegistration(Type type, Type policyInterface, object policy)
        {
            var registration = new InternalRegistration(policyInterface, policy);
            registration.BuildChain = GetBuilders(type, registration);
            return registration;
        }

        #endregion


        #region Registration manipulation

        private IEnumerable<IPolicySet> AddOrReplaceRegistrations(IEnumerable<Type> interfaces, string name, ContainerRegistration registration)
        {
            NamedType key;

            int count = 0;
            key.Name = name;

            if (null != interfaces)
            {
                foreach (var type in interfaces)
                {
                    // Type to register
                    key.Type = type;

                    // Add or replace existing 
                    var previous = Register(ref key, registration);
                    if (null != previous) yield return previous;

                    count++;
                }
            }

            if (0 == count)
            {
                // TODO: Move to diagnostic
                if (null == registration.Type) throw new ArgumentNullException(nameof(interfaces));

                // Type to register
                key.Type = registration.Type;

                // Add or replace existing 
                var previous = Register(ref key, registration);
                if (null != previous) yield return previous;
            }
        }


        #endregion


        #region Legacy

        private IPolicySet AddOrUpdateLegacy(Type type, string name, InternalRegistration registration)
        {
            var collisions = 0;
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    ref var candidate = ref _registrations.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = candidate.Value;
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry registry
                                 ? new HashRegistry(registry)
                                 : new HashRegistry(LinkedRegistry.ListToHashCutoverPoint * 2, (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.SetOrReplace(name, registration);
                }

                if (_registrations.RequireToGrow || ListToHashCutPoint < collisions)
                {
                    _registrations = new Registrations(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                ref var entry = ref _registrations.Entries[_registrations.Count];
                entry.HashCode = hashCode;
                entry.Next = _registrations.Buckets[targetBucket];
                entry.Key = type;
                entry.Value = new LinkedRegistry(name, registration);
                _registrations.Buckets[targetBucket] = _registrations.Count++;

                return null;
            }
        }

        private IPolicySet GetOrAdd(Type type, string name)
        {
            var collisions = 0;
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;

            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                ref var candidate = ref _registrations.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key != type)
                {
                    continue;
                }

                var policy = candidate.Value?[name];
                if (null != policy) return policy;
            }

            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    ref var candidate = ref _registrations.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = candidate.Value;
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry registry
                                 ? new HashRegistry(registry)
                                 : new HashRegistry(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                   (LinkedRegistry)existing);
                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.GetOrAdd(name, () => CreateRegistration(type, name));
                }

                if (_registrations.RequireToGrow || ListToHashCutPoint < collisions)
                {
                    _registrations = new Registrations(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = CreateRegistration(type, name);
                ref var entry = ref _registrations.Entries[_registrations.Count];
                entry.HashCode = hashCode;
                entry.Next = _registrations.Buckets[targetBucket];
                entry.Key = type;
                entry.Value = new LinkedRegistry(name, registration);
                _registrations.Buckets[targetBucket] = _registrations.Count++;
                return registration;
            }
        }

        private IPolicySet GetOrAddGeneric(Type type, string name, Type definition)
        {
            var collisions = 0;
            int hashCode;
            int targetBucket;

            if (null != _parent)
            {
                hashCode = (definition?.GetHashCode() ?? 0) & 0x7FFFFFFF;
                targetBucket = hashCode % _registrations.Buckets.Length;
                for (var j = _registrations.Buckets[targetBucket]; j >= 0; j = _registrations.Entries[j].Next)
                {
                    ref var candidate = ref _registrations.Entries[j];
                    if (candidate.HashCode != hashCode || candidate.Key != definition)
                    {
                        continue;
                    }

                    if (null != candidate.Value?[name]) break;

                    return _parent._getGenericRegistration(type, name, definition);
                }
            }

            hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            targetBucket = hashCode % _registrations.Buckets.Length;

            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    ref var candidate = ref _registrations.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = candidate.Value;
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry registry
                                 ? new HashRegistry(registry)
                                 : new HashRegistry(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                   (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.GetOrAdd(name, () => CreateRegistration(type, name));
                }

                if (_registrations.RequireToGrow || ListToHashCutPoint < collisions)
                {
                    _registrations = new Registrations(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = CreateRegistration(type, name);
                ref var entry = ref _registrations.Entries[_registrations.Count];
                entry.HashCode = hashCode;
                entry.Next = _registrations.Buckets[targetBucket];
                entry.Key = type;
                entry.Value = new LinkedRegistry(name, registration);
                _registrations.Buckets[targetBucket] = _registrations.Count++;
                return registration;
            }


        }

        private IPolicySet Get(Type type, string name)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                ref var candidate = ref _registrations.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key != type)
                {
                    continue;
                }

                return candidate.Value?[name];
            }

            return null;
        }

        private void Set(Type type, string name, IPolicySet value)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            var collisions = 0;
            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    ref var candidate = ref _registrations.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = candidate.Value;
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry registry
                            ? new HashRegistry(registry)
                            : new HashRegistry(LinkedRegistry.ListToHashCutoverPoint * 2,
                                (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    existing[name] = value;
                    return;
                }

                if (_registrations.RequireToGrow || ListToHashCutPoint < collisions)
                {
                    _registrations = new Registrations(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                ref var entry = ref _registrations.Entries[_registrations.Count];
                entry.HashCode = hashCode;
                entry.Next = _registrations.Buckets[targetBucket];
                entry.Key = type;
                entry.Value = new LinkedRegistry(name, value);
                _registrations.Buckets[targetBucket] = _registrations.Count++;
            }
        }

        #endregion


        #region Local policy manipulation

        private object Get(Type type, string name, Type policyInterface)
        {
            object policy = null;
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                ref var candidate = ref _registrations.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key != type)
                {
                    continue;
                }

                policy = candidate.Value?[name]?.Get(policyInterface);
                break;
            }

            return policy ?? _parent?.GetPolicy(type, name, policyInterface);
        }

        private void Set(Type type, string name, Type policyInterface, object policy)
        {
            var collisions = 0;
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    ref var candidate = ref _registrations.Entries[i];
                    if (candidate.HashCode != hashCode || candidate.Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = candidate.Value;
                    var policySet = existing[name];
                    if (null != policySet)
                    {
                        policySet.Set(policyInterface, policy);
                        return;
                    }

                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry registry
                                 ? new HashRegistry(registry)
                                 : new HashRegistry(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                   (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    existing.GetOrAdd(name, () => CreateRegistration(type, policyInterface, policy));
                    return;
                }

                if (_registrations.RequireToGrow || ListToHashCutPoint < collisions)
                {
                    _registrations = new Registrations(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = CreateRegistration(type, policyInterface, policy);
                ref var entry = ref _registrations.Entries[_registrations.Count];
                entry.HashCode = hashCode;
                entry.Next = _registrations.Buckets[targetBucket];
                entry.Key = type;
                entry.Value = new LinkedRegistry(name, registration);
                _registrations.Buckets[targetBucket] = _registrations.Count++;
            }
        }

        private void Clear(Type type, string name, Type policyInterface)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                ref var candidate = ref _registrations.Entries[i];
                if (candidate.HashCode != hashCode || candidate.Key != type)
                {
                    continue;
                }

                candidate.Value?[name]?.Clear(policyInterface);
                return;
            }
        }

        #endregion
    }
}
