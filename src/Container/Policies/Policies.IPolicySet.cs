namespace Unity.Container
{
    public partial class Policies
    {
        ///<inheritdoc/>
        public void Clear(Type type)
        {
            var meta = Meta;
            var hash = (uint)(37 ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.Value = null;
                    return;
                }

                position = Meta[position].Location;
            }
        }


        ///<inheritdoc/>
        public object? Get(Type type)
        {
            var meta = Meta;
            var hash = (uint)(37 ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = Meta[position].Location;
            }

            return null;
        }


        ///<inheritdoc/>
        public void Set(Type type, object value)
        {
            var hash = (uint)(37 ^ type.GetHashCode());

            lock (SyncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                    {
                        // Found existing
                        candidate.Value = value;
                        return;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new
                Data[Count] = new Policy(hash, type, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }
    }
}
