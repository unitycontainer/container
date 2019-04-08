using System;
using Unity.Policy;
using Unity.Resolution;
using Unity.Utility;

namespace Unity.Storage
{
    public class Registry
    {
        #region Fields

        public readonly int[] Buckets;
        public readonly Entry[] Entries;
        public int Count;

        #endregion


        #region Constructors

        public Registry(int capacity = 37)
        {
            var size = HashHelpers.GetPrime(capacity);

            Buckets = new int[size];
            Entries = new Entry[size];

            HashHelpers.FillArray(Buckets, -1);
        }

        public Registry(Registry registry)
            : this(HashHelpers.GetPrime(registry.Entries.Length * 2))
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
            registry.Count = 0;
        }

        #endregion


        public bool RequireToGrow => (Entries.Length - Count) < 100 &&
                                     (float)Count / Entries.Length > 0.72f;


        #region Nested Types

        public struct Entry
        {
            public int HashCode;
            public NamedType Key;
            public int Next;
            public IPolicySet Reference;
        }

        #endregion
    }
}
