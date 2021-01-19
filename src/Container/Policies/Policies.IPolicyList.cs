using System;

namespace Unity.Container
{
    public partial class Policies<TContext>
    {
        ///<inheritdoc/>
        public void Clear(Type? target, Type type)
        {
            var meta = Meta;
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
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
            var meta = Meta;
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var position = meta[hash % meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
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

            lock (SyncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Type, type))
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
                Data[Count] = new Policy(hash, target, type, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }
    }
}
