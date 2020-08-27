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
            var position = _meta[hash % _meta.Length].Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return candidate.Value;
                }

                position = _meta[position].Next;
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
                    ref var bucket = ref _meta[hash % _meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
                        {
                            // Found existing
                            candidate.Value = value;
                            handler = candidate.Handler;
                            return;
                        }

                        position = _meta[position].Next;
                    }

                    if (++_count >= _data.Length)
                    {
                        Expand();
                        bucket = ref _meta[hash % _meta.Length];
                    }

                    // Add new
                    _data[_count] = new Policy(hash, type, value);
                    _meta[_count].Next = bucket.Position;
                    bucket.Position = _count;
                }
            }
            finally
            {
                DefaultPolicyChanged?.Invoke(type, value);
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
                    ref var bucket = ref _meta[hash % _meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
                        {
                            // Found existing
                            candidate.Value = value;
                            candidate.PolicyChanged += subscriber;
                            handler = candidate.Handler;
                            return;
                        }

                        position = _meta[position].Next;
                    }

                    if (++_count >= _data.Length)
                    {
                        Expand();
                        bucket = ref _meta[hash % _meta.Length];
                    }

                    // Add new entry and subscribe
                    ref var entry = ref _data[_count];
                    entry = new Policy(hash, type, value);
                    entry.PolicyChanged += subscriber;
                    handler = entry.Handler;

                    _meta[_count].Next = bucket.Position;
                    bucket.Position = _count;
                }
            }
            finally
            {
                DefaultPolicyChanged?.Invoke(type, value);
                handler?.Invoke(value);
            }
        }

        /// <summary>
        /// Allocates placeholder
        /// </summary>
        /// <param name="type"><see cref="Type"/> of policy</param>
        /// <returns></returns>
        private int Allocate(Type type)
        {
            var hash = (uint)(37 ^ type.GetHashCode());

            lock (_syncRoot)
            {
                ref var bucket = ref _meta[hash % _meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref _data[position];
                    if (null == candidate.Target && ReferenceEquals(candidate.Type, type))
                        throw new InvalidOperationException($"{type.Name} already allocated");

                    position = _meta[position].Next;
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new
                _data[_count] = new Policy(hash, type, null);
                _meta[_count].Next = bucket.Position;
                bucket.Position = _count;
                return _count;
            }
        }
    }
}
