using System;
using System.Security;

namespace Unity.Storage
{
    [SecuritySafeCritical]
    public class QuickSet<TValue>
    {
        #region Fields

        private int _prime;
        private int[] Buckets;
        private Entry[] Entries;
        public int Count { get; private set; }

        #endregion
        

        #region Constructors

        public QuickSet()
        {
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

        #endregion


        #region Public Methods

        public bool Add(int hashCode, TValue value)
        {
            var collisions = 0;
            var targetBucket = (hashCode & UnityContainer.HashMask) % Buckets.Length;

            // Check for the existing 
            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                ref var candidate = ref Entries[i];
                if (candidate.HashCode != hashCode || !Equals(candidate.Value, value))
                {
                    collisions++;
                    continue;
                }

                // Already exists
                return false;
            }

            // Expand if required
            if (Count >= Entries.Length || 3 < collisions)
            {
                Expand();
                targetBucket = (hashCode & UnityContainer.HashMask) % Buckets.Length;
            }

            // Add registration
            ref var entry = ref Entries[Count];
            entry.HashCode = hashCode;
            entry.Value = value;
            entry.Next = Buckets[targetBucket];
            Buckets[targetBucket] = Count++;

            return true;
        }

        #endregion


        #region Entry Type

        private struct Entry
        {
            public int HashCode;
            public TValue Value;
            public int Next;
        }

        #endregion


        #region Implementation

        private void Expand()
        {
            var entries = Entries;

            _prime += 1;

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
            Array.Copy(entries, 0, Entries, 0, Count);
            for (var i = 0; i < Count; i++)
            {
                var hashCode = Entries[i].HashCode & UnityContainer.HashMask;
                if (hashCode < 0) continue;

                var bucket = hashCode % Buckets.Length;
                Entries[i].Next = Buckets[bucket];
                Buckets[bucket] = i;
            }
        }

        public static readonly int[] Primes = {
            11, 37, 71, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 1103, 1327, 1597,
            1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
            25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 187751,
            225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369};

        #endregion
    }
}
