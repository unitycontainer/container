using System;
using Unity.Policy;

namespace Unity.Container
{
    public partial class Defaults : IPolicyList
    {
        ///<inheritdoc/>
        public void Clear(Type? type, Type policy) => throw new NotSupportedException();

        ///<inheritdoc/>
        public object? Get(Type? target, Type type)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var position = _meta[hash % _meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (ReferenceEquals(candidate.Target, target) && 
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = _meta[position].Next;
            }

            return null;
        }

        ///<inheritdoc/>
        public void Set(Type? target, Type type, object value)
        {
            if (null == target)
            {
                Set(type, value);
                return;
            }

            var hash = (uint)((target.GetHashCode() + 37) ^ type.GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref _meta[hash % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref _data[position];
                    if (ReferenceEquals(candidate.Target, target) && 
                        ReferenceEquals(candidate.Type, type))
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

                // Add new registration
                _data[_count] = new Policy(hash, target, type, value);
                _meta[_count].Next = bucket.Position;
                bucket.Position = _count;
            }
        }


        private int Add(Type? target, Type type, object value)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref _meta[hash % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref _data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Type, type))
                    {
                        // Found existing
                        candidate.Value = value;
                        return position;
                    }

                    position = _meta[position].Next;
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new registration
                _data[_count] = new Policy(hash, target, type, value);
                _meta[_count].Next = bucket.Position;
                bucket.Position = _count;
            }

            return 0;
        }
    }
}
