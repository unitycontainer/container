using System;
using Unity.Pipeline;
using Unity.Resolution;
using Unity.Storage;
using static Unity.UnityContainer;

namespace Unity.Container
{

    public delegate ResolveDelegate<ResolveContext> ResolveDelegateFactory(in ContainerContext contest);

    public partial class Defaults
    {
        #region Fields


        #endregion


        #region Resolver Factory


        public ResolveDelegateFactory TypeResolver { get; private set; }
        
        public ResolveDelegateFactory FactoryResolver { get; private set; }

        public ResolveDelegateFactory InstanceResolver { get; private set; }

        public ResolveDelegateFactory UnregisteredFactory { get; private set; }

        #endregion


        #region Pipelines

        public StagedChain<BuilderStage, PipelineProcessor> TypePipeline { get; }
                                        
        public StagedChain<BuilderStage, PipelineProcessor> FactoryPipeline { get; }
                                        
        public StagedChain<BuilderStage, PipelineProcessor> InstancePipeline { get; }
                                        
        public StagedChain<BuilderStage, PipelineProcessor> UnregisteredPipeline { get; }

        #endregion


        #region Implementation

        private ResolveDelegate<ResolveContext> DummyResolver(in ContainerContext contest)
        {
            return (ref ResolveContext c) => c.Type;
        }

        #endregion
    }
}
