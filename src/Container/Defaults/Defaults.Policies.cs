using System;
using System.Diagnostics;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Constants

        private static uint ResolverHash = (uint)typeof(ResolveDelegate<PipelineContext>).GetHashCode();

        #endregion


        #region Span

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ReadOnlySpan<Policy> Span => new ReadOnlySpan<Policy>(Data, 1, Count);

        #endregion

        public ResolveDelegate<PipelineContext>? this[Type? target]
        {
            get
            {
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ ResolverHash);
                var position = Meta[hash % Meta.Length].Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Type, typeof(ResolveDelegate<PipelineContext>)))
                    {
                        // Found existing
                        return (ResolveDelegate<PipelineContext>?)candidate.Value;
                    }

                    position = Meta[position].Location;
                }

                return null;
            }

            set
            {
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ ResolverHash);

                lock (_syncRoot)
                {
                    ref var bucket = ref Meta[hash % Meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref Data[position];
                        if (ReferenceEquals(candidate.Target, target) &&
                            ReferenceEquals(candidate.Type, typeof(ResolveDelegate<PipelineContext>)))
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

                    // Add new registration
                    Data[Count] = new Policy(hash, target, typeof(ResolveDelegate<PipelineContext>), value);
                    Meta[Count].Location = bucket.Position;
                    bucket.Position = Count;
                }
            }
        }

        /// <summary>
        /// Adds pipeline, if does not exist already, or returns existing
        /// </summary>
        /// <param name="target">Target <see cref="Type"/></param>
        /// <param name="pipeline">Pipeline to add</param>
        /// <returns>Existing or added pipeline</returns>
        public ResolveDelegate<PipelineContext> AddOrGet(Type? target, ResolveDelegate<PipelineContext> pipeline)
        {
            var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ ResolverHash);

            lock (_syncRoot)
            {
                ref var bucket = ref Meta[hash % Meta.Length];
                var position = bucket.Position;

                while (position > 0)
                {
                    ref var candidate = ref Data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
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
                Data[Count] = new Policy(hash, target, typeof(ResolveDelegate<PipelineContext>), pipeline);
                Meta[Count].Location = bucket.Position;
                bucket.Position = Count;

                return pipeline;
            }
        }

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
