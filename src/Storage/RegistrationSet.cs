using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Registration;
using Unity.Utility;

namespace Unity.Storage
{
    [DebuggerDisplay("IEnumerable<IContainerRegistration> ({Count}) ")]
    [DebuggerTypeProxy(typeof(RegistrationSetDebugProxy))]
    public class RegistrationSet : IEnumerable<IContainerRegistration> 
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int InitialCapacity = 11;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
            var hashCode = GetHashCode(type, name);
            var bucket = hashCode % _buckets.Length;
            var collisionCount = 0;
            for (var i = _buckets[bucket]; i >= 0; i = _entries[i].Next)
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
            newEntry.Next = _buckets[bucket];
        }

        #endregion


        #region Implementation

        public int GetHashCode(Type type, string name = null)
        {
            return ((type?.GetHashCode() ?? 0 + 37) ^ (name?.GetHashCode() ?? 0 + 17)) & 0x7FFFFFFF; ;
        }

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

        [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
        private struct Entry : IContainerRegistration
        {
            internal int HashCode;
            internal int Next;

            public Type RegisteredType { get; internal set; }

            public Type MappedToType { get; internal set; }

            public LifetimeManager LifetimeManager { get; internal set; }

            public string Name { get; internal set; }
        }

        private class RegistrationSetDebugProxy  
        {
            private readonly RegistrationSet _set;

            public RegistrationSetDebugProxy(RegistrationSet set)
            {
                _set = set;
            }

            public int Count => _set.Count;

            public IContainerRegistration[] Entries => _set._entries
                                                           .Cast<IContainerRegistration>()
                                                           .Take(_set.Count)
                                                           .ToArray();
        }


        #endregion
    }
}
