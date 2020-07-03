using System;
using System.Diagnostics;

namespace Unity.Storage
{
    public class QuickSet<TValue>
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
        private const int HashMask = unchecked((int)(uint.MaxValue >> 1));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const float LoadFactor = 0.72f;

        #endregion


        #region Fields

        private int _max;
        private int _count;
        private int _prime = 3;
        private Entry[]    _entries;
        private Metadata[] _metadata;

        #endregion


        #region Constructors

        public QuickSet()
        {
            var size = Prime.Numbers[_prime];
            _entries  = new Entry[size];
            _metadata = new Metadata[size];
            _max = Math.Min(50, (int)(size * LoadFactor));
        }

        #endregion


        #region Public Methods

        public bool Add(TValue value)
        {
            var hashCode = value?.GetHashCode() ?? 0;
            var targetBucket = (hashCode & HashMask) % _entries.Length;
            for (var i = _metadata[targetBucket].Bucket; i > 0; i = _metadata[i].Next)
            {
                ref var candidate = ref _entries[i];

                if (candidate.HashCode != hashCode || !Equals(candidate.Value, value))
                    continue;

                // Already exists
                return false;
            }

            // Expand if required
            if (_count >= _max)
            {
                Expand();
                targetBucket = (hashCode & HashMask) % _entries.Length;
            }

            // Add registration
            ref var entry  = ref _entries[++_count];
            _metadata[_count].Next     = _metadata[targetBucket].Bucket;
            entry.Value    = value;
            entry.HashCode = hashCode;
            _metadata[targetBucket].Bucket = _count;

            return true;
        }

        public bool Add(TValue value, int hashCode)
        {
            var targetBucket = (hashCode & HashMask) % _entries.Length;
            for (var i = _metadata[targetBucket].Bucket; i > 0; i = _metadata[i].Next)
            {
                ref var candidate = ref _entries[i];
                if (candidate.HashCode != hashCode || !Equals(candidate.Value, value))
                    continue;

                // Already exists
                return false;
            }

            // Expand if required
            if (_count >= _max)
            {
                Expand();
                targetBucket = (hashCode & HashMask) % _entries.Length;
            }

            // Add registration
            ref var entry = ref _entries[++_count];
            _metadata[_count].Next = _metadata[targetBucket].Bucket;
            entry.Value = value;
            entry.HashCode = hashCode;
            _metadata[targetBucket].Bucket = _count;

            return true;
        }

        #endregion


        #region Implementation

        private void Expand()
        {
            var size = Prime.Numbers[++_prime];
            _max = Math.Min(50, (int)(size * LoadFactor));

            Array.Resize(ref _entries, size);
            _metadata = new Metadata[size];

            for (var index = 1; index <= _count; index++)
            {
                var offset = (_entries[index].HashCode & HashMask) % size;
                ref var bucket = ref _metadata[offset].Bucket;

                _metadata[index].Next = bucket;
                bucket = index;
            }
        }

        #endregion


        #region Entry Type

        [DebuggerDisplay("{ Value,nq }")]
        public struct Entry
        {
            public int HashCode;
            public TValue Value;
        }

        #endregion
    }
}
