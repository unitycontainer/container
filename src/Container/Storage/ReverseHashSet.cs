using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Registration;
using Unity.Utility;

namespace Unity.Container.Storage
{
    internal class ReverseHashSet : IEnumerable<IContainerRegistration>
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
        public void Add(IContainerRegistration item)
        {
            var hashCode = item?.GetHashCode() & 0x7FFFFFFF ?? 0 ;
            var bucket = hashCode % _buckets.Length;
            var collisionCount = 0;

            for (int i = _buckets[bucket]; --i >= 0; i = _slots[i].Next)
            {
                if (_slots[i].HashCode == hashCode && Equals(_slots[i].Value, item))
                {
                    _slots[i].Value = item;
                    return;
                }
                collisionCount++;
            }

            if (_count == _slots.Length || 6 < collisionCount)
            {
                IncreaseCapacity();
                bucket = hashCode % _buckets.Length;
            }

            _slots[_count].HashCode = hashCode;
            _slots[_count].Value = item;
            _slots[_count].Next = _buckets[bucket];
            _count++;
            _buckets[bucket] = _count;
        }

        public void Clear()
        {
            for (var i = 0; i < _count; i++)
            {
                _buckets[i] = 0;
                _slots[_count].HashCode = 0;
                _slots[_count].Value = null;
                _slots[_count].Next = 0;
            }

            _count = 0;
        }

        public IEnumerator<IContainerRegistration> GetEnumerator()
        {
            for(var i = 0; i < _count; i++)
                yield return _slots[i].Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        #region Helper methods


        private void IncreaseCapacity()
        {
            int newSize = HashHelpers.ExpandPrime(_count * 2);

            var newSlots = new Slot[newSize];
            Array.Copy(_slots, newSlots, _count);

            var newBuckets = new int[newSize];
            for (var i = 0; i < _count; i++)
            {
                var bucket = newSlots[i].HashCode % newSize;
                newSlots[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i + 1;
            }

            _slots = newSlots;
            _buckets = newBuckets;
        }

        #endregion

        private struct Slot
        {
            internal int HashCode;      // Lower 31 bits of hash code, 0 if unused
            internal IContainerRegistration Value;
            internal int Next;          // Index of next entry, 0 if last
        }
    }

}
