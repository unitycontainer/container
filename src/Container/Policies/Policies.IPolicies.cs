using Unity.Policy;

namespace Unity.Container
{
    public partial class Policies : IPolicies
    {
        #region Get

        ///<inheritdoc/>
        public object? Get(Type type, PolicyChangeHandler handler)
        {
            var meta = _meta;
            var hash = (uint)(37 ^ type.GetHashCode());
            ref var bucket = ref meta[hash % meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.PolicyChanged += handler;
                    return candidate.Value;
                }

                position = meta[position].Location;
            }


            var length = _count;

            lock (_sync)
            {
                // Repeat search if modified
                if (length != _count)
                {
                    bucket = ref _meta[hash % _meta.Length];
                    position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                        {
                            // Found existing
                            candidate.PolicyChanged += handler;
                            return candidate.Value;
                        }

                        position = _meta[position].Location;
                    }
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new registration
                _data[_count] = new Entry(hash, type, (object?)default, handler);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
                return default;
            }
        }


        ///<inheritdoc/>
        public object? Get(Type? target, Type type, PolicyChangeHandler handler)
        {
                var meta = _meta;
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            ref var bucket = ref meta[hash % _meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.PolicyChanged += handler;
                    return candidate.Value;
                }

                position = meta[position].Location;
            }

            var length = _count;

            lock (_sync)
            {
                // Repeat search if modified
                if (length != _count)
                {
                    bucket = ref _meta[hash % _meta.Length];
                    position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (ReferenceEquals(candidate.Target, target) &&
                            ReferenceEquals(candidate.Type, type))
                        {
                            // Found existing
                            candidate.PolicyChanged += handler;
                            return candidate.Value;
                        }

                        position = _meta[position].Location;
                    }
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new registration
                _data[_count] = new Entry(hash, target, type, default, handler);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
                return default;
            }
        }

        #endregion

        
        #region Set

        ///<inheritdoc/>
        public void Set(Type type, object? policy, PolicyChangeHandler handler)
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
                        candidate.Value = policy;
                        candidate.PolicyChanged += handler;
                    }

                    position = _meta[position].Location;
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new registration
                _data[_count] = new Entry(hash, type, policy, handler);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
            }
        }


        ///<inheritdoc/>
        public void Set(Type? target, Type type, object? policy, PolicyChangeHandler handler)
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
                        candidate.Value = policy;
                        candidate.PolicyChanged += handler;
                        return;
                    }

                    position = _meta[position].Location;
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new registration
                _data[_count] = new Entry(hash, target, type, policy, handler);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
            }
        }

        #endregion


        #region Compare Exchange

        ///<inheritdoc/>
        public TPolicy? CompareExchange<TPolicy>(Type? target, TPolicy policy, TPolicy? comparand) 
            where TPolicy : class
        {
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ typeof(TPolicy).GetHashCode());
                var meta = _meta;
            ref var bucket = ref meta[hash % _meta.Length];
                var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, typeof(TPolicy)))
                {
                    // Found existing
                    return (TPolicy?)candidate.CompareExchange(policy, comparand);
                }

                position = meta[position].Location;
            }

            if (comparand is not null) return default;

            var length = _count;

            lock (_sync)
            {
                // Repeat search if modified
                if (length != _count)
                {
                    bucket = ref _meta[hash % _meta.Length];
                    position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (ReferenceEquals(candidate.Target, target) &&
                            ReferenceEquals(candidate.Type, typeof(TPolicy)))
                        {
                            // Found existing
                            return (TPolicy?)candidate.CompareExchange(policy, comparand);
                        }

                        position = _meta[position].Location;
                    }
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new registration
                _data[_count] = new Entry(hash, target, typeof(TPolicy), policy);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
                return default;
            }
        }


        ///<inheritdoc/>
        public TPolicy? CompareExchange<TPolicy>(Type? target, TPolicy policy, TPolicy? comparand, PolicyChangeHandler handler)
            where TPolicy : class
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ typeof(TPolicy).GetHashCode());
            var meta = _meta;
            ref var bucket = ref meta[hash % _meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref _data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, typeof(TPolicy)))
                {
                    // Found existing
                    var value = candidate.CompareExchange(policy, comparand);
                    candidate.PolicyChanged += handler;
                    return (TPolicy?)value;
                }

                position = meta[position].Location;
            }

            if (comparand is not null) return default;

            var length = _count;

            lock (_sync)
            {
                // Repeat search if modified
                if (length != _count)
                {
                    bucket = ref _meta[hash % _meta.Length];
                    position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (ReferenceEquals(candidate.Target, target) &&
                            ReferenceEquals(candidate.Type, typeof(TPolicy)))
                        {
                            // Found existing
                            var value = candidate.CompareExchange(policy, comparand);
                            candidate.PolicyChanged += handler;
                            return (TPolicy?)value;
                        }

                        position = _meta[position].Location;
                    }
                }

                if (++_count >= _data.Length)
                {
                    Expand();
                    bucket = ref _meta[hash % _meta.Length];
                }

                // Add new registration
                _data[_count] = new Entry(hash, target, typeof(TPolicy), policy, handler);
                _meta[_count].Location = bucket.Position;
                bucket.Position = _count;
                return default;
            }
        }

        #endregion
    }
}
