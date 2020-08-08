using System;
using System.Diagnostics;
using Unity.Pipeline;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Constants

        private static uint ResolverHash = (uint)typeof(ResolveDelegate<ResolveContext>).GetHashCode();
        
        #endregion


        #region Span

        public ReadOnlySpan<Policy> Span => new ReadOnlySpan<Policy>(_data, 1, _count);

        #endregion

        public ResolveDelegate<ResolveContext>? this[Type? target]
        {
            get
            {
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ ResolverHash);
                var position = _meta[hash % _meta.Length].Position;

                while (position > 0)
                {
                    ref var candidate = ref _data[position];
                    if (ReferenceEquals(candidate.Target, target) &&
                        ReferenceEquals(candidate.Type, typeof(ResolveDelegate<ResolveContext>)))
                    {
                        // Found existing
                        return (ResolveDelegate<ResolveContext>?)candidate.Value;
                    }

                    position = _meta[position].Next;
                }

                return null;
            }

            set
            {
                var hash = (uint)(((target?.GetHashCode() ?? 0) + 37) ^ ResolverHash);

                lock (_syncRoot)
                {
                    ref var bucket = ref _meta[hash % _meta.Length];
                    var position = bucket.Position;

                    while (position > 0)
                    {
                        ref var candidate = ref _data[position];
                        if (ReferenceEquals(candidate.Target, target) &&
                            ReferenceEquals(candidate.Type, typeof(ResolveDelegate<ResolveContext>)))
                        {
                            // Found existing
                            candidate.Value = value;
                            return;
                        }

                        position = _meta[position].Next;
                    }

                    if (++_count >= _data.Length)
                    {
                        Expand();
                        bucket = ref _meta[hash % _meta.Length];
                    }

                    // Add new registration
                    _data[_count] = new Policy(hash, target, typeof(ResolveDelegate<ResolveContext>), value);
                    _meta[_count].Next = bucket.Position;
                    bucket.Position = _count;
                }
            }
        }

        protected virtual void Expand()
        {
            Array.Resize(ref _data, Prime.Numbers[_prime++]);
            _meta = new Metadata[Prime.Numbers[_prime]];

            for (var current = 1; current < _count; current++)
            {
                var bucket = _data[current].Hash % _meta.Length;
                _meta[current].Next = _meta[bucket].Position;
                _meta[bucket].Position = current;
            }
        }


        #region Policy structure

        [DebuggerDisplay("Target = { Target?.Name }, Policy = { Type.Name }", Name = "{ Hash }")]
        public struct Policy
        {
            public readonly uint  Hash;
            public readonly Type? Target;
            public readonly Type  Type;
            
            public object?  Value;

            public Policy(uint hash, Type type, object? value)
            {
                Hash = hash;
                Target = null;
                Type = type;
                Value = value;
            }

            public Policy(uint hash, Type? target, Type type, object? value)
            {
                Hash   = hash;
                Target = target;
                Type   = type;
                Value  = value;
            }
        }

        #endregion
    }
}
