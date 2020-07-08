using System;
using Unity.Policy;

namespace Unity.Container
{
    public partial class Defaults : IPolicySet
    {
        ///<inheritdoc/>
        public void Clear(Type type)
        {
            lock (_syncRoot) { base.Clear(null, type); }
            DefaultPolicyChanged?.Invoke(type, null);
        }

        ///<inheritdoc/>
        public object? Get(Type type) => base.Get(null, type);

        ///<inheritdoc/>
        public void Set(Type policy, object instance)
        {
            try
            {
                lock (_syncRoot)
                {
                    var hashCode = policy.GetHashCode();
                    var targetBucket = hashCode % _metadata.Length;
                    for (var i = _metadata[targetBucket].Position; i > 0; i = _metadata[i].Next)
                    {
                        ref var candidate = ref _policies[i];
                        if (candidate.Type != null || candidate.Policy != policy) continue;

                        // Change existing and report
                        candidate.Value = instance;

                        return;
                    }

                    // Expand if required
                    if (_count >= _max)
                    {
                        Expand();
                        targetBucket = hashCode % _metadata.Length;
                    }

                    ref var entry = ref _policies[++_count];
                    entry.HashCode = hashCode;
                    entry.Value = instance;
                    entry.Policy = policy;

                    _metadata[_count].Next = _metadata[targetBucket].Position;
                    _metadata[targetBucket].Position = _count;

                }
            }
            finally
            {
                DefaultPolicyChanged?.Invoke(policy, instance);
            }
        }

        ///<inheritdoc/>
        public override void Set(Type? type, Type policy, object instance)
        {
            try
            {
                lock (_syncRoot)
                {
                    var hashCode = HashCode(type, policy);
                    var targetBucket = hashCode % _metadata.Length;
                    for (var i = _metadata[targetBucket].Position; i > 0; i = _metadata[i].Next)
                    {
                        ref var candidate = ref _policies[i];
                        if (candidate.Type != type || candidate.Policy != policy) continue;

                        // Change existing and report
                        candidate.Value = instance;

                        return;
                    }

                    // Expand if required
                    if (_count >= _max)
                    {
                        Expand();
                        targetBucket = hashCode % _metadata.Length;
                    }

                    ref var entry = ref _policies[++_count];
                    entry.HashCode = hashCode;
                    entry.Type = type;
                    entry.Value = instance;
                    entry.Policy = policy;

                    _metadata[_count].Next = _metadata[targetBucket].Position;
                    _metadata[targetBucket].Position = _count;

                }
            }
            finally
            {
                if (null == type && null != DefaultPolicyChanged) DefaultPolicyChanged.Invoke(policy, instance);
            }
        }
    }
}
