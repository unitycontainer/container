using System;
using System.Collections.Generic;
using System.Globalization;
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

        private const int ContainerInitialCapacity = 37;
        private const int ListToHashCutoverPoint = 8;

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
            // Validate imput
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
                    member.AddPolicies(registration.RegisteredType, registration.MappedToType, 
                                       registration.Name, context);
                }
            }

            // Register policies for each strategy
            var chain = new List<BuilderStrategy>();
            var strategies = _buildChain;
            for (var i = 0; i < strategies.Length; i++)
            {
                var strategy = strategies[i];
                if (strategy.RequiredToBuildType(this, registration, null))
                    chain.Add(strategy);
            }
            registration.BuildChain = chain;

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
            // Validate imput
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

            // Register Build strategies
            registration.BuildChain = _strategies.Where(s => s.RequiredToResolveInstance(this, registration))
                                                 .ToArray();
            // Raise event
            container.RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(registration.RegisteredType, instance,
                                                                   registration.Name, registration.LifetimeManager));
            return this;
        }

        #endregion


        #region Check Registration

        public bool IsRegistered(Type type, string name) => IsTypeRegistered(type, name);

        #endregion


        #region Registrations Collection

        /// <summary>
        /// GetOrDefault a sequence of <see cref="IContainerRegistration"/> that describe the current state
        /// of the container.
        /// </summary>
        public IEnumerable<IContainerRegistration> Registrations
        {
            get
            {
                return GetRegisteredTypes(this).SelectMany(type => GetRegisteredType(this, type));
            }
        }

        private ISet<Type> GetRegisteredTypes(UnityContainer container)
        {
            var set = null == container._parent ? new HashSet<Type>() 
                                                : GetRegisteredTypes(container._parent);

            if (null == container._registrations) return set;

            foreach (var type in container._registrations.Keys.Where(t => null != t))
                set.Add(type);

            return set;
        }

        private IEnumerable<IContainerRegistration> GetRegisteredType(UnityContainer container, Type type)
        {
            ReverseHashSet set;

            if (null != container._parent)
                set = (ReverseHashSet)GetRegisteredType(container._parent, type);
            else 
                set = new ReverseHashSet();

            if (null == container._registrations) return set;

            foreach (var registration in container.Get(type)?.Values.OfType<IContainerRegistration>()
                                      ?? Enumerable.Empty<IContainerRegistration>())
            {
                set.Add(registration);
            }

            return set;
        }

        private IEnumerable<string> GetRegisteredNames(UnityContainer container, Type type)
        {
            ISet<string> set;

            if (null != container._parent)
                set = (ISet<string>)GetRegisteredNames(container._parent, type);
            else
                set = new HashSet<string>();

            if (null == container._registrations) return set;

            foreach (var registration in container.Get(type)?.Values.OfType<IContainerRegistration>()
                                      ?? Enumerable.Empty<IContainerRegistration>())
            {
                set.Add(registration.Name);
            }

            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (generic != type)
            {
                foreach (var registration in container.Get(generic)?.Values.OfType<IContainerRegistration>()
                                          ?? Enumerable.Empty<IContainerRegistration>())
                {
                    set.Add(registration.Name);
                }
            }

            return set;
        }

        #endregion


        #region Entire Type of named registrations

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

        private IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
        {
            list = null;
            IBuilderPolicy policy = null;
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

            if (null != policy)
            {
                list = _context.Policies;
                return policy;
            }

            return _parent?.GetPolicy(type, name, policyInterface, out list);
        }

        private void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
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
                    if (existing.RequireToGrow)
                    {
                        existing = existing is HashRegistry<string, IPolicySet> registry
                                 ? new HashRegistry<string, IPolicySet>(registry)
                                 : new HashRegistry<string, IPolicySet>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                                                       (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    existing.GetOrAdd(name, () => new InternalRegistration(type, name)).Set(policyInterface, policy);
                    return;
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicySet>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = new InternalRegistration(type, name);
                registration.Set(policyInterface, policy);

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
