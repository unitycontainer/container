using System;
using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Policy;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        /// <inheritdoc />
        public partial class ContainerContext
        {
            #region Extension Context

            public override UnityContainer Container { get; }

            public override ILifetimeContainer Lifetime => Container.LifetimeContainer;

            public override IPolicyList Policies => this as IPolicyList;

            #endregion


            #region Pipelines

            public override StagedStrategyChain<PipelineBuilder, PipelineStage> TypePipeline
            {
                get => _typePipeline; set
                {
                    _typePipeline = value;
                    _typePipeline.Invalidated += (s, e) => TypePipelineCache = TypePipeline.ToArray();
                    TypePipelineCache = _typePipeline.ToArray();
                }
            }

            public override StagedStrategyChain<PipelineBuilder, PipelineStage> FactoryPipeline
            {
                get => _factoryPipeline; set
                {
                    _factoryPipeline = value;
                    _factoryPipeline.Invalidated += (s, e) => FactoryPipelineCache = FactoryPipeline.ToArray();
                    FactoryPipelineCache = _factoryPipeline.ToArray();
                }
            }

            public override StagedStrategyChain<PipelineBuilder, PipelineStage> InstancePipeline
            {
                get => _instancePipeline; set
                {
                    _instancePipeline = value;
                    _instancePipeline.Invalidated += (s, e) => InstancePipelineCache = InstancePipeline.ToArray();
                    InstancePipelineCache = _instancePipeline.ToArray();
                }
            }

            #endregion


            #region Lifetime

            public override ITypeLifetimeManager TypeLifetimeManager
            {
                get => (ITypeLifetimeManager)_typeLifetimeManager;
                set
                {
                    _typeLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _typeLifetimeManager.InUse = true;
                }
            }

            public override IFactoryLifetimeManager FactoryLifetimeManager
            {
                get => (IFactoryLifetimeManager)_factoryLifetimeManager;
                set
                {
                    _factoryLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _factoryLifetimeManager.InUse = true;
                }
            }

            public override IInstanceLifetimeManager InstanceLifetimeManager
            {
                get => (IInstanceLifetimeManager)_instanceLifetimeManager;
                set
                {
                    _instanceLifetimeManager = (LifetimeManager)(value ?? throw new ArgumentNullException(error));
                    _instanceLifetimeManager.InUse = true;
                }
            }

            #endregion
        }
    }
}
