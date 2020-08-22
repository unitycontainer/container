using Unity.Pipeline;
using Unity.Storage;
using Unity.Lifetime;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Chains

        public StagedChain<BuilderStage, PipelineProcessor> TypeChain { get; }

        public StagedChain<BuilderStage, PipelineProcessor> FactoryChain { get; }

        public StagedChain<BuilderStage, PipelineProcessor> InstanceChain { get; }

        public StagedChain<BuilderStage, PipelineProcessor> UnregisteredChain { get; }

        #endregion


        #region Factories

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.OnceInLifetime"/> lifetime
        /// </summary>
        public ResolveDelegateFactory DelegateFactory
            => (ResolveDelegateFactory)_data[FACTORY_DELEGATE].Value!;

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.OnceInLifetime"/> lifetime
        /// </summary>
        public SingletonPipelineFactory SingletonPipeline
            => (SingletonPipelineFactory)_data[FACTORY_SINGLETON].Value!;
        
        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.OnceInAWhile"/> lifetime
        /// </summary>
        public BalancedPipelineFactory BalancedPipeline
            => (BalancedPipelineFactory)_data[FACTORY_BALANCED].Value!;

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.EveryTime"/> lifetime
        /// </summary>
        public OptimizedPipelineFactory OptimizedPipeline
            => (OptimizedPipelineFactory)_data[FACTORY_OPTIMIZED].Value!;

        /// <summary>
        /// Crate resolution pipeline for unregistered <see cref="Type"/>
        /// </summary>
        public UnregisteredPipelineFactory UnregisteredPipeline
            => (UnregisteredPipelineFactory)_data[FACTORY_UNREGISTERED].Value!;

        #endregion
    }
}
