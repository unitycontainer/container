using System.Diagnostics;
using Unity.Builder;
using Unity.Extension;
using Unity.Policy;
using Unity.Processors;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Policies
    {
        #region Implementation

        private void Allocate<TPolicy>(PolicyChangeHandler handler)
        {
                var hash = (uint)(37 ^ typeof(TPolicy).GetHashCode());
            ref var bucket = ref _meta[hash % _meta.Length];
            ref var entry = ref _data[++_count];

            entry = new Entry(hash, typeof(TPolicy), null);
            entry.PolicyChanged += handler;
            _meta[_count].Location = bucket.Position;
            bucket.Position = _count;
        }

        private void Allocate<TPolicy>(Type target, PolicyChangeHandler handler)
        {
            var hash = (uint)((target.GetHashCode() + 37) ^ typeof(TPolicy).GetHashCode());
            ref var bucket = ref _meta[hash % _meta.Length];
            ref var entry = ref _data[++_count];

            entry = new Entry(hash, target, typeof(TPolicy), null, handler);
            _meta[_count].Location = bucket.Position;
            bucket.Position = _count;
        }

        private void Expand()
        {
            var prime = Prime.IndexOf(_meta.Length);

            Array.Resize(ref _data, Prime.Numbers[prime++]);
            _meta = new Metadata[Prime.Numbers[prime]];

            for (var current = 1; current < _count; current++)
            {
                var bucket = _data[current].Hash % _meta.Length;
                _meta[current].Location = _meta[bucket].Position;
                _meta[bucket].Position = current;
            }
        }

        #endregion


        #region Algorithms

        private void OnResolveUnregisteredChanged(Type? target, Type type, object? policy)
            => ResolveUnregistered = (ResolverPipeline)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        private void OnResolveRegisteredChanged(Type? target, Type type, object? policy)
            => ResolveRegistered = (ResolverPipeline)(policy ??
                throw new ArgumentNullException(nameof(policy)));

        private void OnResolveArrayChanged(Type? target, Type type, object? policy)
            => ResolveArray = (ResolverPipeline)(policy ??
                throw new ArgumentNullException(nameof(policy)));
        
        #endregion


        #region Rebuilds stage chains when modified

        private void OnActivateChainChanged(object? sender, EventArgs e)
        {
            var chain = ((IStagedStrategyChain<BuilderStrategyDelegate<BuilderContext>>)(sender ??
                throw new ArgumentNullException(nameof(sender)))).MakeStrategyChain();

            var converter = this.Get<ChainToPipelineConverter>() ?? 
                throw new InvalidOperationException();
            
            ActivatePipeline = converter.Invoke(chain);
        }

        private void OnMappingChainChanged(object? sender, EventArgs e)
        {
            var chain = (StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityMappingStage>)(sender ??
                throw new ArgumentNullException(nameof(sender)));

            if (1 == chain.Version) return;

            MappingPipeline = RebuildPipeline(chain.MakeStrategyChain());
        }

        private void OnInstanceChainChanged(object? sender, EventArgs e)
        {
            var chain = (StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityInstanceStage>)(sender ??
                throw new ArgumentNullException(nameof(sender)));

            if (1 == chain.Version) return;

            InstancePipeline = RebuildPipeline(chain.MakeStrategyChain());
        }

        private void OnFactoryChainChanged(object? sender, EventArgs e)
        {
            var chain = (StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityFactoryStage>)(sender ??
                throw new ArgumentNullException(nameof(sender)));

            if (1 == chain.Version) return;

            FactoryPipeline = RebuildPipeline(chain.MakeStrategyChain());
        }

        private void OnBuildChainChanged(object? sender, EventArgs e)
        {
            var chain = (IStagedStrategyChain<MemberProcessor>)(sender ??
                throw new ArgumentNullException(nameof(sender)));

            // TODO: if (1 == chain.Version) return;

            var factory = this.Get<ChainToFactoryConverter>() ??
                throw new InvalidOperationException("Factory policy is null");

            PipelineFactory = factory.Invoke(chain.MakeStrategyChain());
        }

        private ResolverPipeline RebuildPipeline(BuilderStrategyDelegate<BuilderContext>[] chain)
        {
            var factory = this.Get<ChainToPipelineConverter>() ??
                throw new InvalidOperationException();
            
            return factory.Invoke(chain);
        }


        private void OnChainToPipelineConverterChanged(Type? target, Type type, object? policy)
        {
            if (_activationChain is null) return;

            var converter = (ChainToPipelineConverter)(policy 
                ?? throw new ArgumentNullException(nameof(ChainToPipelineConverter)));

            ActivatePipeline = converter.Invoke(ActivationChain.MakeStrategyChain());
            MappingPipeline = converter.Invoke(MappingChain.MakeStrategyChain());
            InstancePipeline = converter.Invoke(InstanceChain.MakeStrategyChain());
            FactoryPipeline = converter.Invoke(FactoryChain.MakeStrategyChain());
        }

        private void OnChainToFactoryConverterChanged(Type? target, Type type, object? policy)
        {
            if (_buildPlanChain is null) return;

            var converter = (ChainToFactoryConverter)(policy 
                ?? throw new ArgumentNullException(nameof(ChainToFactoryConverter)));

            PipelineFactory = converter.Invoke(BuildPlanChain.MakeStrategyChain());
        }

        #endregion


        #region Policy Storage

        [DebuggerDisplay("Policy = { Type?.Name }", Name = "{ Target?.Name }")]
        [CLSCompliant(false)]
        public struct Entry
        {
            #region Fields

            private object? _value;
            public readonly uint Hash;
            public readonly Type? Target;
            public readonly Type  Type;

            #endregion


            #region Constructors

            public Entry(uint hash, Type type, object? value)
            {
                Hash = hash;
                Target = null;
                Type = type;
                _value = value;
                PolicyChanged = default;
            }

            public Entry(uint hash, Type type, object? value, PolicyChangeHandler handler)
            {
                Hash = hash;
                Target = null;
                Type = type;
                _value = value;
                PolicyChanged = handler;
            }

            public Entry(uint hash, Type? target, Type type, object? value)
            {
                Hash = hash;
                Target = target;
                Type = type;
                _value = value;
                PolicyChanged = default;
            }

            public Entry(uint hash, Type? target, Type type, object? value, PolicyChangeHandler handler)
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
