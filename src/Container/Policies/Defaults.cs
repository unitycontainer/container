using Unity.Storage;

namespace Unity.Container
{
    public partial class Policies<TProcessor, TStage>
    {
        #region Pipelines

        public StagedStrategyChain<TProcessor, TStage> InstancePipeline { get; }
        public StagedStrategyChain<TProcessor, TStage> FactoryPipeline { get; }
        public StagedStrategyChain<TProcessor, TStage> TypePipeline { get; }

        #endregion
    }
}
