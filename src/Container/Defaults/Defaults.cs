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
            InstancePipeline = new StagedStrategyChain<PipelineProcessor, BuilderStage>();
            FactoryPipeline  = new StagedStrategyChain<PipelineProcessor, BuilderStage>();
            TypePipeline     = new StagedStrategyChain<PipelineProcessor, BuilderStage>();

            // Storage
            _data = new Policy[Prime.Numbers[_prime]];
            _meta = new Metadata[Prime.Numbers[++_prime]];
        }

        #endregion


        #region Pipelines

        public StagedStrategyChain<PipelineProcessor, BuilderStage> InstancePipeline { get; }

        public StagedStrategyChain<PipelineProcessor, BuilderStage> FactoryPipeline { get; }

        public StagedStrategyChain<PipelineProcessor, BuilderStage> TypePipeline { get; }

        #endregion


        #region Events

        /// <summary>
        /// Event fired when one of the default policies has changed
        /// </summary>
        public DefaultPolicyChangedHandler? DefaultPolicyChanged;

        #endregion
    }
}
