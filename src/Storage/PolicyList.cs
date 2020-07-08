using System;
using System.Runtime.CompilerServices;
using Unity.Policy;

namespace Unity.Storage
{
    /// <summary>
    /// This class implements <see cref="IPolicyList"/> and manages policies
    /// </summary>
    public class PolicyList : IPolicyList
    {
        #region Constants

        public const int HashMask = unchecked((int)(uint.MaxValue >> 1));
        protected const float LoadFactor = 0.72f;

        #endregion


        #region Fields

        protected int _max;
        protected int _count;
        protected int _prime;
        protected Entry[]    _policies;
        protected Metadata[] _metadata;

        #endregion


        #region Constructors

        /// <summary>
        /// <see cref="PolicyList"/> constructor
        /// </summary>
        /// <param name="prime">Index of prime number to be used to initialize size of 
        /// the list. The index is referencing <see cref="Prime.Numbers"/> collection</param>
        public PolicyList(int prime = 3)
        {
            _prime = prime;
            var size = Prime.Numbers[_prime];
            _max = (int)(size * LoadFactor);
            _policies = new Entry[size];
            _metadata = new Metadata[size];
        }

        #endregion


        #region IPolicyList

        ///<inheritdoc/>
        public virtual void Clear(Type? type, Type policy)
        {
            for (var i = _metadata[HashCode(type, policy) % _metadata.Length].Position; i > 0; i = _metadata[i].Next)
            {
                ref var candidate = ref _policies[i];
                if (candidate.Type != type || candidate.Policy != policy) continue;

                candidate.Value = null;
                return;
            }
        }

        ///<inheritdoc/>
        public virtual object? Get(Type? type, Type policy)
        {
            for (var i = _metadata[HashCode(type, policy) % _metadata.Length].Position; i > 0; i = _metadata[i].Next)
            {
                ref var candidate = ref _policies[i];
                if (candidate.Type != type || candidate.Policy != policy) continue;

                return candidate.Value;
            }

            return null;
        }

        ///<inheritdoc/>
        public virtual void Set(Type? type, Type policy, object instance)
        {
            var hashCode = HashCode(type, policy);
            var targetBucket = hashCode % _metadata.Length;
            for (var i = _metadata[targetBucket].Position; i > 0; i = _metadata[i].Next)
            {
                ref var candidate = ref _policies[i];
                if (candidate.Type != type || candidate.Policy != policy) continue;

                candidate.Value = instance;
                return;
            }

            // Expand if required
            if (_count >= _max)
            {
                Expand();
                targetBucket = hashCode % _metadata.Length;
            }

            ref var entry  = ref _policies[++_count];
            entry.HashCode = hashCode;
            entry.Type   = type;
            entry.Value  = instance;
            entry.Policy = policy;

            _metadata[_count].Next = _metadata[targetBucket].Position;
            _metadata[targetBucket].Position = _count;
        }

        #endregion


        #region Implementation

        /// <summary>
        /// Method that calculates HashCode of the <see cref="Type"/> pair.
        /// </summary>
        /// <param name="type"><see cref="Type"/> this policy applies to</param>
        /// <param name="policy"><see cref="Type"/> of the policy</param>
        /// <returns>Hash code</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual int HashCode(Type? type, Type policy)
        {
            var hash = null == type
                ? policy.GetHashCode()
                : (7199369 * type.GetHashCode()) ^ policy.GetHashCode();

            return hash & HashMask;
        }

        /// <summary>
        /// This method expands <see cref="PolicyList"/> when it reaches capacity
        /// </summary>
        protected virtual void Expand()
        {
            var size = Prime.Numbers[++_prime];
            _max = (int)(size * LoadFactor);

            Array.Resize(ref _policies, size);
            var metadata = new Metadata[size];

            for (var index = 1; index <= _count; index++)
            {
                var offset = _policies[index].HashCode % size;
                ref var bucket = ref metadata[offset].Position;

                metadata[index].Next = bucket;
                bucket = index;
            }

            _metadata = metadata;
        }

        /// <summary>
        /// Internal structure holding policies
        /// </summary>
        protected struct Entry
        {
            public int HashCode;
            public Type? Type;
            public Type Policy;
            public object? Value;
        }

        #endregion
    }
}
