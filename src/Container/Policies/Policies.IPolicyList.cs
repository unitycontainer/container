namespace Unity.Container
{
    public partial class Policies
    {
        ///<inheritdoc/>
        public void Clear(Type? target, Type type)
        {
            var meta = _meta;
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.Value = null;
                    return;
                }

                position = meta[position].Location;
            }
        }


        ///<inheritdoc/>
        public object? Get(Type? target, Type type)
        {
            var meta = _meta;
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (ReferenceEquals(candidate.Target, target) && 
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = meta[position].Location;
            }

            return null;
        }


        ///<inheritdoc/>
        public void Set(Type? target, Type type, object value)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());

            lock (_sync)
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

                    position = _meta[position].Location;
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new 
                _data[_count] = new Entry(hash, target, type, value);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
            }
        }
    }
}
