using Unity.Builder;
using Unity.Storage;
using Unity.Strategies;

namespace Unity.Extension
{
    public static class ExtensionContextExtensions
    {
        public static void Add(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain, BuilderStrategy strategy, UnityBuildStage stage)
        {
            chain.Add(stage, strategy);
        }
    }
}
