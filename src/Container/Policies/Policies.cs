using System;
using Unity.Storage;

namespace Unity.Container
{
    public delegate void DefaultPolicyChangedHandler(Type type, object? value);

    public partial class Policies<TProcessor, TStage> : PolicyList
    {
        #region Fields

        object _syncRoot = new object();

        #endregion


        #region Constructors

        public Policies() : base(2)
        {
            // Build Chains
            InstancePipeline = new StagedStrategyChain<TProcessor, TStage>();
            FactoryPipeline  = new StagedStrategyChain<TProcessor, TStage>();
            TypePipeline     = new StagedStrategyChain<TProcessor, TStage>();
        }

        #endregion


        #region Pipelines

        public StagedStrategyChain<TProcessor, TStage> InstancePipeline { get; }
        public StagedStrategyChain<TProcessor, TStage> FactoryPipeline { get; }
        public StagedStrategyChain<TProcessor, TStage> TypePipeline { get; }

        #endregion


        #region Events

        /// <summary>
        /// Event fired when one of the default policies has changed
        /// </summary>
        public DefaultPolicyChangedHandler? DefaultPolicyChanged;

        #endregion
    }
}
