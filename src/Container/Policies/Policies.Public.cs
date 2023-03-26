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
        #region Build Chains

        /// <summary>
        /// Build Up strategies chain
        /// </summary>
        public IActivateChain ActivateChain
        {
            get
            {
                if (_activateChain is not null) return _activateChain;

                var factory = this.Get<IBuildPlanChain, Func<IActivateChain>>()
                    ?? throw new InvalidOperationException("Build Plan chain initializer is not set");

                _activateChain = factory();
                _activateChain.Invalidated += OnActivateChainChanged;

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

                var factory = this.Get<Func<IFactoryChain>>()
                    ?? throw new InvalidOperationException("Factory chain initializer is not set");

                _factoryChain = factory();
                _factoryChain.Invalidated += OnFactoryChainChanged;

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

                var factory = this.Get<Func<IInstanceChain>>()
                    ?? throw new InvalidOperationException("Instance chain initializer is not set");

                _instanceChain = factory();
                _instanceChain.Invalidated += OnInstanceChainChanged;

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

                var factory = this.Get<Func<IMappingChain>>()
                    ?? throw new InvalidOperationException("Mapping chain initializer is not set");

                _mappingChain = factory();
                _mappingChain.Invalidated += OnMappingChainChanged;

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

                var factory = this.Get<Func<IBuildPlanChain>>()
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
    }
}
