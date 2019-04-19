using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Unity.Resolution;

namespace Unity.Storage
{
    [SecuritySafeCritical]
    public class QuickSet
    {
        #region Fields

        private readonly int _prime;
        public readonly int[] Buckets;
        public readonly Entry[] Entries;
        public int Count;

        #endregion


        #region Constructors

        public QuickSet(int prime = 0)
        {
            if (prime < 0 || prime >= Primes.Length) throw new ArgumentException("Capacity Overflow");

            _prime = prime;

            var size = Primes[_prime];
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

        public QuickSet(QuickSet set)
            : this(set._prime + 1)
        {
            Array.Copy(set.Entries, 0, Entries, 0, set.Count);
            for (var i = 0; i < set.Count; i++)
            {
                var hashCode = Entries[i].HashCode;
                if (hashCode < 0) continue;

                var bucket = hashCode % Buckets.Length;
                Entries[i].Next = Buckets[bucket];
                Buckets[bucket] = i;
            }
            Count = set.Count;
        }

        #endregion


        #region Public Members

        public bool Add(int hashCode, Type type)
        {
            var targetBucket = hashCode % Buckets.Length;

            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                if (Entries[i].Type != type) continue;

                return false;
            }

            // Create new metadata entry
            ref var entry = ref Entries[Count];
            entry.Next = Buckets[targetBucket];
            entry.HashCode = hashCode;
            entry.Type = type;
            Buckets[targetBucket] = Count++;

            return true;
        }

        // TODO: Redo the verification
        public bool Add(Type type, string name)
        {
            var hashCode = NamedType.GetHashCode(type, name) & UnityContainer.HashMask;
            var targetBucket = hashCode % Buckets.Length;

            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                if (Entries[i].Type != type) continue;

                return false;
            }

            // Create new metadata entry
            ref var entry = ref Entries[Count];
            entry.Next = Buckets[targetBucket];
            entry.HashCode = hashCode;
            entry.Type = type;
            Buckets[targetBucket] = Count++;

            return true;
        }

        public bool RequireToGrow => (Entries.Length - Count) < 100 &&
                                     (float)Count / Entries.Length > 0.72f;
        #endregion


        #region Nested Types

        public struct Entry
        {
            public int HashCode;
            public int Next;
            public Type Type;
        }

        #endregion


        #region Prime Numbers

        public static readonly int[] Primes = {
            37, 107, 163, 239, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597,
            1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
            25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751,
            225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        #endregion
    }
}
