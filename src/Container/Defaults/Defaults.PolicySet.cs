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
            var position = Meta[hash % Meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = Meta[position].Next;
            }

            return null;
        }

        ///<inheritdoc/>
        public void Set(Type type, object value)
        {
            PolicyChangeNotificationHandler? handler = null;

            try
            {
                var hash = (uint)(37 ^ type.GetHashCode());

                lock (_syncRoot)
                {
                    ref var bucket = ref Meta[hash % Meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position];
                        if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
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

                    // Add new
                    Data[Count] = new Policy(hash, type, value);
                    Meta[Count].Next = bucket.Position;
                    bucket.Position = Count;
                }
            }
            finally
            {
                handler?.Invoke(value);
            }
        }

        public void Set(Type type, object value, PolicyChangeNotificationHandler subscriber)
        {
            PolicyChangeNotificationHandler? handler = null;

            try
            {
                var hash = (uint)(37 ^ type.GetHashCode());

                lock (_syncRoot)
                {
                    ref var bucket = ref Meta[hash % Meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position];
                        if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
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

                    // Add new entry and subscribe
                    ref var entry = ref Data[Count];
                    entry = new Policy(hash, type, value);
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
        /// <param name="type"><see cref="Type"/> of policy</param>
        /// <returns>Position of the element</returns>
        private int Allocate(Type type)
        {
            var hash = (uint)(37 ^ type.GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
                        throw new InvalidOperationException($"{type.Name} already allocated");

                    position = Meta[position].Next;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new
                Data[Count] = new Policy(hash, type, null);
                Meta[Count].Next = bucket.Position;
                bucket.Position = Count;
                return Count;
            }
        }
    }
}
