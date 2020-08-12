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

        public ResolveDelegate<ResolveContext> TypeActivationPipeline { get; set; }
        public ResolveDelegate<ResolveContext> FactoryActivationPipeline { get; set; }
        public ResolveDelegate<ResolveContext> InstanceActivationPipeline { get; set; }
        public ResolveDelegate<ResolveContext> UnregisteredActivationPipeline { get; set; }

        #endregion


        #region Chains

        public StagedChain<BuilderStage, PipelineProcessor> TypeChain { get; }
                                        
        public StagedChain<BuilderStage, PipelineProcessor> FactoryChain { get; }
                                        
        public StagedChain<BuilderStage, PipelineProcessor> InstanceChain { get; }
                                        
        public StagedChain<BuilderStage, PipelineProcessor> UnregisteredChain { get; }

        #endregion


        #region Implementation

        private ResolveDelegate<ResolveContext> DummyResolver(in ContainerContext contest)
        {
            return (ref ResolveContext c) => c.Type;
        }

        private object? DummyPipeline(ref ResolveContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
