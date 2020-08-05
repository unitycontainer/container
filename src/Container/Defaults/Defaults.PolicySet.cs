using System;
using Unity.Policy;

namespace Unity.Container
{
    public partial class Defaults : IPolicySet
    {
        ///<inheritdoc/>
        public void Clear(Type type) => throw new NotSupportedException();

        ///<inheritdoc/>
        public object? Get(Type type)
        {
            var hash = (uint)(37 ^ type.GetHashCode());
            var position = _meta[hash % _meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (null == candidate.Target && candidate.Type == type)
                {
                    // Found existing
                    return candidate.Value;
                }

                position = _meta[position].Next;
            }

            return null;
        }

        ///<inheritdoc/>
        public void Set(Type type, object value)
        {
            try
            {
                var hash = (uint)(37 ^ type.GetHashCode());

                lock (_syncRoot)
                {
                    ref var bucket = ref _meta[hash % _meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (null == candidate.Target && candidate.Type == type)
                        {
                            // Found existing
                            candidate.Value = value;
                            return;
                        }

                        position = _meta[position].Next;
                    }

                    if (++_count >= _data.Length)
                    {
                        Expand();
                        bucket = ref _meta[hash % _meta.Length];
                    }

                    // Add new
                    _data[_count] = new Policy(hash, type, value);
                    _meta[_count].Next = bucket.Position;
                    bucket.Position = _count;
                }
            }
            finally
            {
                DefaultPolicyChanged?.Invoke(type, value);
            }
        }
    }
}
