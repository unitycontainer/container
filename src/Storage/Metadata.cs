using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.Utility;

namespace Unity.Storage
{
    [DebuggerDisplay("Metadata ({Count}) ")]
    internal class Metadata
    {
        #region Fields

        private int[] Buckets;
        private Entry[] Entries;
        private int Count;
        public static readonly int[] Empty = new int[0];

        #endregion


        #region Constructors

        public Metadata()
        {
            Buckets = new int[37];
            Entries = new Entry[37];

            HashHelpers.FillArray(Buckets, -1);
        }

        #endregion


        #region Public Members

        public void Add(Type key, int value)
        {
            // Check for existing
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % Buckets.Length;
            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                ref var candidate = ref Entries[i];
                if (candidate.HashCode != hashCode || !Equals(candidate.Key, key)) continue;

                if (candidate.Data.Length == candidate.Count)
                {
                    var source = candidate.Data;
                    candidate.Data = new int[2 * candidate.Data.Length];
                    Array.Copy(source, 0, candidate.Data, 0, candidate.Count);
                }

                candidate.Data[candidate.Count++] = value;
            }

            // Grow if required
            if ((Entries.Length - Count) < 100 && (float)Count / Entries.Length > 0.72f)
            {
                var entries = Entries;
                var size = HashHelpers.GetPrime(2 * Buckets.Length);

                Buckets = new int[size];
                Entries = new Entry[size];

                HashHelpers.FillArray(Buckets, -1);

                Array.Copy(entries, 0, Entries, 0, Count);
                for (var i = 0; i < Count; i++)
                {
                    var hash = Entries[i].HashCode;
                    if (hashCode < 0) continue;

                    var bucket = hash % Buckets.Length;
                    Entries[i].Next = Buckets[bucket];
                    Buckets[bucket] = i;
                }
            }

            // Add new entry
            ref var entry = ref Entries[Count];
            entry.Next = Buckets[targetBucket];
            entry.Key = key;
            entry.Count = 1;
            entry.Data = new int[] { value, -1 };
            entry.HashCode = hashCode;
            Buckets[targetBucket] = Count++;
        }

        // TODO: Performance
        public IEnumerable<int> Get(Type key)
        {
            var hashCode = (key?.GetHashCode() ?? 0) & 0x7FFFFFFF;
            var targetBucket = hashCode % Buckets.Length;
            for (var i = Buckets[targetBucket]; i >= 0; i = Entries[i].Next)
            {
                ref var candidate = ref Entries[i];
                if (candidate.HashCode != hashCode || !Equals(candidate.Key, key)) continue;

                var count = candidate.Count;
                var data = candidate.Data;

                return data.Take(count);
            }

            return Empty;
        }

        #endregion


        #region Nested Types

        [DebuggerDisplay("Type='{Key}'   Registrations='{Count}'")]
        public struct Entry
        {
            public int HashCode;
            public int Next;
            public Type Key;
            public int Count;
            public int[] Data;
        }

        #endregion
    }
}
