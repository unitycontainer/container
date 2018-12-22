using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Utility;

namespace Unity.Storage
{
    public class RegistrationSet : IEnumerable<IContainerRegistration> 
    {
        #region Fields

        private const int InitialCapacity = 11;
        private int[] _buckets;
        private Entry[] _entries;
        private int Count;

        #endregion


        #region Constructors

        public RegistrationSet()
        {
            _buckets = new int[InitialCapacity];
            _entries = new Entry[InitialCapacity];
            for (var i = 0; i < _buckets.Length; i++) _buckets[i] = -1;
        }

        #endregion


        #region Public Members

        public void Add(Type type, string name, IPolicySet registration)
        {
            var hashCode = (type, name).GetHashCode() & 0x7FFFFFFF;
            var bucket = hashCode % _buckets.Length;
            var collisionCount = 0;
            var index = _buckets[bucket];
            for (int i = index; --i >= 0; i = _entries[i].Next)
            {
                ref var entry = ref _entries[i];
                if (entry.HashCode == hashCode && entry.Type == type)
                {
                    entry.Type = type;
                    entry.Name = name;
                    entry.Registration = registration;
                    return;
                }
                collisionCount++;
            }

            if (Count == _entries.Length || 6 < collisionCount)
            {
                IncreaseCapacity();
                bucket = hashCode % _buckets.Length;
            }

            ref var newEntry = ref _entries[Count];
            _buckets[bucket] = Count++;

            newEntry.HashCode = hashCode;
            newEntry.Type = type;
            newEntry.Name = name;
            newEntry.Registration = registration;
            newEntry.Next = index;
        }


        #endregion


        #region Implementation

        private void IncreaseCapacity()
        {
            int newSize = HashHelpers.ExpandPrime(Count * 2);

            var newSlots = new Entry[newSize];
            Array.Copy(_entries, newSlots, Count);

            var newBuckets = new int[newSize];
            for (var i = 0; i < Count; i++)
            {
                var bucket = newSlots[i].HashCode % newSize;
                newSlots[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i + 1;
            }

            _entries = newSlots;
            _buckets = newBuckets;
        }

        public IEnumerator<IContainerRegistration> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                var pilicySet = (Registration.ContainerRegistration)_entries[i].Registration;
                ContainerRegistration registration = new ContainerRegistration
                {
                    RegisteredType  = _entries[i].Type,
                    Name            = _entries[i].Name,
                    MappedToType    = pilicySet.MappedToType,
                    LifetimeManager = pilicySet.LifetimeManager,
                };

                yield return registration;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        #region Nested Types


        private struct ContainerRegistration : IContainerRegistration
        {
            public Type RegisteredType { get; internal set; }

            public string Name { get; internal set; }

            public Type MappedToType { get; internal set; }

            public LifetimeManager LifetimeManager { get; internal set; }
        }


        private struct Entry
        {
            public int HashCode;
            public int Next;
            public Type Type;
            public string Name;
            public IPolicySet Registration;
        }

        #endregion
    }
}
