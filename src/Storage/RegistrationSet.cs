using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Registration;
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

        public void Add(Type type, string name, ContainerRegistration registration)
        {
            var hashCode = (type, name).GetHashCode() & 0x7FFFFFFF;
            var bucket = hashCode % _buckets.Length;
            var collisionCount = 0;
            var index = _buckets[bucket];
            for (int i = index; --i >= 0; i = _entries[i].Next)
            {
                ref var entry = ref _entries[i];
                if (entry.HashCode == hashCode && entry.RegisteredType == type)
                {
                    entry.RegisteredType = type;
                    entry.Name = name;
                    entry.MappedToType = registration.MappedToType;
                    entry.LifetimeManager = registration.LifetimeManager;
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
            newEntry.RegisteredType = type;
            newEntry.Name = name;
            newEntry.MappedToType = registration.MappedToType;
            newEntry.LifetimeManager = registration.LifetimeManager;
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
                yield return _entries[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        #region Nested Types

        private struct Entry : IContainerRegistration
        {
            internal int HashCode;
            internal int Next;

            public Type RegisteredType { get; internal set; }

            public Type MappedToType { get; internal set; }

            public LifetimeManager LifetimeManager { get; internal set; }

            public string Name { get; internal set; }
        }

        #endregion
    }
}
