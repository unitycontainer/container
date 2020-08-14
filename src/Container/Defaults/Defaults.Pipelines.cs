using Unity.Lifetime;
using Unity.Pipeline;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public delegate ResolveDelegate<ResolveContext> ResolveDelegateFactory(ref ResolveContext context);


    public partial class Defaults
    {
        #region Constants

        private readonly int NONOPTIMIZEDPIPELINE_FACTORY;
        private readonly int PIPECREATIONOPTIMIZED_FACTORY;
        private readonly int PERFORMANCEOPTIMIZED_FACTORY;

        #endregion


        #region Chains

        public StagedChain<BuilderStage, PipelineProcessor> TypeChain { get; }

        public StagedChain<BuilderStage, PipelineProcessor> FactoryChain { get; }

        public StagedChain<BuilderStage, PipelineProcessor> InstanceChain { get; }

        public StagedChain<BuilderStage, PipelineProcessor> UnregisteredChain { get; }

        #endregion


        #region Resolution

        public object? ResolveContract(in ResolveContext context)
        {
            return null;
        }

        public object? ResolveContract(UnityContainer container, in Contract contract, LifetimeManager manager, ResolverOverride[] overrides)
        {
            return null;
        }

        public ResolveDelegateFactory NonOptimizedPipelineFactory 
            => (ResolveDelegateFactory)_data[NONOPTIMIZEDPIPELINE_FACTORY].Value!; 

        public ResolveDelegateFactory PipeCreationOptimizedFactory 
            => (ResolveDelegateFactory)_data[PIPECREATIONOPTIMIZED_FACTORY].Value!;

        public ResolveDelegateFactory PerformanceOptimizedFactory 
            => (ResolveDelegateFactory)_data[PERFORMANCEOPTIMIZED_FACTORY].Value!;

        #endregion
    }
}
