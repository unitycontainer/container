using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Container;
using Unity.Container.Registration;
using Unity.Container.Storage;
using Unity.Events;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        private const int ContainerInitialCapacity = 37;
        private const int ListToHashCutoverPoint = 8;

        #endregion


        #region Fields

        private readonly object _syncRoot = new object();
        private HashRegistry<Type, IRegistry<string, IMap<Type, IBuilderPolicy>>> _registrations = 
            new HashRegistry<Type, IRegistry<string, IMap<Type, IBuilderPolicy>>>(ContainerInitialCapacity);

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
            var to = typeTo ?? throw new ArgumentNullException(nameof(typeTo));
            if (typeFrom != null && !typeFrom.GetTypeInfo().IsGenericType && !to.GetTypeInfo().IsGenericType)
            {
                if (!typeFrom.GetTypeInfo().IsAssignableFrom(to.GetTypeInfo()))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        Constants.TypesAreNotAssignable, typeFrom, to), nameof(typeFrom));
                }
            }

            // Register type
            var registration = new TypeRegistration(typeFrom, typeTo, name, lifetimeManager, injectionMembers);

            var old = SetOrUpdate(registration.RegisteredType, registration.Name, registration);
            if (old is IDisposable disposable)
            {
                _lifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // TODO: Obsolete
            _registeredNames.RegisterType(registration.RegisteredType, registration.Name);

            Registering?.Invoke(this, new RegisterEventArgs(registration.RegisteredType, registration.MappedToType, registration.Name, registration.LifetimeManager));

            if (null != injectionMembers && injectionMembers.Length > 0)
            {
                var proxy = new PolicyListProxy(_context, registration);
                foreach (var member in injectionMembers)
                {
                    member.AddPolicies(registration.RegisteredType, registration.MappedToType, registration.Name, proxy);
                }
            }

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
        /// <param name="registrationType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to be returned.</param>
        /// <param name="registrationName">Name for registration.</param>
        /// <param name="lifetimeManager">
        /// <para>If null or <see cref="ContainerControlledLifetimeManager"/>, the container will take over the lifetime of the instance,
        /// calling Dispose on it (if it's <see cref="IDisposable"/>) when the container is Disposed.</para>
        /// <para>
        ///  If <see cref="ExternallyControlledLifetimeManager"/>, container will not maintain a strong reference to <paramref name="instance"/>. 
        /// User is responsible for disposing instance, and for keeping the instance typeFrom being garbage collected.</para></param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RegisterInstance(Type registrationType, string registrationName, object instance, LifetimeManager lifetimeManager)
        {
            var registration = new InstanceRegistration(registrationType, registrationName, instance, lifetimeManager);

            if (registration.LifetimeManager is IDisposable manager) _lifetimeContainer.Add(manager);
            var old = SetOrUpdate(registration.RegisteredType, registration.Name, registration);

            if (old is IDisposable disposable)
            {
                _lifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // TODO: Obsolete
            _registeredNames.RegisterType(registration.RegisteredType, registration.Name);

            RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(registrationType, instance, registrationName, lifetimeManager));

            return this;
        }

        #endregion



        private IMap<Type, IBuilderPolicy> this[Type type, string name]
        {
            get
            {
                var index = -1;
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
                            _registrations.Entries[i].Value =
                                existing is HashRegistry<string, IMap<Type, IBuilderPolicy>> registry
                                    ? new HashRegistry<string, IMap<Type, IBuilderPolicy>>(registry)
                                    : new HashRegistry<string, IMap<Type, IBuilderPolicy>>(ListRegistry.ListToHashCutoverPoint * 2, ((ListRegistry)existing).Head);
                        }

                        index = i;
                        break;
                    }

                    if (-1 == index)
                    {

                        if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                        {
                            _registrations = new HashRegistry<Type, IRegistry<string, IMap<Type, IBuilderPolicy>>>(_registrations);
                            targetBucket = hashCode % _registrations.Buckets.Length;
                        }

                        index = _registrations.Count;
                        _registrations.Entries[_registrations.Count].HashCode = hashCode;
                        _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                        _registrations.Entries[_registrations.Count].Key = type;
                        _registrations.Entries[_registrations.Count].Value = new ListRegistry();
                        _registrations.Buckets[targetBucket] = _registrations.Count;
                        _registrations.Count++;
                    }

                    // TODO: Add more robust locking
                    var repo = _registrations.Entries[index].Value;
                    return repo is HashRegistry<string, IMap<Type, IBuilderPolicy>> hashRegistry
                        ? hashRegistry.GetOrAdd(name, () => new PolicyRegistry())
                        : ((ListRegistry)repo).GetOrAdd(name, () => new PolicyRegistry());
                }
            }
            set
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

                        var old = _registrations.Entries[i].Value[name];
                        if (value != old)
                        {
                            if (null != old && old is IContainerRegistration instance && 
                                     instance.LifetimeManager is IDisposable disposable)
                            {
                                _lifetimeContainer.Remove(disposable);
                                disposable.Dispose();
                            }
                            _registrations.Entries[i].Value[name] = value;
                            
                        }
                        return;
                    }

                    if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                    {
                        _registrations = new HashRegistry<Type, IRegistry<string, IMap<Type, IBuilderPolicy>>>(_registrations);
                        targetBucket = hashCode % _registrations.Buckets.Length;
                    }

                    _registrations.Entries[_registrations.Count].HashCode = hashCode;
                    _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                    _registrations.Entries[_registrations.Count].Key = type;
                    _registrations.Entries[_registrations.Count].Value = new ListRegistry(name, value);
                    _registrations.Buckets[targetBucket] = _registrations.Count;
                    _registrations.Count++;
                }
            }
        }

        private IBuilderPolicy this[Type type, string name, Type interfaceType]
        {
            get
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

                    return _registrations.Entries[i].Value?[name]?[interfaceType];
                }

                return null;
            }
            set => this[type, name][interfaceType] = value;
        }


        #region Thread Safe Accessors

        private IMap<Type, IBuilderPolicy> SetOrUpdate(Type type, string name, IMap<Type, IBuilderPolicy> registration)
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
                        existing = existing is HashRegistry<string, IMap<Type, IBuilderPolicy>> registry
                                 ? new HashRegistry<string, IMap<Type, IBuilderPolicy>>(registry)
                                 : new HashRegistry<string, IMap<Type, IBuilderPolicy>>(ListRegistry.ListToHashCutoverPoint * 2, ((ListRegistry)existing).Head);

                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.SetOrReplace(name, registration);
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IMap<Type, IBuilderPolicy>>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new ListRegistry(name, registration); 
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;

                return null;
            }
        }


        #endregion



        #region Registrations

        /// <summary>
        /// GetOrDefault a sequence of <see cref="ContainerRegistration"/> that describe the current state
        /// of the container.
        /// </summary>
        public IEnumerable<IContainerRegistration> Registrations
        {
            get
            {
                var allRegisteredNames = new Dictionary<Type, List<string>>();
                FillTypeRegistrationDictionary(allRegisteredNames);

                return
                    from type in allRegisteredNames.Keys
                    from name in allRegisteredNames[type]
                    select new ContainerRegistration(type, name, _context);
            }
        }

        private void FillTypeRegistrationDictionary(IDictionary<Type, List<string>> typeRegistrations)
        {
            if (_parent != null)
            {
                _parent.FillTypeRegistrationDictionary(typeRegistrations);
            }

            foreach (Type t in _registeredNames.RegisteredTypes)
            {
                if (!typeRegistrations.ContainsKey(t))
                {
                    typeRegistrations[t] = new List<string>();
                }

                typeRegistrations[t] =
                    (typeRegistrations[t].Concat(_registeredNames.GetKeys(t))).Distinct().ToList();
            }
        }

        #endregion
    }
}
