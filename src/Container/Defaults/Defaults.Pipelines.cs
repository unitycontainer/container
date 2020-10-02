using Unity.Lifetime;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public partial class Defaults
    {
        #region Constants

        public static ResolveDelegate<PipelineContext> DefaulRequiredResolver
            = (ref PipelineContext context) => context.Resolve();

        public static ResolveDelegate<PipelineContext> DefaulOptionalResolver
            = (ref PipelineContext context) => context.Resolve();

        #endregion


        #region Chains

        public StagedChain<BuildStage, PipelineProcessor> TypeChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> FactoryChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> InstanceChain { get; }

        public StagedChain<BuildStage, PipelineProcessor> UnregisteredChain { get; }

        #endregion


        #region Pipeline Builder
        
        public PipelineFactory BuildPipeline 
            => (PipelineFactory)Data[BUILD_PIPELINE].Value!;


        #endregion


        #region Resolution

        public RegistrationProducerDelegate ResolveContract
            => (RegistrationProducerDelegate)Data[RESOLVE_CONTRACT].Value!;

        public ResolveUnregisteredDelegate ResolveUnregistered
            => (ResolveUnregisteredDelegate)Data[RESOLVE_UNKNOWN].Value!;

        public ResolveMappedDelegate ResolveMapped
            => (ResolveMappedDelegate) Data[RESOLVE_MAPPED].Value!;

        public ResolveArrayDelegate ResolveArray
            => (ResolveArrayDelegate)Data[RESOLVE_ARRAY].Value!;

        #endregion


        #region Activators

        /// <summary>
        /// Resolve object with <see cref="ResolutionStyle.OnceInLifetime"/> lifetime and
        /// <see cref="RegistrationCategory.Type"/> registration
        /// </summary>
        public ResolveDelegate<PipelineContext> TypePipeline 
            => (ResolveDelegate<PipelineContext>)Data[PIPELINE_TYPE].Value!;

        /// <summary>
        /// Resolve object with <see cref="ResolutionStyle.OnceInLifetime"/> lifetime and
        /// <see cref="RegistrationCategory.Instance"/> registration
        /// </summary>
        public ResolveDelegate<PipelineContext> InstancePipeline 
            => (ResolveDelegate<PipelineContext>)Data[PIPELINE_INSTANCE].Value!;

        /// <summary>
        /// Resolve object with <see cref="ResolutionStyle.OnceInLifetime"/> lifetime and
        /// <see cref="RegistrationCategory.Factory"/> registration
        /// </summary>
        public ResolveDelegate<PipelineContext> FactoryPipeline 
            => (ResolveDelegate<PipelineContext>)Data[PIPELINE_FACTORY].Value!;

        #endregion


        #region Factories

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.OnceInWhile"/> lifetime
        /// </summary>
        public BalancedPipelineFactory BalancedFactory
            => (BalancedPipelineFactory)Data[FACTORY_BALANCED].Value!;

        /// <summary>
        /// Create resolution pipeline for <see cref="ResolutionStyle.EveryTime"/> lifetime
        /// </summary>
        public OptimizedPipelineFactory OptimizedFactory
            => (OptimizedPipelineFactory)Data[FACTORY_OPTIMIZED].Value!;


        /// <summary>
        /// Create resolution pipeline for unregistered type
        /// </summary>
        public UnregisteredPipelineFactory UnregisteredFactory
            => (UnregisteredPipelineFactory)Data[FACTORY_UNREGISTERED].Value!;

        #endregion
    }
}
