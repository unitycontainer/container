namespace Unity.Extension
{
    public static class ExtensionContextExtensions
    {
        public static void Add(this IStagedStrategyChain chain, BuilderStrategy strategy, UnityBuildStage stage)
        {
            chain.Add(stage, strategy);
        }
    }
}
