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
        protected int Prime = 2;

        [CLSCompliant(false)] protected Policy[] Data;
        [CLSCompliant(false)] protected Metadata[] Meta;

        private readonly int PIPELINE_TYPE;
        private readonly int PIPELINE_FACTORY;
        private readonly int PIPELINE_INSTANCE;

        private readonly int FACTORY_BALANCED;
        private readonly int FACTORY_OPTIMIZED;
        private readonly int FACTORY_UNREGISTERED;

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

            // Activation pipeline
            PIPELINE_TYPE     = Allocate(typeof(TypeCategory),     typeof(ResolveDelegate<ResolveContext>));
            PIPELINE_FACTORY  = Allocate(typeof(FactoryCategory),  typeof(ResolveDelegate<ResolveContext>));
            PIPELINE_INSTANCE = Allocate(typeof(InstanceCategory), typeof(ResolveDelegate<ResolveContext>));

            // Add factory placeholders
            FACTORY_UNREGISTERED = Allocate(typeof(ResolveDelegateFactory));
            FACTORY_OPTIMIZED    = Allocate(typeof(OptimizedFactoryDelegate));
            FACTORY_BALANCED     = Allocate(typeof(BalancedFactoryDelegate));
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
