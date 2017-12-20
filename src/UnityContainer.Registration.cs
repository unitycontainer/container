using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
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


        #region Fields

        private readonly object _syncRoot = new object();
        private HashRegistry<Type, IRegistry<string, IPolicyStore>> _registrations = 
            new HashRegistry<Type, IRegistry<string, IPolicyStore>>(ContainerInitialCapacity);

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
            if (registration.LifetimeManager is IDisposable manager) _lifetimeContainer.Add(manager);

            if (SetOrUpdate(registration.RegisteredType, registration.Name, registration) is IDisposable disposable)
            {
                _lifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

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

            if (SetOrUpdate(registration.RegisteredType, registration.Name, registration) is IDisposable disposable)
            {
                _lifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(registrationType, instance, registrationName, lifetimeManager));

            return this;
        }

        #endregion


        #region Check Registration


        public bool IsRegistered(Type type, string name)
        {
            for (var registry = this; null != registry; registry = registry._parent)
            {
                if (null == registry[type, name]) continue;

                return true;
            }

            return false;
        }

        #endregion


        private IRegistry<string, IPolicyStore> this[Type type]
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

                    return _registrations.Entries[i].Value;
                }

                return null;
            }
        }

        private IPolicyStore this[Type type, string name]
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

                    return _registrations.Entries[i].Value?[name];
                }

                return null;
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

                        var existing = _registrations.Entries[i].Value;
                        if (existing.RequireToGrow)
                        {
                            existing = existing is HashRegistry<string, IPolicyStore> registry
                                ? new HashRegistry<string, IPolicyStore>(registry)
                                : new HashRegistry<string, IPolicyStore>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                    (LinkedRegistry)existing);

                            _registrations.Entries[i].Value = existing;
                        }

                        existing[name] = value;
                        return;
                    }

                    if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                    {
                        _registrations = new HashRegistry<Type, IRegistry<string, IPolicyStore>>(_registrations);
                        targetBucket = hashCode % _registrations.Buckets.Length;
                    }

                    _registrations.Entries[_registrations.Count].HashCode = hashCode;
                    _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                    _registrations.Entries[_registrations.Count].Key = type;
                    _registrations.Entries[_registrations.Count].Value = new LinkedRegistry { [name] = value }; 
                    _registrations.Buckets[targetBucket] = _registrations.Count;
                    _registrations.Count++;
                }
            }
        }


        /// <summary>
        /// Gets or sets policy for specified named type 
        /// </summary>
        /// <remarks>
        /// This call never fails. If type or name is not present this method crates 
        /// empty <see cref="InternalRegistration"/> object and initializes it with policy
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private IBuilderPolicy this[Type type, string name, Type interfaceType]
        {
            get
            {
                var collisions = 0;
                var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
                var targetBucket = hashCode % _registrations.Buckets.Length;
                IPolicyStore registration = null;
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
                            existing = existing is HashRegistry<string, IPolicyStore> registry
                                     ? new HashRegistry<string, IPolicyStore>(registry)
                                     : new HashRegistry<string, IPolicyStore>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                                                           (LinkedRegistry)existing);

                            _registrations.Entries[i].Value = existing;
                        }

                        registration = existing.GetOrAdd(name, () => CreateRegistration(type, name));
                        break;
                    }

                    if (null == registration)
                    {
                        if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                        {
                            _registrations = new HashRegistry<Type, IRegistry<string, IPolicyStore>>(_registrations);
                            targetBucket = hashCode % _registrations.Buckets.Length;
                        }

                        registration = CreateRegistration(type, name);
                        _registrations.Entries[_registrations.Count].HashCode = hashCode;
                        _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                        _registrations.Entries[_registrations.Count].Key = type;
                        _registrations.Entries[_registrations.Count].Value = new LinkedRegistry { [name] = registration };
                        _registrations.Buckets[targetBucket] = _registrations.Count;
                        _registrations.Count++;
                    }
                }

                return registration.Get(interfaceType);
            }
            set
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
                            existing = existing is HashRegistry<string, IPolicyStore> registry
                                     ? new HashRegistry<string, IPolicyStore>(registry)
                                     : new HashRegistry<string, IPolicyStore>(LinkedRegistry.ListToHashCutoverPoint * 2, 
                                                                                           (LinkedRegistry)existing);

                            _registrations.Entries[i].Value = existing;
                        }

                        existing.GetOrAdd(name, () => CreateRegistration(type, name)).Set(interfaceType, value);
                        return;
                    }

                    if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                    {
                        _registrations = new HashRegistry<Type, IRegistry<string, IPolicyStore>>(_registrations);
                        targetBucket = hashCode % _registrations.Buckets.Length;
                    }

                    var registration = CreateRegistration(type, name);
                    registration.Set(interfaceType, value);

                    _registrations.Entries[_registrations.Count].HashCode = hashCode;
                    _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                    _registrations.Entries[_registrations.Count].Key = type;                       
                    _registrations.Entries[_registrations.Count].Value = new LinkedRegistry { [name] = registration };
                    _registrations.Buckets[targetBucket] = _registrations.Count;
                    _registrations.Count++;
                }
            }
        }


        #region Thread Safe Accessors

        private IPolicyStore SetOrUpdate(Type type, string name, IPolicyStore registration)
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
                        existing = existing is HashRegistry<string, IPolicyStore> registry
                                 ? new HashRegistry<string, IPolicyStore>(registry)
                                 : new HashRegistry<string, IPolicyStore>(LinkedRegistry.ListToHashCutoverPoint * 2, (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return existing.SetOrReplace(name, registration);
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicyStore>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry { [name] = registration };
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;

                return null;
            }
        }

        /// <summary>
        /// Retrieves registration for requested named type
        /// </summary>
        /// <param name="type">Registration type</param>
        /// <param name="name">Registration name</param>
        /// <param name="create">Instruncts container if it should create registration if not found</param>
        /// <returns>Registration for requested named type or null if named type is not registered and 
        /// <see cref="create"/> is false</returns>
        public INamedType Registration(Type type, string name, bool create = false)
        {
            for (var container = this; null != container; container = container._parent)
            {
                IPolicyStore data;
                if (null == (data = container[type, name])) continue;

                return (INamedType)data;
            }

            if (!create) return null;

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
                        existing = existing is HashRegistry<string, IPolicyStore> registry
                                 ? new HashRegistry<string, IPolicyStore>(registry)
                                 : new HashRegistry<string, IPolicyStore>(LinkedRegistry.ListToHashCutoverPoint * 2,
                                                                                       (LinkedRegistry)existing);

                        _registrations.Entries[i].Value = existing;
                    }

                    return (INamedType)existing.GetOrAdd(name, () => CreateRegistration(type, name));
                }

                if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                {
                    _registrations = new HashRegistry<Type, IRegistry<string, IPolicyStore>>(_registrations);
                    targetBucket = hashCode % _registrations.Buckets.Length;
                }

                var registration = CreateRegistration(type, name);
                _registrations.Entries[_registrations.Count].HashCode = hashCode;
                _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                _registrations.Entries[_registrations.Count].Key = type;
                _registrations.Entries[_registrations.Count].Value = new LinkedRegistry { [name] = registration };
                _registrations.Buckets[targetBucket] = _registrations.Count;
                _registrations.Count++;

                return (INamedType)registration;
            }
        }



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

            foreach (var registration in container[type]?.Values.OfType<IContainerRegistration>()
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

            foreach (var registration in container[type]?.Values.OfType<IContainerRegistration>()
                                      ?? Enumerable.Empty<IContainerRegistration>())
            {
                set.Add(registration.Name);
            }

            var generic = type.GetTypeInfo().IsGenericType ? type.GetGenericTypeDefinition() : type;

            if (generic != type)
            {
                foreach (var registration in container[generic]?.Values.OfType<IContainerRegistration>()
                                          ?? Enumerable.Empty<IContainerRegistration>())
                {
                    set.Add(registration.Name);
                }
            }

            return set;
        }

        #endregion


        #region PolicyListProxy


        private class PolicyListProxy : IPolicyList
        {
            private readonly IPolicyList _policies;
            private readonly IPolicyStore _registration;
            private readonly Type _type;
            private readonly string _name;

            public PolicyListProxy(IPolicyList policies, IPolicyStore registration)
            {
                _policies = policies;
                _registration = registration;
                if (registration is INamedType namedType)
                {
                    _type = namedType.Type;
                    _name = namedType.Name;
                }
            }

            public IBuilderPolicy Get(Type type, string name, Type policyInterface, out IPolicyList list)
            {
                if (_type != type || _name != name)
                {
                    return _policies.Get(type, name, policyInterface, out list);
                }

                list = _policies;
                return _registration.Get(policyInterface);
            }

            public void Set(Type type, string name, Type policyInterface, IBuilderPolicy policy)
            {
                if (_type != type || _name != name)
                    _policies.Set(type, name, policyInterface, policy);
                else
                    _registration.Set(policyInterface, policy);
            }

            public void Clear(Type type, string name, Type policyInterface)
            {
                if (_type != type || _name != name)
                    _policies.Clear(type, name, policyInterface);
                else
                    _registration.Clear(policyInterface);
            }

            public void ClearAll()
            {
            }
        }

        #endregion
    }
}
