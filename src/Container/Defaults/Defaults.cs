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

            // Add factories
            NONOPTIMIZEDPIPELINE_FACTORY = Add(typeof(NonOptimizedPipelineFactory), 
                                              typeof(ResolveDelegateFactory), 
                (ResolveDelegateFactory)Container.NonOptimizedPipelineFactory.Factory);

            PIPECREATIONOPTIMIZED_FACTORY = Add(typeof(PipeCreationOptimizedFactory), 
                                               typeof(ResolveDelegateFactory), 
                (ResolveDelegateFactory)Container.PipeCreationOptimizedFactory.Factory);

            PERFORMANCEOPTIMIZED_FACTORY = Add(typeof(PerformanceOptimizedFactory), 
                                               typeof(ResolveDelegateFactory),  
                (ResolveDelegateFactory)Container.PerformanceOptimizedFactory.Factory);
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
