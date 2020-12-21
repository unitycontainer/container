using System;
using System.Diagnostics;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Get Or Add

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
        /// Adds pipeline, if does not exist already, or returns existing
        /// </summary>
        /// <param name="type">Target <see cref="Type"/></param>
        /// <param name="pipeline">Pipeline to add</param>
        /// <returns>Existing or added pipeline</returns>
        public ResolveDelegate<PipelineContext> GetOrAdd(Type? type, ResolveDelegate<PipelineContext> pipeline)
        {
            var hash = (uint)(((type?.GetHashCode() ?? 0) + 37) ^ _resolverHash);

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, type) &&
                        ReferenceEquals(candidate.Type, typeof(ResolveDelegate<PipelineContext>)))
                    {
                        // Found existing
                        if (candidate.Value is null) candidate.Value = pipeline;
                        return (ResolveDelegate<PipelineContext>)candidate.Value;
                    }

                    position = Meta[position].Location;
                }

                if (++Count >= Data.Length)
                {
                    Expand();
                    bucket = ref Meta[hash % Meta.Length];
                }

                // Add new registration
                Data[Count] = new Policy(hash, type, typeof(ResolveDelegate<PipelineContext>), pipeline);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;

                return pipeline;
            }
        }

        #endregion


        #region Span

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ReadOnlySpan<Policy> Span => new ReadOnlySpan<Policy>(Data, 1, Count);

        #endregion


        #region Implementation

        protected virtual void Expand()
        {
            Array.Resize(ref Data, Storage.Prime.Numbers[Prime++]);
            Meta = new Metadata[Storage.Prime.Numbers[Prime]];

            for (var current = 1; current < Count; current++)
            {
                var bucket = Data[current].Hash % Meta.Length;
                Meta[current].Location = Meta[bucket].Position;
                Meta[bucket].Position = current;
            }
        }

        #endregion


        #region Policy structure

        [DebuggerDisplay("Policy = { Type?.Name }", Name = "{ Target?.Name }")]
        [CLSCompliant(false)]
        public struct Policy
        {
            public readonly uint Hash;
            public readonly Type? Target;
            public readonly Type Type;

            public object? Value;

            public Policy(uint hash, Type type, object? value)
            {
                Hash = hash;
                Target = null;
                Type = type;
                Value = value;
                PolicyChanged = default;
            }

            public Policy(uint hash, Type? target, Type type, object? value)
            {
                Hash = hash;
                Target = target;
                Type = type;
                Value = value;
                PolicyChanged = default;
            }


            #region Notification

            public event PolicyChangeNotificationHandler? PolicyChanged;

            #endregion

            internal PolicyChangeNotificationHandler? Handler => PolicyChanged; 
        }

        #endregion
    }
}
