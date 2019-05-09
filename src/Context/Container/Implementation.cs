using System;
using System.Linq;
using Unity.Lifetime;
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

            #endregion


            #region Constructors

            // Root Container Constructor
            internal ContainerContext(UnityContainer container, StagedStrategyChain<Pipeline, Stage> type,
                                                                StagedStrategyChain<Pipeline, Stage> factory,
                                                                StagedStrategyChain<Pipeline, Stage> instance)
            {
                // Container this context represents
                Container = container;

                // Lifetime Managers
                _typeLifetimeManager     = TransientLifetimeManager.Instance;
                _factoryLifetimeManager  = TransientLifetimeManager.Instance;
                _instanceLifetimeManager = new ContainerControlledLifetimeManager();

                // Initialize Pipelines
                TypePipeline = type;
                TypePipeline.Invalidated += (s, e) => TypePipelineCache = TypePipeline.ToArray();
                TypePipelineCache = TypePipeline.ToArray();

                FactoryPipeline = factory;
                FactoryPipeline.Invalidated += (s, e) => FactoryPipelineCache = FactoryPipeline.ToArray();
                FactoryPipelineCache = FactoryPipeline.ToArray();

                InstancePipeline = instance;
                InstancePipeline.Invalidated += (s, e) => InstancePipelineCache = InstancePipeline.ToArray();
                InstancePipelineCache = InstancePipeline.ToArray();
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
                TypePipeline = new StagedStrategyChain<Pipeline, Stage>(parent.Context.TypePipeline);
                TypePipeline.Invalidated += (s, e) => TypePipelineCache = TypePipeline.ToArray();
                TypePipelineCache = TypePipeline.ToArray();

                FactoryPipeline = new StagedStrategyChain<Pipeline, Stage>(parent.Context.FactoryPipeline);
                FactoryPipeline.Invalidated += (s, e) => FactoryPipelineCache = FactoryPipeline.ToArray();
                FactoryPipelineCache = FactoryPipeline.ToArray();

                InstancePipeline = new StagedStrategyChain<Pipeline, Stage>(parent.Context.InstancePipeline);
                InstancePipeline.Invalidated += (s, e) => InstancePipelineCache = InstancePipeline.ToArray();
                InstancePipelineCache = InstancePipeline.ToArray();
            }

            #endregion
        }
    }
}
