using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Lifetime;
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
        private const int InitialCapacity = 71;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int[] _buckets;

        private Entry[] _entries;

        #endregion


        #region Constructors

        public RegistrationSet()
        {
            _buckets = new int[InitialCapacity];
            _entries = new Entry[InitialCapacity];
        }

        #endregion


        #region Public Members

        public ref Entry this[int index]
        {
            get => ref _entries[index];
        }

        public int Count { get; private set; }

        public void Add(Type type, string name, InternalRegistration registration)
        {
            var hashCode = (37 ^ (name?.GetHashCode() ?? 0 + 17)) & 0x7FFFFFFF; 
            var bucket = hashCode % _buckets.Length;
            var collisionCount = 0;
            for (int i = _buckets[bucket]; --i >= 0; i = _entries[i].Next)
            {
                ref var entry = ref _entries[i];
                if (entry.HashCode == hashCode && entry.Name == name)
                {
                    entry.RegisteredType = type;
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

            ref var newEntry = ref _entries[Count++];

            newEntry.HashCode = hashCode;
            newEntry.RegisteredType = type;
            newEntry.Name = name;
            newEntry.Registration = registration;
            newEntry.Next = _buckets[bucket];
            _buckets[bucket] = Count;
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

        [DebuggerDisplay("RegisteredType={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
        [DebuggerTypeProxy(typeof(EntryDebugProxy))]
        public struct Entry : IContainerRegistration
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            internal int Next;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            internal int HashCode;

            internal InternalRegistration Registration; 

            public Type RegisteredType { get; internal set; }

            public string Name { get; internal set; }

            public Type MappedToType => Registration is ContainerRegistration registration 
                ? registration.Type : null;

            public LifetimeManager LifetimeManager => Registration is ContainerRegistration registration 
                ? registration.LifetimeManager : null;

        }

        private class EntryDebugProxy
        {
            private readonly Entry _entry;

            public EntryDebugProxy(Entry entry)
            {
                _entry = entry;
            }

            public Type RegisteredType => _entry.RegisteredType;

            public string Name => _entry.Name;

            public Type MappedToType => _entry.MappedToType;

            public LifetimeManager LifetimeManager => _entry.LifetimeManager;

        }

        private class RegistrationSetDebugProxy 
        {
            public RegistrationSetDebugProxy(RegistrationSet set)
            {
                Registrations = set._entries
                             .Cast<IContainerRegistration>()
                             .Take(set.Count)
                             .ToArray();
            }

            public IContainerRegistration[] Registrations { get; }
        }

        #endregion
    }
}
