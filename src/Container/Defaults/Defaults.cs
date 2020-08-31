using System;
using Unity.Pipeline;
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
        protected int Prime = 7;

        [CLSCompliant(false)] protected Policy[] Data;
        [CLSCompliant(false)] protected Metadata[] Meta;

        private readonly int RESOLVE_CONTRACT;
        private readonly int RESOLVE_UNKNOWN;
        private readonly int RESOLVE_ARRAY;

        private readonly int PIPELINE_TYPE;
        private readonly int PIPELINE_FACTORY;
        private readonly int PIPELINE_INSTANCE;

        private readonly int FACTORY_SINGLETON;
        private readonly int FACTORY_BALANCED;
        private readonly int FACTORY_OPTIMIZED;

        #endregion


        #region Constructors

        internal Defaults() 
        {
            // Build Chains
            TypeChain         = new StagedChain<BuilderStage, PipelineProcessor>();
            FactoryChain      = new StagedChain<BuilderStage, PipelineProcessor>();
            InstanceChain     = new StagedChain<BuilderStage, PipelineProcessor>();
            UnregisteredChain = new StagedChain<BuilderStage, PipelineProcessor>();

            // Storage
            Data = new Policy[Storage.Prime.Numbers[Prime]];
            Meta = new Metadata[Storage.Prime.Numbers[++Prime]];

            // Resolvers
            RESOLVE_UNKNOWN  = Allocate(typeof(ResolveUnregisteredDelegate));
            RESOLVE_CONTRACT = Allocate(typeof(ResolveRegistrationDelegate));
            RESOLVE_ARRAY    = Allocate(typeof(ResolveArrayDelegate));

            // Pipelines
            PIPELINE_TYPE     = Allocate(typeof(TypeCategory),     typeof(ResolveDelegate<ResolutionContext>));
            PIPELINE_FACTORY  = Allocate(typeof(FactoryCategory),  typeof(ResolveDelegate<ResolutionContext>));
            PIPELINE_INSTANCE = Allocate(typeof(InstanceCategory), typeof(ResolveDelegate<ResolutionContext>));

            // Factories
            FACTORY_SINGLETON = Allocate(typeof(SingletonFactoryDelegate));
            FACTORY_OPTIMIZED = Allocate(typeof(OptimizedFactoryDelegate));
            FACTORY_BALANCED  = Allocate(typeof(BalancedFactoryDelegate));
        }

        #endregion


        #region Events

        /// <summary>
        /// Event fired when one of the default policies has changed
        /// </summary>
        public DefaultPolicyChangedHandler? DefaultPolicyChanged;

        #endregion
    }
}
