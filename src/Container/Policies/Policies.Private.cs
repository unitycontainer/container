using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Policies<TContext>
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

        private void Allocate<TPolicy>(Type target, PolicyChangeHandler handler)
        {
            var hash = (uint)((target.GetHashCode() + 37) ^ typeof(TPolicy).GetHashCode());
            ref var bucket = ref Meta[hash % Meta.Length];
            ref var entry = ref Data[++Count];

            entry = new Policy(hash, target, typeof(TPolicy), null, handler);
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

        private static ResolveDelegate<TContext> DummyFactory(ref TContext context)
                    => UnityContainer.DummyPipeline;

        #endregion


        #region Change Handlers

        // Algorithms

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnResolveUnregisteredChanged(Type? target, Type type, object? policy)
            => ResolveUnregistered = (ResolveDelegate<TContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnResolveRegisteredChanged(Type? target, Type type, object? policy)
            => ResolveRegistered = (ResolveDelegate<TContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnResolveArrayChanged(Type? target, Type type, object? policy)
            => ResolveArray = (ResolveDelegate<TContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        // Pipelines

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnActivatePipelineChanged(Type? target, Type type, object? policy)
            => ActivatePipeline = (ResolveDelegate<TContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnFactoryPipelineChanged(Type? target, Type type, object? policy)
            => FactoryPipeline = (ResolveDelegate<TContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnInstancePipelineChanged(Type? target, Type type, object? policy)
            => InstancePipeline = (ResolveDelegate<TContext>)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        #endregion


        #region Policy Storage

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

            public Policy(uint hash, Type type, object? value, PolicyChangeHandler handler)
            {
                Hash = hash;
                Target = null;
                Type = type;
                _value = value;
                PolicyChanged = handler;
            }

            public Policy(uint hash, Type? target, Type type, object? value)
            {
                Hash = hash;
                Target = target;
                Type = type;
                _value = value;
                PolicyChanged = default;
            }

            public Policy(uint hash, Type? target, Type type, object? value, PolicyChangeHandler handler)
            {
                Hash = hash;
                Target = target;
                Type = type;
                _value = value;
                PolicyChanged = handler;
            }

            #endregion


            #region Public Members

            public object? Value
            {
                get => _value;
                set
                {
                    _value = value;
                    PolicyChanged?.Invoke(Target, Type, value!);
                }
            }

            public object? CompareExchange(object? policy, object? comparand) 
                => Interlocked.CompareExchange(ref _value, policy, comparand);

            #endregion


            #region Notification

            public event PolicyChangeHandler? PolicyChanged;

            #endregion
        }

        #endregion
    }
}
