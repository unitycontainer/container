using System;
using System.CodeDom;
using Unity.Policy;

namespace Unity.Container
{
    public partial class Policies<TProcessor, TStage> : IPolicySet
    {
        public void Clear(Type policyInterface)
        {
            var hashCode = policyInterface.GetHashCode() & HashMask;
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

        public object? Get(Type policyInterface)
        {
            var hashCode = policyInterface.GetHashCode() & HashMask;
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

        public void Set(Type policyInterface, object value)
        {
            var hashCode = policyInterface.GetHashCode() & HashMask;
            lock (_policies)
            {
                var targetBucket = hashCode % _policies.Length;
                for (var i = _metadata[targetBucket].Bucket; i > 0; i = _metadata[i].Next)
                {
                    ref var candidate = ref _policies[i];

                    if (candidate.Type   != null ||
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

                ref var entry  = ref _policies[++_count];
                entry.HashCode = hashCode;
                entry.Type     = null;
                entry.Value    = value;
                entry.Policy   = policyInterface;
                _metadata[_count].Next = _metadata[targetBucket].Bucket;
                _metadata[targetBucket].Bucket = _count;
            }

            // Defaults Shortcuts
            //switch (policyInterface)
            //{
            //    case Type type 
            //    when type == typeof(Type):
            //        break;

            //    case Type type 
            //    when type == typeof(string):
            //        break;
            //}
        }
    }
}
