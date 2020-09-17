using System;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);

    public partial class Defaults
    {
        #region Fields

        readonly object _syncRoot = new object();

        protected int Count;
        protected int Prime = 5;

        [CLSCompliant(false)] protected Policy[] Data;
        [CLSCompliant(false)] protected Metadata[] Meta;

        private readonly int BUILD_PIPELINE;

        private readonly int RESOLVE_CONTRACT;
        private readonly int RESOLVE_UNKNOWN;
        private readonly int RESOLVE_MAPPED;
        private readonly int RESOLVE_ARRAY;

        private readonly int PIPELINE_TYPE;
        private readonly int PIPELINE_FACTORY;
        private readonly int PIPELINE_INSTANCE;

        private readonly int FACTORY_SINGLETON;
        private readonly int FACTORY_BALANCED;
        private readonly int FACTORY_OPTIMIZED;
        private readonly int FACTORY_UNREGISTERED;

        #endregion


        #region Constructors

        internal Defaults()
        {
            // Build Chains
            TypeChain = new StagedChain<BuildStage, PipelineProcessor>();
            FactoryChain = new StagedChain<BuildStage, PipelineProcessor>();
            InstanceChain = new StagedChain<BuildStage, PipelineProcessor>();
            UnregisteredChain = new StagedChain<BuildStage, PipelineProcessor>();

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Factories
            BUILD_PIPELINE    = Allocate(typeof(PipelineFactory));

            FACTORY_SINGLETON    = Allocate(typeof(SingletonPipelineFactory));
            FACTORY_OPTIMIZED    = Allocate(typeof(OptimizedPipelineFactory));
            FACTORY_BALANCED     = Allocate(typeof(BalancedPipelineFactory));
            FACTORY_UNREGISTERED = Allocate(typeof(UnregisteredPipelineFactory));

            // Pipelines
            PIPELINE_TYPE = Allocate(typeof(TypeCategory), typeof(Pipeline));
            PIPELINE_FACTORY = Allocate(typeof(FactoryCategory), typeof(Pipeline));
            PIPELINE_INSTANCE = Allocate(typeof(InstanceCategory), typeof(Pipeline));

            // Resolvers
            RESOLVE_CONTRACT = Allocate(typeof(RegistrationProducerDelegate));
            RESOLVE_UNKNOWN = Allocate(typeof(ResolveUnregisteredDelegate));
            RESOLVE_MAPPED = Allocate(typeof(ResolveMappedDelegate));
            RESOLVE_ARRAY = Allocate(typeof(ResolveArrayDelegate));
        }

        #endregion
    }
}
