using System;
using Unity.Pipeline;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);

    public partial class Defaults : PolicyList
    {
        #region Fields

        object _syncRoot = new object();

        #endregion


        #region Constructors

        internal Defaults() : base(2)
        {
            // Build Chains
            InstancePipeline = new StagedStrategyChain<PipelineProcessor, BuilderStage>();
            FactoryPipeline  = new StagedStrategyChain<PipelineProcessor, BuilderStage>();
            TypePipeline     = new StagedStrategyChain<PipelineProcessor, BuilderStage>();
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
