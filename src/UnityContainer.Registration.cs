using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ContainerInitialCapacity = 37;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ListToHashCutPoint = 8;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public const string All = "ALL";
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] internal const int HashMask = unchecked((int)(uint.MaxValue >> 1));

        #endregion


        #region Registration Fields

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

        internal bool RegistrationExists(Type type, string name)
        {
            IPolicySet defaultRegistration = null;
            IPolicySet noNameRegistration = null;

            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            for (var container = this; null != container; container = container._parent)
            {
                if (null == container._registrations) continue;

                var targetBucket = hashCode % container._registrations.Buckets.Length;
                for (var i = container._registrations.Buckets[targetBucket]; i >= 0; i = container._registrations.Entries[i].Next)
                {
                    ref var candidate = ref container._registrations.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key != type)
                    {
                        continue;
                    }

                    var registry = candidate.Value;

                    if (null != registry[name]) return true;
                    if (null == defaultRegistration) defaultRegistration = registry[All];
                    if (null != name && null == noNameRegistration) noNameRegistration = registry[null];
                }
            }

            if (null != defaultRegistration) return true;
            if (null != noNameRegistration) return true;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = type.GetTypeInfo();
            if (!info.IsGenericType) return false;

            type = info.GetGenericTypeDefinition();
#else
            if (!type?.IsGenericType ?? false) return false;

            type = type?.GetGenericTypeDefinition();
#endif
            hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            for (var container = this; null != container; container = container._parent)
            {
                if (null == container._registrations) continue;

                var targetBucket = hashCode % container._registrations.Buckets.Length;
                for (var i = container._registrations.Buckets[targetBucket]; i >= 0; i = container._registrations.Entries[i].Next)
                {
                    ref var candidate = ref container._registrations.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key != type)
                    {
                        continue;
                    }

                    var registry = candidate.Value;

                    if (null != registry[name]) return true;
                    if (null == defaultRegistration) defaultRegistration = registry[All];
                    if (null != name && null == noNameRegistration) noNameRegistration = registry[null];
                }
            }

            if (null != defaultRegistration) return true;
            return null != noNameRegistration;
        }

        #endregion


        #region Registrations Collections

        private static RegistrationSet GetRegistrations(UnityContainer container)
        {
            var seed = null != container._parent ? GetRegistrations(container._parent)
                                                 : new RegistrationSet();

            if (null == container._registrations) return seed;

            var length = container._registrations.Count;
            var entries = container._registrations.Entries;

            for (var i = null == container._parent ? GetStartIndex() : 0; i < length; i++)
            {
                ref var entry = ref entries[i];
                var registry = entry.Value;

                switch (registry)
                {
                    case LinkedRegistry linkedRegistry:
                        for (var node = (LinkedNode<string, IPolicySet>)linkedRegistry; null != node; node = node.Next)
                        {
                            if (node.Value is ContainerRegistration containerRegistration)
                                seed.Add(entry.Key, node.Key, containerRegistration);
                        }
                        break;

                    case HashRegistry hashRegistry:
                        var count = hashRegistry.Count;
                        var nodes = hashRegistry.Entries;
                        for (var j = 0; j < count; j++)
                        {
                            ref var refNode = ref nodes[j];
                            if (refNode.Value is ContainerRegistration containerRegistration)
                                seed.Add(entry.Key, refNode.Key, containerRegistration);
                        }
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of registry");
                }
            }

            return seed;

            int GetStartIndex()
            {
                int start = -1;
                while (++start < length)
                {
                    if (typeof(IUnityContainer) != container._registrations.Entries[start].Key)
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

            if (null == container._registrations) return seed;

            foreach (var type in types)
            {
                var registry = container.Get(type);
                if (null == registry?.Values) continue;

                switch (registry)
                {
                    case LinkedRegistry linkedRegistry:
                        for (var node = (LinkedNode<string, IPolicySet>)linkedRegistry; null != node; node = node.Next)
                        {
                            if (node.Value is ContainerRegistration containerRegistration)
                                seed.Add(type, node.Key, containerRegistration);
                        }
                        break;

                    case HashRegistry hashRegistry:
                        var count = hashRegistry.Count;
                        var nodes = hashRegistry.Entries;
                        for (var j = 0; j < count; j++)
                        {
                            ref var refNode = ref nodes[j];
                            if (refNode.Value is ContainerRegistration containerRegistration)
                                seed.Add(type, refNode.Key, containerRegistration);
                        }
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of registry");
                }
            }

            return seed;
        }

        private static RegistrationSet GetNamedRegistrations(UnityContainer container, params Type[] types)
        {
            var seed = null != container._parent ? GetNamedRegistrations(container._parent, types)
                                                 : new RegistrationSet();

            if (null == container._registrations) return seed;

            foreach (var type in types)
            {
                var registry = container.Get(type);
                if (null == registry?.Values) continue;

                switch (registry)
                {
                    case LinkedRegistry linkedRegistry:
                        for (var node = (LinkedNode<string, IPolicySet>)linkedRegistry; null != node; node = node.Next)
                        {
                            if (node.Value is ContainerRegistration containerRegistration && !string.IsNullOrEmpty(node.Key))
                                seed.Add(type, node.Key, containerRegistration);
                        }
                        break;

                    case HashRegistry hashRegistry:
                        var count = hashRegistry.Count;
                        var nodes = hashRegistry.Entries;
                        for (var j = 0; j < count; j++)
                        {
                            ref var refNode = ref nodes[j];
                            if (refNode.Value is ContainerRegistration containerRegistration && !string.IsNullOrEmpty(refNode.Key))
                                seed.Add(type, refNode.Key, containerRegistration);
                        }
                        break;

                    default:
                        throw new InvalidOperationException("Unknown type of registry");
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


        #region Registration manipulation


        // Register new and return overridden registration
        internal IPolicySet AppendNew(Type type, string name, InternalRegistration registration)
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

                    if (candidate.Value is HashRegistry registry)
                    {
                        candidate.Value = new LinkedRegistry(registry);
                    }
                    
                    var existing = candidate.Value as LinkedRegistry;
                    Debug.Assert(null != existing);

                    existing.Append(name, registration);

                    return null;
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

        // Register new and return overridden registration
        private IPolicySet AddOrUpdate(Type type, string name, InternalRegistration registration)
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

        // Return generic registration or create from factory if not registered
        private IPolicySet GetOrAddGeneric(Type type, string name, Type definition)
        {
            var collisions = 0;
            int hashCode;
            int targetBucket;
            InternalRegistration factory = null;

            hashCode = (definition?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            targetBucket = hashCode % _registrations.Buckets.Length;
            for (var j = _registrations.Buckets[targetBucket]; j >= 0; j = _registrations.Entries[j].Next)
            {
                ref var candidate = ref _registrations.Entries[j];
                if (candidate.HashCode != hashCode || candidate.Key != definition)
                {
                    continue;
                }

                if (null != (factory = (InternalRegistration)candidate.Value?[name])) break;
            }

            if (null == factory && null != _parent) return _parent._getGenericRegistration(type, name, definition);

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

                    return existing.GetOrAdd(name, () => CreateRegistration(type, name, factory));
                }

                if (_registrations.RequireToGrow || ListToHashCutPoint < collisions)
                {
                    _registrations = new Registrations(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = CreateRegistration(type, name, factory);
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
