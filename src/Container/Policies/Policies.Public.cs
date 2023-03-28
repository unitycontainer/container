﻿using System.Diagnostics;
using Unity.Builder;
using Unity.Extension;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Container
{
    public partial class Policies<TContext>
    {
        #region Build Chains

        /// <summary>
        /// Build Up strategies chain
        /// </summary>
        public IActivateChain ActivationChain
        {
            get
            {
                if (_activateChain is not null) return _activateChain;

                _activateChain = new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityActivateStage>();
                _activateChain.Invalidated += OnActivateChainChanged;

                this.Get<Action<IActivateChain>>()?.Invoke(_activateChain);

                return _activateChain;
            }
        }
        
        /// <summary>
        /// Factory strategies chain
        /// </summary>
        public IFactoryChain FactoryChain
        {
            get
            {
                if (_factoryChain is not null) return _factoryChain;

                _factoryChain = new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityFactoryStage>(); 
                _factoryChain.Invalidated += OnFactoryChainChanged;
                
                this.Get<Action<IFactoryChain>>()?.Invoke(_factoryChain);

                return _factoryChain;
            }
        }

        /// <summary>
        /// Instance strategies chain
        /// </summary>
        public IInstanceChain InstanceChain
        {
            get
            {
                if (_instanceChain is not null) return _instanceChain;

                _instanceChain = new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityInstanceStage>();
                _instanceChain.Invalidated += OnInstanceChainChanged;

                this.Get<Action<IInstanceChain>>()?.Invoke(_instanceChain);

                return _instanceChain;
            }
        }

        /// <summary>
        /// Mapping strategies chain
        /// </summary>
        public IMappingChain MappingChain
        {
            get
            {
                if (_mappingChain is not null) return _mappingChain;

                _mappingChain = new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityMappingStage>();
                _mappingChain.Invalidated += OnMappingChainChanged;

                this.Get<Action<IMappingChain>>()?.Invoke(_mappingChain);

                return _mappingChain;
            }
        }

        /// <summary>
        /// Build Plan strategies chain
        /// </summary>
        public IBuildPlanChain BuildPlanChain
        {
            get
            {
                if (_buildPlanChain is not null) return _buildPlanChain;

                _buildPlanChain = new StagedStrategyChain<BuilderStrategyDelegate<BuilderContext>, UnityBuildStage>();
                _buildPlanChain.Invalidated += OnBuildChainChanged;

                this.Get<Action<IBuildPlanChain>>()?.Invoke(_buildPlanChain);

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
    }
}
