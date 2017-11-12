using System;
using Unity.Container.Registration;
using Unity.Container.Storage;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private readonly object _syncRoot = new object();
        private HashHybridRegistry<Type, IHybridRegistry<string, IContainerRegistration>> _registrations = 
            new HashHybridRegistry<Type, IHybridRegistry<string, IContainerRegistration>>(ContainerInitialCapacity);

        #endregion


        #region Registration

        private void Register(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            var registration = new TypeRegistration(typeFrom, typeTo, name, lifetimeManager, injectionMembers);
            this[registration.RegisteredType, registration.Name] = registration;
        }

        private void Register(Type mapType, string name, object instance, LifetimeManager lifetime)
        {
            var registration = new InstanceRegistration(mapType, name, instance, lifetime);
            this[registration.RegisteredType, registration.Name] = registration;
        }

        #endregion


        private IContainerRegistration this[Type type, string name]
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

                    return _registrations.Entries[i].Value[name];
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

                        _registrations.Entries[i].Value[name] = value;
                        return;
                    }

                    if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                    {
                        _registrations = new HashHybridRegistry<Type, IHybridRegistry<string, IContainerRegistration>>(_registrations);
                        targetBucket = hashCode % _registrations.Buckets.Length;
                    }

                    _registrations.Entries[_registrations.Count].HashCode = hashCode;
                    _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                    _registrations.Entries[_registrations.Count].Key = type;
                    _registrations.Entries[_registrations.Count].Value = new ListHybridRegistry(value);
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

                    return (_registrations.Entries[i].Value[name] as IIndexerOf<Type, IBuilderPolicy>)?[interfaceType];
                }

                return null;
            }
            set
            {
                var hashCode = (type?.GetHashCode() ?? 0) & 0x7FFFFFFF;
                var targetBucket = hashCode % _registrations.Buckets.Length;
                IContainerRegistration registration = null;
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

                        registration = _registrations.Entries[i].Value[name];
                        break;
                    }

                    if (null == registration)
                    {
                        if (_registrations.RequireToGrow || ListToHashCutoverPoint < collisions)
                        {
                            _registrations = new HashHybridRegistry<Type, IHybridRegistry<string, IContainerRegistration>>(_registrations);
                            targetBucket = hashCode % _registrations.Buckets.Length;
                        }

                        registration = new ContainerRegistration(type, name);
                        _registrations.Entries[_registrations.Count].HashCode = hashCode;
                        _registrations.Entries[_registrations.Count].Next = _registrations.Buckets[targetBucket];
                        _registrations.Entries[_registrations.Count].Key = type;
                        _registrations.Entries[_registrations.Count].Value = new ListHybridRegistry(registration);
                        _registrations.Buckets[targetBucket] = _registrations.Count;
                        _registrations.Count++;
                    }
                }

                lock (registration)
                {
                    ((IIndexerOf<Type, IBuilderPolicy>)registration)[interfaceType] = value;
                }
            }
        }

        #region Implementation

        private IHybridRegistry<string, IContainerRegistration> Resize(IHybridRegistry<string, IContainerRegistration> dictionary)
        {
            if (dictionary is ListHybridRegistry list)
            {
                var newDictionary = new HashHybridRegistry<string, IContainerRegistration>(11);
                for (var node = list.Head; null != node; node = node.Next)
                {
                    newDictionary[node.Key] = node.Value;
                }
                return newDictionary;
            }

            return new HashHybridRegistry<string, IContainerRegistration>((HashHybridRegistry<string, IContainerRegistration>)dictionary);
        }

        #endregion
    }
}
