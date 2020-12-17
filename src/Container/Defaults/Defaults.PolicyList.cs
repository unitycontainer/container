using System;
using Unity.Extension;

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

                position = Meta[position].Location;
            }

            return null;
        }

        ///<inheritdoc/>
        public void Set(Type? target, Type type, object value)
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
                ref var entry = ref Data[Count];
                entry = new Policy(hash, target, type, value);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }

        public void Set(Type? target, Type type, object value, PolicyChangeNotificationHandler subscriber)
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
                        candidate.Value = value;
                        candidate.PolicyChanged += subscriber;
                        candidate.Handler!.Invoke(value);
                        return;
                    }

                    position = Meta[position].Location;
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
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }

        public T CompareExchange<T>(Type? target, T value, T comparand, PolicyChangeNotificationHandler? subscriber = null)
            where T : class
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ typeof(T).GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Type, typeof(T)))
                    {
                        // Found existing
                        if (ReferenceEquals(comparand, candidate.Value))
                        { 
                            candidate.Value = value;
                            candidate.Handler?.Invoke(value);
                        }

                        if (null != subscriber) candidate.PolicyChanged += subscriber;
                        return (T)candidate.Value!;
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
                entry = new Policy(hash, target, typeof(T), value);
                entry.PolicyChanged += subscriber;

                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;

                return value;
            }
        }

        public T GetOrAdd<T>(Type? target, T value, PolicyChangeNotificationHandler subscriber)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ typeof(T).GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Type, typeof(T)))
                    {
                        // Found existing
                        if (candidate.Value is null)
                        {
                            candidate.Value = value;
                            candidate.Handler?.Invoke(value!);
                        }

                        candidate.PolicyChanged += subscriber;
                        return (T)candidate.Value!;
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
                entry = new Policy(hash, target, typeof(T), value);
                entry.PolicyChanged += subscriber;

                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;

                return value;
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

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, target, type, null);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
                return Count;
            }
        }

        public bool Contains(Type? target, Type type)
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
                    return true;
                }

                position = Meta[position].Location;
            }

            return false;
        }

    }
}
