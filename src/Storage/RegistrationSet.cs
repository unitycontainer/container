using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Utility;

namespace Unity.Storage
{
    public class RegistrationSet : IEnumerator<IContainerRegistration>
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int InitialCapacity = 71;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int[] _buckets;

        private Entry[] _entries;

        private int _index = -1;
        private IContainerRegistration? _current = null;

        #endregion


        #region Constructors

        public RegistrationSet()
        {
            _buckets = new int[InitialCapacity];
            _entries = new Entry[InitialCapacity];
        }

        #endregion




        #region IEnumerator

        object IEnumerator.Current => Current;

        public IContainerRegistration Current => _current;

        public bool MoveNext()
        {
            //Avoids going beyond the end of the collection.
            if (++_index >= Count) return false;

            // Set current box to next item in collection.
            _current = new ContainerRegistration(_entries[_index].Type, _entries[_index].Registration);

            return true;
        }

        public void Reset()
        {
            _current = null;
            _index = -1;
        }

        public void Dispose() { }

        #endregion


        #region Public Members

        public int Count { get; private set; }

        public void Add(Type type, ExplicitRegistration registration)
        {
            var key = new HashKey(type, registration.Name);
            var bucket = key.HashCode % _buckets.Length;
            var collisionCount = 0;

            for (int i = _buckets[bucket]; --i >= 0; i = _entries[i].Next)
            {
                ref var entry = ref _entries[i];
                if (entry.Key == key)
                {
                    entry.Registration = registration;
                    return;
                }
                collisionCount++;
            }

            if (Count == _entries.Length || 6 < collisionCount)
            {
                Expand();
                bucket = key.HashCode % _buckets.Length;
            }

            ref var newEntry = ref _entries[Count++];

            newEntry.Key = key;
            newEntry.Type = type;
            newEntry.Registration = registration;
            newEntry.Next = _buckets[bucket] - 1;
            _buckets[bucket] = Count;
        }

        #endregion


        #region Implementation

        private void Expand()
        {
            int newSize = HashHelpers.ExpandPrime(Count * 2);

            var newSlots = new Entry[newSize];
            Array.Copy(_entries, newSlots, Count);

            var newBuckets = new int[newSize];
            for (var i = 0; i < Count; i++)
            {
                var bucket = newSlots[i].Key.HashCode % newSize;
                newSlots[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i + 1;
            }

            _entries = newSlots;
            _buckets = newBuckets;
        }

        #endregion


        #region Nested Types

        public struct Entry
        {
            public int Next;
            public Type Type;
            public HashKey Key;
            public ExplicitRegistration Registration;
        }

        #endregion
    }
}
