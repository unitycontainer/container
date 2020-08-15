using System;
using Unity.Pipeline;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);


    public partial class Defaults
    {
        #region Fields

        readonly object _syncRoot = new object();

        protected int _count;
        protected int _prime = 2;
        protected Policy[] _data;
        protected Metadata[] _meta;

        private readonly int FACTORY_BALANCED;
        private readonly int FACTORY_SINGLETON;
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
            _data = new Policy[Prime.Numbers[_prime]];
            _meta = new Metadata[Prime.Numbers[++_prime]];

            // Add factory placeholders
            FACTORY_BALANCED     = Allocate(typeof(BalancedPipelineFactory));
            FACTORY_SINGLETON    = Allocate(typeof(SingletonPipelineFactory));
            FACTORY_OPTIMIZED    = Allocate(typeof(OptimizedPipelineFactory));
            FACTORY_UNREGISTERED = Allocate(typeof(UnregisteredPipelineFactory));
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
