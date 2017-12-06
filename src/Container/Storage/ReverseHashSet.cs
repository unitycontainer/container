using System;
using Unity.Utility;

namespace Unity.Container.Storage
{
    public class ReverseHashSet<T> 
    {
        #region Fields

        private int[] _buckets;
        private Slot[] _slots;
        private int _count;

        #endregion


        #region Constructors

        public ReverseHashSet()
        {
            _count = 0;
            _buckets = new int[37];
            _slots   = new Slot[37];
        }

        #endregion


        #region ReverseHashSet methods

        /// <summary>
        /// Add item to this HashSet. Later value replaces previosly set value
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            int hashCode = item?.GetHashCode() & 0x7FFFFFFF ?? 0 ;
            int bucket = hashCode % _buckets.Length;
            int collisionCount = 0;

            for (int i = _buckets[bucket]; --i >= 0; i = _slots[i].next)
            {
                if (_slots[i].hashCode == hashCode && Equals(_slots[i].value, item))
                {
                    _slots[i].value = item;
                    return;
                }
                collisionCount++;
            }

            if (_count == _slots.Length)
            {
                IncreaseCapacity();
                bucket = hashCode % _buckets.Length;
            }

            _slots[_count].hashCode = hashCode;
            _slots[_count].value = item;
            _slots[_count].next = _buckets[bucket];
            _count++;
            _buckets[bucket] = _count;
        }

        public void Clear()
        {
            for (int i = 0; i < _count; i++)
            {
                _buckets[i] = 0;
                _slots[_count].hashCode = 0;
                _slots[_count].value = default(T);
                _slots[_count].next = 0;
            }

            _count = 0;
        }

        #endregion


        #region Helper methods


        private void IncreaseCapacity()
        {
            int newSize = HashHelpers.ExpandPrime(_count * 2);

            Slot[] newSlots = new Slot[newSize];
            Array.Copy(_slots, newSlots, _count);

            int[] newBuckets = new int[newSize];
            for (int i = 0; i < _count; i++)
            {
                int bucket = newSlots[i].hashCode % newSize;
                newSlots[i].next = newBuckets[bucket];
                newBuckets[bucket] = i + 1;
            }

            _slots = newSlots;
            _buckets = newBuckets;
        }

        #endregion

        internal struct Slot
        {
            internal int hashCode;      // Lower 31 bits of hash code, 0 if unused
            internal T value;
            internal int next;          // Index of next entry, 0 if last
        }
    }

}
