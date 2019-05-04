using System;
using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        public partial class ContainerContext
        {
            #region Fields

            private LifetimeManager _typeLifetimeManager;
            private LifetimeManager _factoryLifetimeManager;
            private LifetimeManager _instanceLifetimeManager;
            private const string error = "Lifetime Manager must not be null";
            private StagedStrategyChain<PipelineBuilder, PipelineStage> _typePipeline;
            private StagedStrategyChain<PipelineBuilder, PipelineStage> _factoryPipeline;
            private StagedStrategyChain<PipelineBuilder, PipelineStage> _instancePipeline;

            #endregion


            #region Constructors

            // Root Container Constructor
            internal ContainerContext(UnityContainer container, StagedStrategyChain<PipelineBuilder, PipelineStage> type,
                                                                StagedStrategyChain<PipelineBuilder, PipelineStage> factory,
                                                                StagedStrategyChain<PipelineBuilder, PipelineStage> instance)
            {
                // Container this context represents
                Container = container;

                // Lifetime Managers
                _typeLifetimeManager     = TransientLifetimeManager.Instance;
                _factoryLifetimeManager  = TransientLifetimeManager.Instance;
                _instanceLifetimeManager = new ContainerControlledLifetimeManager();

                // Initialize Pipelines
                _typePipeline = type;
                _typePipeline.Invalidated += (s, e) => TypePipelineCache = TypePipeline.ToArray();
                TypePipelineCache = _typePipeline.ToArray();

                _factoryPipeline = factory;
                _factoryPipeline.Invalidated += (s, e) => FactoryPipelineCache = FactoryPipeline.ToArray();
                FactoryPipelineCache = _factoryPipeline.ToArray();

                _instancePipeline = instance;
                _instancePipeline.Invalidated += (s, e) => InstancePipelineCache = InstancePipeline.ToArray();
                InstancePipelineCache = _instancePipeline.ToArray();
            }

            // Child Container Constructor
            internal ContainerContext(UnityContainer container)
            {
                // Container this context represents
                Container = container;

                var parent = Container._parent ?? throw new InvalidOperationException("Parent must not be null");

                // Lifetime Managers
                _typeLifetimeManager     = parent.Context._typeLifetimeManager;
                _factoryLifetimeManager  = parent.Context._factoryLifetimeManager;
                _instanceLifetimeManager = parent.Context._instanceLifetimeManager;

                // TODO: Create on demand

                // Initialize Pipelines
                _typePipeline = new StagedStrategyChain<PipelineBuilder, PipelineStage>(parent.Context.TypePipeline);
                _typePipeline.Invalidated += (s, e) => TypePipelineCache = TypePipeline.ToArray();
                TypePipelineCache = _typePipeline.ToArray();

                _factoryPipeline = new StagedStrategyChain<PipelineBuilder, PipelineStage>(parent.Context.FactoryPipeline);
                _factoryPipeline.Invalidated += (s, e) => FactoryPipelineCache = FactoryPipeline.ToArray();
                FactoryPipelineCache = _factoryPipeline.ToArray();

                _instancePipeline = new StagedStrategyChain<PipelineBuilder, PipelineStage>(parent.Context.InstancePipeline);
                _instancePipeline.Invalidated += (s, e) => InstancePipelineCache = InstancePipeline.ToArray();
                InstancePipelineCache = _instancePipeline.ToArray();
            }

            #endregion



        }
    }
}
