using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Events;
using Unity.Injection;
using Unity.Registration;
using Unity.Storage;


namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ContainerInitialCapacity = 37;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ListToHashCutoverPoint = 8;

        #endregion


        #region Type Registration

        /// <summary>
        /// RegisterType a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="typeFrom"><see cref="Type"/> that will be requested.</param>
        /// <param name="typeTo"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RegisterType(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            // Validate input
            if (string.Empty == name) name = null;
            if (null == typeTo) throw new ArgumentNullException(nameof(typeTo));
            if (null == lifetimeManager) lifetimeManager = TransientLifetimeManager.Instance;
            if (lifetimeManager.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeFrom != null && !typeFrom.GetTypeInfo().IsGenericType && !typeTo.GetTypeInfo().IsGenericType && 
                                    !typeFrom.GetTypeInfo().IsAssignableFrom(typeTo.GetTypeInfo()))
#else
            if (typeFrom != null && !typeFrom.IsGenericType && !typeTo.IsGenericType &&
                                    !typeFrom.IsAssignableFrom( typeTo))
#endif
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Constants.TypesAreNotAssignable, typeFrom, typeTo), nameof(typeFrom));
            }

            // Create registration and add to appropriate storage
            var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
            var registration = new ContainerRegistration(typeFrom, name, typeTo, lifetimeManager, injectionMembers);

            // Add or replace existing 
            var previous = container.Register(registration);
            if (previous is ContainerRegistration old &&
                old.LifetimeManager is IDisposable disposable)
            {
                // Dispose replaced lifetime manager
                container._lifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // If Disposable add to container's lifetime
            if (registration.LifetimeManager is IDisposable manager)
                container._lifetimeContainer.Add(manager);

            // Add Injection Members
            if (null != injectionMembers && injectionMembers.Length > 0)
            {
                var context = new RegistrationContext(this, registration);
                foreach (var member in injectionMembers)
                {
                    member.AddPolicies<BuilderContext, RegistrationContext>(
                        registration.RegisteredType, registration.MappedToType,
                        registration.Name, ref context);
                }
            }

            // Check what strategies to run
            registration.BuildChain = _buildChain.ToArray()
                                                 .Where(strategy => strategy.RequiredToBuildType(this, registration, injectionMembers))
                                                 .ToArray();
            // Raise event
            container.Registering?.Invoke(this, new RegisterEventArgs(registration.RegisteredType,
                                                                      registration.MappedToType,
                                                                      registration.Name,
                                                                      registration.LifetimeManager));
            return this;
        }

        #endregion


        #region Instance Registration

        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks> <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para></remarks>
        /// <param name="registeredType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to be returned.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="lifetimeManager">
        /// <para>If null or <see cref="ContainerControlledLifetimeManager"/>, the container will take over the lifetime of the instance,
        /// calling Dispose on it (if it's <see cref="IDisposable"/>) when the container is Disposed.</para>
        /// <para>
        ///  If <see cref="ExternallyControlledLifetimeManager"/>, container will not maintain a strong reference to <paramref name="instance"/>. 
        /// User is responsible for disposing instance, and for keeping the instance typeFrom being garbage collected.</para></param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RegisterInstance(Type registeredType, string name, object instance, LifetimeManager lifetimeManager)
        {
            // Validate input
            if (string.Empty == name) name = null;
            if (null == instance) throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();
            var lifetime = lifetimeManager ?? new ContainerControlledLifetimeManager();
            if (lifetime.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);
            lifetime.SetValue(instance, _lifetimeContainer);

            // Create registration and add to appropriate storage
            var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
            var registration = new ContainerRegistration(registeredType ?? type, name, type, lifetime);

            // Add or replace existing 
            var previous = container.Register(registration);
            if (previous is ContainerRegistration old &&
                old.LifetimeManager is IDisposable disposable)
            {
                // Dispose replaced lifetime manager
                container._lifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // If Disposable add to container's lifetime
            if (registration.LifetimeManager is IDisposable manager)
                container._lifetimeContainer.Add(manager);

            // Check what strategies to run
            registration.BuildChain = _buildChain.ToArray()
                                                 .Where(strategy => strategy.RequiredToResolveInstance(this, registration))
                                                 .ToArray();
            // Raise event
            container.RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(registration.RegisteredType, instance,
                                                                   registration.Name, registration.LifetimeManager));
            return this;
        }

        #endregion


        #region Check Registration

        public bool IsRegistered(Type type, string name) =>
            ReferenceEquals(string.Empty, name) ? _isTypeExplicitlyRegistered(type)
                                                : _isExplicitlyRegistered(type, name);

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
                return registry?[name] is IContainerRegistration ||
                       (_parent?.IsRegistered(type, name) ?? false);
            }

            return _parent?.IsRegistered(type, name) ?? false;
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
                           .Any(v => v is IContainerRegistration) ||
                       (_parent?._isTypeExplicitlyRegistered(type) ?? false);
            }

            return _parent?._isTypeExplicitlyRegistered(type) ?? false;
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
                    if (null == defaultRegistration) defaultRegistration = registry[string.Empty];
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
            if (!type.IsGenericType) return false;

            type = type.GetGenericTypeDefinition();
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
                    if (null == defaultRegistration) defaultRegistration = registry[string.Empty];
                    if (null != name && null == noNameRegistration) noNameRegistration = registry[null];
                }
            }

            if (null != defaultRegistration) return true;
            return null != noNameRegistration;
        }


        #endregion


        #region Registrations Collection

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

                    case HashRegistry<string, IPolicySet> hashRegistry:
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

                    case HashRegistry<string, IPolicySet> hashRegistry:
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

                    case HashRegistry<string, IPolicySet> hashRegistry:
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

        private IPolicySet AddOrUpdate(InternalRegistration registration)
        {
            var collisions = 0;
            var hashCode = (registration.Type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    ref var candidate = ref _registrations.Entries[i];
                    if (candidate.HashCode != hashCode ||
                        candidate.Key != registration.Type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = candidate.Value;
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry<string, IPolicySet> registry
                                 ? new HashRegistry<string, IPolicySet>(registry)
                                 : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2, (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.SetOrReplace(registration.Name, registration);
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                ref var entry = ref _registrations.Entries[_registrations.Count];
                entry.HashCode = hashCode;
                entry.Next = _registrations.Buckets[targetBucket];
                entry.Key = registration.Type;
                entry.Value = new LinkedRegistry(registration.Name, registration);
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
                        existing = existing is HashRegistry<string, IPolicySet> registry
                                 ? new HashRegistry<string, IPolicySet>(registry)
                                 : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                                                       (LinkedRegistry)existing);
                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.GetOrAdd(name, () => CreateRegistration(type, name));
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
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
                        existing = existing is HashRegistry<string, IPolicySet> registry
                                 ? new HashRegistry<string, IPolicySet>(registry)
                                 : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                                                       (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.GetOrAdd(name, () => CreateRegistration(type, name));
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
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
                        existing = existing is HashRegistry<string, IPolicySet> registry
                            ? new HashRegistry<string, IPolicySet>(registry)
                            : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    existing[name] = value;
                    return;
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
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
                        existing = existing is HashRegistry<string, IPolicySet> registry
                                 ? new HashRegistry<string, IPolicySet>(registry)
                                 : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                                                       (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    existing.GetOrAdd(name, () => CreateRegistration(type, name, policyInterface, policy));
                    return;
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = CreateRegistration(type, name, policyInterface, policy);
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
