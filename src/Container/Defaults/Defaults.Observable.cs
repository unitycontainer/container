using System;
using Unity.Extension;

namespace Unity.Container
{
    public partial class Defaults : IPolicyObservable
    {
        public object? Get(Type? target, Type type, PolicyChangeHandler handler)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());

            ref var bucket = ref Meta[hash % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.PolicyChanged += handler;
                    return candidate.Value;
                }

                position = Meta[position].Location;
            }

            if (++Count >= Data.Length)
            {
                Expand();
                bucket = ref Meta[hash % Meta.Length];
            }

            // Allocate placeholder 
            ref var entry = ref Data[Count];
            entry = new Policy(hash, target, type, default);
            entry.PolicyChanged += handler;
            Meta[Count].Location = bucket.Position;
            bucket.Position = Count;
            return default;
        }

        public void Set(Type? target, Type type, object policy, PolicyChangeHandler handler)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());

            lock (_syncRoot)
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
                        if (candidate.Value is null) candidate.Value = policy;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, target, type, policy);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }
    }
}
