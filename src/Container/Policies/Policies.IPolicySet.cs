namespace Unity.Container
{
    public partial class Policies
    {
        ///<inheritdoc/>
        public void Clear(Type type)
        {
            var meta = _meta;
            var hash = (uint)(37 ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.Value = null;
                    return;
                }

                position = _meta[position].Location;
            }
        }


        ///<inheritdoc/>
        public object? Get(Type type)
        {
            var meta = _meta;
            var hash = (uint)(37 ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = _meta[position].Location;
            }

            return null;
        }


        ///<inheritdoc/>
        public void Set(Type type, object value)
        {
            var hash = (uint)(37 ^ type.GetHashCode());

            lock (_sync)
            {
                ref var bucket = ref _meta[hash % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref _data[position];
                    if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
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
                _data[_count] = new Entry(hash, type, value);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
            }
        }
    }
}
