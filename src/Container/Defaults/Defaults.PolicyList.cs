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
            var position = Meta[hash % Meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) && 
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = Meta[position].Next;
            }

            return null;
        }

        ///<inheritdoc/>
        public void Set(Type? target, Type type, object value)
        {
            PolicyChangeNotificationHandler? handler = null;

            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());

            try
            {
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
                            candidate.Value = value;
                            handler = candidate.Handler;
                            return;
                        }

                        position = Meta[position].Next;
                    }

                    if (++Count >= Data.Length)
                    {
                        Expand();
                        bucket = ref Meta[hash % Meta.Length];
                    }

                    // Add new registration
                    ref var entry = ref Data[Count];
                    entry = new Policy(hash, target, type, value);
                    handler = entry.Handler;
                    Meta[Count].Next = bucket.Position;
                    bucket.Position = Count;
                }
            }
            finally
            {
                handler?.Invoke(value);
            }
        }

        public void Set(Type? target, Type type, object value, PolicyChangeNotificationHandler subscriber)
        {
            PolicyChangeNotificationHandler? handler = null;

            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());

            try
            {
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
                            candidate.Value = value;
                            candidate.PolicyChanged += subscriber;
                            handler = candidate.Handler;
                            return;
                        }

                        position = Meta[position].Next;
                    }

                    if (++Count >= Data.Length)
                    {
                        Expand();
                        bucket = ref Meta[hash % Meta.Length];
                    }

                    // Add new registration
                    ref var entry = ref Data[Count];
                    entry = new Policy(hash, target, type, value);
                    entry.PolicyChanged += subscriber;
                    Meta[Count].Next = bucket.Position;
                    bucket.Position = Count;
                }
            }
            finally
            {
                handler?.Invoke(value);
            }
        }


        /// <summary>
        /// Allocates placeholder
        /// </summary>
        /// <param name="target"><see cref="Type"/> of target</param>
        /// <param name="type"><see cref="Type"/> of policy</param>
        /// <returns></returns>
        private int Allocate(Type? target, Type type)
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
                        throw new InvalidOperationException($"Combination {target?.Name} - {type.Name} already allocated");

                    position = Meta[position].Next;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, target, type, null);
                Meta[Count].Next = bucket.Position;
                bucket.Position = Count;
                return Count;
            }
        }
    }
}
