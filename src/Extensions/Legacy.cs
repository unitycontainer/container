using System.Linq;
using Unity.Builder;
using Unity.Processors;
using Unity.Storage;

namespace Unity.Extension
{
    public class Legacy : UnityContainerExtension
    {
        protected override void Initialize()
        {
            var strategies = (StagedStrategyChain<MemberProcessor, BuilderStage>)Context.BuildPlanStrategies;
            var processor = (ConstructorProcessor)strategies.First(s => s is ConstructorProcessor);

            processor.SelectMethod = processor.LegacySelector;
        }
    }
}
