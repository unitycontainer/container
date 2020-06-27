using Unity.Extension;
using Unity.Pipeline;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private StagedStrategyChain<PipelineProcessor, PipelineStage> FactoryPipeline { get; }
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
        private StagedStrategyChain<PipelineProcessor, PipelineStage> InstancePipeline { get; }
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();
        private StagedStrategyChain<PipelineProcessor, PipelineStage> TypePipeline { get; }
            = new StagedStrategyChain<PipelineProcessor, PipelineStage>();

        private event RegistrationEvent? TypeRegistered;
        private event RegistrationEvent? InstanceRegistered;
        private event RegistrationEvent? FactoryRegistered;
        private event ChildCreatedEvent? ChildContainerCreated;

        #endregion
    }
}
