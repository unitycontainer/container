using System;
using Unity.Policy;

namespace Unity.Container
{
    public partial class Policies<TProcessor, TStage> : IPolicyList
    {
        public void Clear(Type type, Type policyInterface)
        {
            var hashCode = (type.GetHashCode() + 37) ^ (policyInterface.GetHashCode() + 17) & HashMask;
            var targetBucket = hashCode % _policies.Length;
            for (var i = _metadata[targetBucket].Bucket; i > 0; i = _metadata[i].Next)
            {
                ref var candidate = ref _policies[i];

                if (candidate.Type != null ||
                    candidate.Policy != policyInterface)
                    continue;

                candidate.Value = null;
            }
        }

        public object? Get(Type type, Type policyInterface)
        {
            var hashCode = (type.GetHashCode() + 37) ^ (policyInterface.GetHashCode() + 17) & HashMask;
            var targetBucket = hashCode % _policies.Length;
            for (var i = _metadata[targetBucket].Bucket; i > 0; i = _metadata[i].Next)
            {
                ref var candidate = ref _policies[i];

                if (candidate.Type != null ||
                    candidate.Policy != policyInterface)
                    continue;

                return candidate.Value;
            }

            return null;
        }

        public void Set(Type type, Type policyInterface, object value)
        {
            var hashCode = (type.GetHashCode() + 37) ^ (policyInterface.GetHashCode() + 17) & HashMask;
            lock (_policies)
            {
                var targetBucket = hashCode % _policies.Length;
                for (var i = _metadata[targetBucket].Bucket; i > 0; i = _metadata[i].Next)
                {
                    ref var candidate = ref _policies[i];

                    if (candidate.Type != null ||
                        candidate.Policy != policyInterface)
                        continue;

                    candidate.Value = value;
                }

                // Expand if required
                if (_count >= _max)
                {
                    Expand();
                    targetBucket = hashCode % _policies.Length;
                }

                ref var entry = ref _policies[++_count];
                entry.HashCode = hashCode;
                entry.Type = null;
                entry.Value = value;
                entry.Policy = policyInterface;
                _metadata[_count].Next = _metadata[targetBucket].Bucket;
                _metadata[targetBucket].Bucket = _count;
            }
        }
    }
}
