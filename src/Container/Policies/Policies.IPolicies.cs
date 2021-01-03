using System;
using Unity.Extension;

namespace Unity.Container
{
    public partial class Policies<TContext> : IPolicies
    {
        #region Get

        ///<inheritdoc/>
        public object? Get(Type type, PolicyChangeHandler handler)
        {
            var meta = Meta;
            var hash = (uint)(37 ^ type.GetHashCode());
            ref var bucket = ref meta[hash % meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    candidate.PolicyChanged += handler;
                    return candidate.Value;
                }

                position = meta[position].Location;
            }


            var length = Count;

            lock (SyncRoot)
            {
                // Repeat search if modified
                if (length != Count)
                {
                    bucket = ref Meta[hash % Meta.Length];
                    position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position];
                        if (candidate.Target is null && ReferenceEquals(candidate.Type, type))
                        {
                            // Found existing
                            candidate.PolicyChanged += handler;
                            return candidate.Value;
                        }

                        position = Meta[position].Location;
                    }
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, type, (object?)default, handler);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
                return default;
            }
        }


        ///<inheritdoc/>
        public object? Get(Type? target, Type type, PolicyChangeHandler handler)
        {
                var meta = Meta;
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            ref var bucket = ref meta[hash % Meta.Length];
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

                position = meta[position].Location;
            }

            var length = Count;

            lock (SyncRoot)
            {
                // Repeat search if modified
                if (length != Count)
                {
                    bucket = ref Meta[hash % Meta.Length];
                    position = bucket.Position;

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
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, target, type, default, handler);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
                return default;
            }
        }

        #endregion

        
        #region Set

        ///<inheritdoc/>
        public void Set(Type type, object? policy, PolicyChangeHandler handler)
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
                        candidate.Value = policy;
                        candidate.PolicyChanged += handler;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, type, policy, handler);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }


        ///<inheritdoc/>
        public void Set(Type? target, Type type, object? policy, PolicyChangeHandler handler)
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
                        candidate.Value = policy;
                        candidate.PolicyChanged += handler;
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
                Data[Count] = new Policy(hash, target, type, policy, handler);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
            }
        }

        #endregion


        #region Compare Exchange

        ///<inheritdoc/>
        public TPolicy? CompareExchange<TPolicy>(Type? target, Type type, TPolicy policy, TPolicy? comparand) 
            where TPolicy : class
        {
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
                var meta = Meta;
            ref var bucket = ref meta[hash % Meta.Length];
                var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    return (TPolicy?)candidate.CompareExchange(policy, comparand);
                }

                position = meta[position].Location;
            }

            if (comparand is not null) return default;

            var length = Count;

            lock (SyncRoot)
            {
                // Repeat search if modified
                if (length != Count)
                {
                    bucket = ref Meta[hash % Meta.Length];
                    position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position];
                        if (ReferenceEquals(candidate.Target, target) &&
                            ReferenceEquals(candidate.Type, type))
                        {
                            // Found existing
                            return (TPolicy?)candidate.CompareExchange(policy, comparand);
                        }

                        position = Meta[position].Location;
                    }
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
                return default;
            }
        }


        ///<inheritdoc/>
        public TPolicy? CompareExchange<TPolicy>(Type? target, Type type, TPolicy policy, TPolicy? comparand, PolicyChangeHandler handler)
            where TPolicy : class
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ type.GetHashCode());
            var meta = Meta;
            ref var bucket = ref meta[hash % Meta.Length];
            var position = bucket.Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position];
                if (ReferenceEquals(candidate.Target, target) &&
                    ReferenceEquals(candidate.Type, type))
                {
                    // Found existing
                    var value = candidate.CompareExchange(policy, comparand);
                    candidate.PolicyChanged += handler;
                    return (TPolicy?)value;
                }

                position = meta[position].Location;
            }

            if (comparand is not null) return default;

            var length = Count;

            lock (SyncRoot)
            {
                // Repeat search if modified
                if (length != Count)
                {
                    bucket = ref Meta[hash % Meta.Length];
                    position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position];
                        if (ReferenceEquals(candidate.Target, target) &&
                            ReferenceEquals(candidate.Type, type))
                        {
                            // Found existing
                            var value = candidate.CompareExchange(policy, comparand);
                            candidate.PolicyChanged += handler;
                            return (TPolicy?)value;
                        }

                        position = Meta[position].Location;
                    }
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, target, type, policy, handler);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;
                return default;
            }
        }

        #endregion
    }
}
