using System;
using System.Runtime.CompilerServices;
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

            lock (_syncRoot)
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
                        candidate.Handler?.Invoke(value);
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

        public T GetOrAdd<T>(T value, PolicyChangeNotificationHandler subscriber)
            where T : class
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            var hash = (uint)(37 ^ typeof(T).GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (candidate.Target is null && ReferenceEquals(candidate.Type, typeof(T)))
                    {
                        if (candidate.Value is null)
                        {
                            candidate.Value = value;
                            candidate.Handler?.Invoke(value);
                        }

                        candidate.PolicyChanged += subscriber;
                        return (T)candidate.Value;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new
                ref var entry = ref Data[Count];
                entry = new Policy(hash, typeof(T), value);
                entry.PolicyChanged += subscriber;

                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;

                return value;
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
                    if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                        throw new InvalidOperationException($"{type.Name} already allocated");

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new
                Data[Count] = new Policy(hash, type, null);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
                return Count;
            }
        }
    }
}
