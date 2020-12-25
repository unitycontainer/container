using System;
using System.Diagnostics;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Implementation

        private void Allocate<TPolicy>(PolicyChangeHandler handler)
        {
                var hash = (uint)(37 ^ typeof(TPolicy).GetHashCode());
            ref var bucket = ref Meta[hash % Meta.Length];
            ref var entry = ref Data[++Count];

            entry = new Policy(hash, typeof(TPolicy), null);
            entry.PolicyChanged += handler;
            Meta[Count].Location = bucket.Position;
            bucket.Position = Count;
        }

        private void Allocate<TTarget, TPolicy>(PolicyChangeHandler handler)
        {
                var hash = (uint)((typeof(TTarget).GetHashCode() + 37) ^ typeof(TPolicy).GetHashCode());
            ref var bucket = ref Meta[hash % Meta.Length];
            ref var entry = ref Data[++Count];

            entry = new Policy(hash, typeof(TTarget), typeof(TPolicy), null);
            entry.PolicyChanged += handler;
            Meta[Count].Location = bucket.Position;
            bucket.Position = Count;
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

        private static object? DummyPipeline(ref PipelineContext _) 
            => UnityContainer.NoValue;

        #endregion


        #region Nested Policy

        [DebuggerDisplay("Policy = { Type?.Name }", Name = "{ Target?.Name }")]
        [CLSCompliant(false)]
        public struct Policy
        {
            #region Fields

            private object? _value;
            public readonly uint Hash;
            public readonly Type? Target;
            public readonly Type  Type;

            #endregion


            #region Constructors

            public Policy(uint hash, Type type, object? value)
            {
                Hash = hash;
                Target = null;
                Type = type;
                _value = value;
                PolicyChanged = default;
            }

            public Policy(uint hash, Type? target, Type type, object? value)
            {
                Hash = hash;
                Target = target;
                Type = type;
                _value = value;
                PolicyChanged = default;
            }

            #endregion


            #region Property

            public object? Value
            {
                get => _value;
                set
                {
                    _value = value;
                    PolicyChanged?.Invoke(Target, Type, value!);
                }
            }

            #endregion


            #region Notification

            public event PolicyChangeHandler? PolicyChanged;

            #endregion
        }

        #endregion
    }
}
