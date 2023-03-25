using System;
using System.Diagnostics;
using Unity.Builder;
using Unity.Extension;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    public partial class Policies<TContext>
    {
        #region Fields

        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _buildPlanChain;
        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _instanceChain;
        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _factoryChain;
        private IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>? _mappingChain;

        #endregion


        #region Build Chains

        /// <summary>
        /// Build Up strategies chain
        /// </summary>
        public IStagedStrategyChain<BuilderStrategy, UnityBuildStage> StrategiesChain { get; }

        /// <summary>
        /// Factory strategies chain
        /// </summary>
        public IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> FactoryChain
        {
            get
            {
                if (_factoryChain is not null) return _factoryChain;

                var factory = this.Get<IFactoryChain, Func<IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>>>()
                    ?? throw new InvalidOperationException("Factory chain initializer is not set");

                _factoryChain = factory();
                _factoryChain.Invalidated += OnFactoryChainChanged;

                return _factoryChain;
            }
        }

        /// <summary>
        /// Instance strategies chain
        /// </summary>
        public IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> InstanceChain
        {
            get
            {
                if (_instanceChain is not null) return _instanceChain;

                var factory = this.Get<IInstanceChain, Func<IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>>>()
                    ?? throw new InvalidOperationException("Instance chain initializer is not set");

                _instanceChain = factory();
                _instanceChain.Invalidated += OnInstanceChainChanged;

                return _instanceChain;
            }
        }

        /// <summary>
        /// Mapping strategies chain
        /// </summary>
        public IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> MappingChain
        {
            get
            {
                if (_mappingChain is not null) return _mappingChain;

                var factory = this.Get<IMappingChain, Func<IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>>>()
                    ?? throw new InvalidOperationException("Mapping chain initializer is not set");

                _mappingChain = factory();
                _mappingChain.Invalidated += OnMappingChainChanged;

                return _mappingChain;
            }
        }

        /// <summary>
        /// Build Plan strategies chain
        /// </summary>
        public IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> BuildPlanChain
        {
            get
            {
                if (_buildPlanChain is not null) return _buildPlanChain;

                var factory = this.Get<IBuildPlanChain, Func<IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage>>>()
                    ?? throw new InvalidOperationException("Build Plan chain initializer is not set");

                _buildPlanChain = factory();
                _buildPlanChain.Invalidated += OnBuildChainChanged;

                return _buildPlanChain;
            }
        }

        #endregion


        #region Public Members

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ReadOnlySpan<Policy> Span => new ReadOnlySpan<Policy>(Data, 1, Count);

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

        #endregion


        #region Pipeline Types

        public interface IStrategiesChain : IStagedStrategyChain<BuilderStrategy, UnityBuildStage> { }
        public interface IBuildPlanChain  : IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> { }
        public interface IFactoryChain    : IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> { }
        public interface IInstanceChain   : IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> { }
        public interface IMappingChain    : IStagedStrategyChain<BuilderStrategyDelegate<TContext>, UnityBuildStage> { }

        #endregion
    }
}
