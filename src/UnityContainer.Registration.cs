using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Events;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] private const int ContainerInitialCapacity = 37;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]  private const int ListToHashCutoverPoint = 8;

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
            if (typeFrom != null && !typeFrom.GetTypeInfo().IsGenericType && !typeTo.GetTypeInfo().IsGenericType && 
                                    !typeFrom.GetTypeInfo().IsAssignableFrom(typeTo.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Constants.TypesAreNotAssignable, typeFrom, typeTo), nameof(typeFrom));
            }

            // Create registration and add to appropriate storage
            var container = (lifetimeManager is ISingletonLifetimePolicy) ? _root : this;
            var registration = new ContainerRegistration(typeFrom, name, typeTo, lifetimeManager);

            if (lifetimeManager is SingletonLifetimeManager || lifetimeManager is ContainerControlledLifetimeManager)
            {
                registration.Set(typeof(IBuildPlanPolicy), new ObjectBuilder.BuildPlan.ConstructorInvoke.ConstructorInvokeBuildPlan());
            }

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
            var chain = new List<BuilderStrategy>();
            var strategies = _buildChain;
            for (var i = 0; i < strategies.Length; i++)
            {
                var strategy = strategies[i];
                if (strategy.RequiredToBuildType(this, registration, injectionMembers))
                    chain.Add(strategy);
            }
            registration.BuildChain = chain.ToArray();

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
            var container = (lifetimeManager is ISingletonLifetimePolicy) ? _root : this;
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
            var chain = new List<BuilderStrategy>();
            var strategies = _buildChain;
            for (var i = 0; i < strategies.Length; i++)
            {
                var strategy = strategies[i];
                if (strategy.RequiredToResolveInstance(this, registration))
                    chain.Add(strategy);
            }
            registration.BuildChain = chain;

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
                if (_registrations.Entries[i].HashCode != hashCode ||
                    _registrations.Entries[i].Key != type)
                {
                    continue;
                }

                var registry = _registrations.Entries[i].Value;
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
                if (_registrations.Entries[i].HashCode != hashCode ||
                    _registrations.Entries[i].Key != type)
                {
                    continue;
                }

                return _registrations.Entries[i].Value
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
                    if (container._registrations.Entries[i].HashCode != hashCode ||
                        container._registrations.Entries[i].Key != type)
                    {
                        continue;
                    }

                    var registry = container._registrations.Entries[i].Value;

                    if (null != registry[name]) return true;
                    if (null == defaultRegistration) defaultRegistration = registry[string.Empty];
                    if (null != name && null == noNameRegistration) noNameRegistration = registry[null];
                }
            }

            if (null != defaultRegistration) return true;
            if (null != noNameRegistration) return true;

            var info = type.GetTypeInfo();
            if (!info.IsGenericType) return false;

            type = info.GetGenericTypeDefinition();
            hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            for (var container = this; null != container; container = container._parent)
            {
                if (null == container._registrations) continue;

                var targetBucket = hashCode % container._registrations.Buckets.Length;
                for (var i = container._registrations.Buckets[targetBucket]; i >= 0; i = container._registrations.Entries[i].Next)
                {
                    if (container._registrations.Entries[i].HashCode != hashCode ||
                        container._registrations.Entries[i].Key != type)
                    {
                        continue;
                    }

                    var registry = container._registrations.Entries[i].Value;

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

        private ISet<Type> GetRegisteredTypes(UnityContainer container)
        {
            var set = null == container._parent ? new HashSet<Type>() 
                                                : GetRegisteredTypes(container._parent);

            if (null == container._registrations) return set;

            var types = container._registrations.Keys;
            foreach (var type in types)
            {
                if (null == type) continue;
                set.Add(type);
            }

            return set;
        }

        private IEnumerable<IContainerRegistration> GetRegisteredType(UnityContainer container, Type type)
        {
            MiniHashSet<IContainerRegistration> set;

            if (null != container._parent)
                set = (MiniHashSet<IContainerRegistration>)GetRegisteredType(container._parent, type);
            else 
                set = new MiniHashSet<IContainerRegistration>();

            if (null == container._registrations) return set;

            var section = container.Get(type)?.Values;
            if (null == section) return set;
            
            foreach (var namedType in section)
            {
                if (namedType is IContainerRegistration registration)
                    set.Add(registration);
            }

            return set;
        }

        #endregion


        #region Type of named registrations

        private IRegistry<string, IPolicySet> Get(Type type)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                if (_registrations.Entries[i].HashCode != hashCode ||
                    _registrations.Entries[i].Key != type)
                {
                    continue;
                }

                return _registrations.Entries[i].Value;
            }

            return null;
        }

        #endregion


        #region Registration manipulation

        private IPolicySet AddOrUpdate(INamedType registration)
        {
            var collisions = 0;
            var hashCode = (registration.Type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    if (_registrations.Entries[i].HashCode != hashCode ||
                        _registrations.Entries[i].Key != registration.Type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = _registrations.Entries[i].Value;
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry<string, IPolicySet> registry
                                 ? new HashRegistry<string, IPolicySet>(registry)
                                 : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2, (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.SetOrReplace(registration.Name, (IPolicySet)registration);
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = registration.Type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry(registration.Name, (IPolicySet)registration);
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;

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
                if (_registrations.Entries[i].HashCode != hashCode ||
                    _registrations.Entries[i].Key != type)
                {
                    continue;
                }

                var policy = _registrations.Entries[i].Value?[name];
                if (null != policy) return policy; 
            }

            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    if (_registrations.Entries[i].HashCode != hashCode ||
                        _registrations.Entries[i].Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = _registrations.Entries[i].Value;
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
                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry(name, registration);
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;
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
                    if (_registrations.Entries[j].HashCode != hashCode ||
                        _registrations.Entries[j].Key != definition)
                    {
                        continue;
                    }

                    if (null != _registrations.Entries[j].Value?[name]) break;

                    return _parent._getGenericRegistration(type, name, definition);
                }
            }

            hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            targetBucket = hashCode % _registrations.Buckets.Length;

            lock (_syncRoot)
            {
                for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
                {
                    if (_registrations.Entries[i].HashCode != hashCode ||
                        _registrations.Entries[i].Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = _registrations.Entries[i].Value;
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
                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry(name, registration);
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;
                return registration;
            }


        }

        private IPolicySet Get(Type type, string name)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                if (_registrations.Entries[i].HashCode != hashCode ||
                    _registrations.Entries[i].Key != type)
                {
                    continue;
                }

                return _registrations.Entries[i].Value?[name];
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
                    if (_registrations.Entries[i].HashCode != hashCode ||
                        _registrations.Entries[i].Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = _registrations.Entries[i].Value;
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

                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry(name, value);
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;
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
                if (_registrations.Entries[i].HashCode != hashCode ||
                    _registrations.Entries[i].Key != type)
                {
                    continue;
                }

                policy = _registrations.Entries[i].Value?[name]?.Get(policyInterface);
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
                    if (_registrations.Entries[i].HashCode != hashCode ||
                        _registrations.Entries[i].Key != type)
                    {
                        collisions++;
                        continue;
                    }

                    var existing = _registrations.Entries[i].Value;
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
                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry(name, registration);
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;
            }
        }

        private void Clear(Type type, string name, Type policyInterface)
        {
            var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % _registrations.Buckets.Length;
            for (var i = _registrations.Buckets[targetBucket]; i >= 0; i = _registrations.Entries[i].Next)
            {
                if (_registrations.Entries[i].HashCode != hashCode ||
                    _registrations.Entries[i].Key != type)
                {
                    continue;
                }

                _registrations.Entries[i].Value?[name]?.Clear(policyInterface);
                return;
            }
        }

        #endregion
    }
}
