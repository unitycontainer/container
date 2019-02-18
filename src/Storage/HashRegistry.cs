using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using Unity.Policy;
using Unity.Utility;

namespace Unity.Storage
{
    [SecuritySafeCritical]
    [DebuggerDisplay("HashRegistry ({Count}) ")]
    internal class HashRegistry : IRegistry<string, IPolicySet>
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
            var size = HashHelpers.GetPrime(capacity);
            Buckets = new int[size];
            Entries = new Entry[size];

#if !NET40
            unsafe
            {
                fixed (int* bucketsPtr = Buckets)
                {
                    int* ptr = bucketsPtr;
                    var end = bucketsPtr + Buckets.Length;
                    while (ptr < end) *ptr++ = -1;
                }
            }
#else
            for(int i = 0; i < Buckets.Length; i++)
                Buckets[i] = -1;
#endif
        }

        public HashRegistry(int capacity, LinkedNode<string, IPolicySet> head)
            : this(capacity)
        {
            for (var node = head; node != null; node = node.Next)
            {
                this[node.Key] = node.Value;
            }
        }

        public HashRegistry(HashRegistry dictionary)
            : this(HashHelpers.GetPrime(dictionary.Entries.Length * 2))
        {
            Array.Copy(dictionary.Entries, 0, Entries, 0, dictionary.Count);
            for (var i = 0; i < dictionary.Count; i++)
            {
                var hashCode = Entries[i].HashCode;
                if (hashCode < 0) continue;

                var bucket = hashCode % Buckets.Length; 
                Entries[i].Next = Buckets[bucket];
                Buckets[bucket] = i;
            }
            Count = dictionary.Count;
            dictionary.Count = 0;
        }

        #endregion


        #region IRegistry

        public IPolicySet this[string key]
        {
            get
            {
                IPolicySet match = null;
                var hashCode = null == key ? 0 : key.GetHashCode() & 0x7FFFFFFF;
                for (var i = Buckets[hashCode % Buckets.Length]; i >= 0; i = Entries[i].Next)
                {
                    ref var entry = ref Entries[i];
                    if (entry.HashCode == hashCode && Equals(entry.Key, key)) return entry.Value;

                    // Cover all match
                    if (ReferenceEquals(entry.Key, UnityContainer.All))
                        match = entry.Value;
                }

                return match;
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

        public IEnumerable<string> Keys
        {
            get
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return Entries[i].Key;
                }
            }
        }

        public IEnumerable<IPolicySet> Values
        {
            get
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return Entries[i].Value;
                }
            }
        }

        public IPolicySet GetOrAdd(string key, Func<IPolicySet> factory)
        {
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % Buckets.Length;
            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                ref var candidate = ref Entries[i];
                if (candidate.HashCode != hashCode || !Equals(candidate.Key, key)) continue;

                return candidate.Value;
            }

            var value = factory();
            ref var entry = ref Entries[Count];

            entry.HashCode = hashCode;
            entry.Next = Buckets[targetBucket];
            entry.Key = key;
            entry.Value = value;
            Buckets[targetBucket] = Count++;

            return value;
        }

        public IPolicySet SetOrReplace(string key, IPolicySet value)
        {
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % Buckets.Length;
            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                ref var candidate = ref Entries[i];
                if (candidate.HashCode != hashCode || !Equals(candidate.Key, key)) continue;

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

            return null;
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("Key='{Key}'   Value='{Value}'   Hash='{HashCode}'")]
        public struct Entry
        {
            public int HashCode;
            public int Next;
            public string Key;
            public IPolicySet Value;
        }

        #endregion
    }
}
