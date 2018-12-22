using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Utility;

namespace Unity.Storage
{
    [DebuggerDisplay("HashRegistry ({Count}) ")]
    internal class HashRegistry<TKey, TValue> : IRegistry<TKey, TValue>
    {
        #region Constants

        private const float LoadFactor = 0.72f;

        #endregion


        #region Fields

        public readonly int[] Buckets;
        public readonly Entry[] Entries;
        public int Count;

        #endregion


        #region Constructors

        public HashRegistry(int capacity)
        {
            var size = Prime.GetPrime(capacity);
            Buckets = new int[size];
            Entries = new Entry[size];

            for (var i = 0; i < Buckets.Length; i++) Buckets[i] = -1;
        }

        public HashRegistry(int capacity, LinkedNode<TKey, TValue> head)
        {
            var size = Prime.GetPrime(capacity);
            Buckets = new int[size];
            Entries = new Entry[size];

            for (var i = 0; i < Buckets.Length; i++) Buckets[i] = -1;
            for (var node = head; node != null; node = node.Next)
            {
                this[node.Key] = node.Value;
            }
        }

        public HashRegistry(HashRegistry<TKey, TValue> dictionary)
        {
            var size = Prime.GetPrime(dictionary.Entries.Length * 2);

            Buckets = new int[size];
            Entries = new Entry[size];
            for (var i = 0; i < Buckets.Length; i++) Buckets[i] = -1;

            Array.Copy(dictionary.Entries, 0, Entries, 0, dictionary.Count);
            for (var i = 0; i < dictionary.Count; i++)
            {
                var hashCode = Entries[i].HashCode;
                if (hashCode < 0) continue;

                var bucket = hashCode % size;
                Entries[i].Next = Buckets[bucket];
                Buckets[bucket] = i;
            }
            Count = dictionary.Count;
            dictionary.Count = 0;
        }

        #endregion


        #region IRegistry

        public TValue this[TKey key]
        {
            get
            {
                var hashCode = null == key ? 0 : key.GetHashCode() & 0x7FFFFFFF;
                for (var i = Buckets[hashCode % Buckets.Length]; i >= 0; i = Entries[i].Next)
                {
                    if (Entries[i].HashCode == hashCode && Equals(Entries[i].Key, key)) return Entries[i].Value;
                }

                return default(TValue);
            }

            set
            {
                var hashCode = null == key ? 0 : key.GetHashCode() & 0x7FFFFFFF;
                var targetBucket = hashCode % Buckets.Length;

                for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
                {
                    if (Entries[i].HashCode == hashCode && Equals(Entries[i].Key, key))
                    {
                        Entries[i].Value = value;
                        return;
                    }
                }

                Entries[Count].HashCode = hashCode;
                Entries[Count].Next = Buckets[targetBucket];
                Entries[Count].Key = key;
                Entries[Count].Value = value;
                Buckets[targetBucket] = Count;
                Count++;
            }
        }

        public bool RequireToGrow => (Entries.Length - Count) < 100 &&
                                     (float)Count / Entries.Length > LoadFactor;

        public IEnumerable<TKey> Keys
        {
            get
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return Entries[i].Key;
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return Entries[i].Value;
                }
            }
        }

        public TValue GetOrAdd(TKey key, Func<TValue> factory)
        {
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % Buckets.Length;
            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                ref var candidate = ref Entries[i];
                if (candidate.HashCode != hashCode || Equals(candidate.Key, key)) continue;

                return candidate.Value;
            }

            var value = factory();
            ref var entry = ref Entries[Count];
            Buckets[targetBucket] = Count++;

            entry.HashCode = hashCode;
            entry.Next = Buckets[targetBucket];
            entry.Key = key;
            entry.Value = value;

            return value;
        }

        public TValue SetOrReplace(TKey key, TValue value)
        {
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % Buckets.Length;
            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                ref var candidate = ref Entries[i];
                if (candidate.HashCode != hashCode || Equals(candidate.Key, key)) continue;

                var old = candidate.Value;
                candidate.Value = value;
                return old;
            }

            ref var entry = ref Entries[Count];
            entry.HashCode = hashCode;
            entry.Next = Buckets[targetBucket];
            entry.Key = key;
            entry.Value = value;
            Buckets[targetBucket] = Count++;

            return default(TValue);
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("Key='{Key}'   Value='{Value}'   Hash='{HashCode}'")]
        public struct Entry
        {
            public int HashCode;
            public int Next;
            public TKey Key;
            public TValue Value;
        }

        #endregion
    }
}
