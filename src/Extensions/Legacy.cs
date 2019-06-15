namespace Unity.Extension
{
    public class Legacy : UnityContainerExtension
    {
        protected override void Initialize()
        {
            //var strategies = (StagedStrategyChain<PipelineProcessor, BuilderStage>)Context.BuildPlanStrategies;
            //var processor = (ConstructorProcessor)strategies.First(s => s is ConstructorProcessor);

            //processor.SelectMethod = processor.LegacySelector;

            // TODO: Requires reimplementation
        }
    }
}
