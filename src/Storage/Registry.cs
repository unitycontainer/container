using System;
using System.Security;
using Unity.Policy;

namespace Unity.Storage
{
    [SecuritySafeCritical]
    public class Registry<TKey, TValue>
    {
        #region Fields

        private readonly int _prime;
        public readonly int[] Buckets;
        public readonly Entry[] Entries;
        public int Count;

        #endregion


        #region Constructors

        public Registry(int prime = 0)
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

        public Registry(Registry<TKey, TValue> registry)
            : this(registry._prime + 1)
        {
            Array.Copy(registry.Entries, 0, Entries, 0, registry.Count);
            for (var i = 0; i < registry.Count; i++)
            {
                var hashCode = Entries[i].HashCode;
                if (hashCode < 0) continue;

                var bucket = hashCode % Buckets.Length;
                Entries[i].Next = Buckets[bucket];
                Buckets[bucket] = i;
            }
            Count = registry.Count;
        }

        public Registry(TValue value)
            : this(0)
        {
            ref var entry = ref Entries[0];
            entry.Next = -1;
            entry.Value = value;

            Buckets[0] = 0;
            Count++;
        }

        #endregion


        #region Public Members

        public bool RequireToGrow => (Entries.Length - Count) < 100 && 
                                     (float)Count / Entries.Length > 0.72f;
        #endregion


        #region Nested Types

        public struct Entry
        {
            public int HashCode;
            public int Next;
            public TKey Key;
            public TValue Value;
        }

        #endregion


        #region Prime Numbers

        public static readonly int[] Primes = {
            37, 71, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597,
            1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
            25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751,
            225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        #endregion
    }
}
