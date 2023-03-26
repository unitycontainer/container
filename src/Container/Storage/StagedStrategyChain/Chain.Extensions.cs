using Unity.Builder;
using Unity.Strategies;

namespace Unity.Storage
{
    public static class StagedStrategyChainExtensions
    {
        /// <summary>
        /// Add a new strategy for the <paramref name="stage"/>.
        /// </summary>
        /// <typeparam name="TStrategy">The <see cref="System.Type"/> of strategy</typeparam>
        /// <param name="chain">The chain this strategy is added to.</param>
        /// <param name="stage">The stage to add the strategy to.</param>
        public static void AddNew<TStrategy>(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain, UnityBuildStage stage)
            where TStrategy : BuilderStrategy, new()
        {
            chain.Add(stage, new TStrategy());
        }


        /// <summary>
        /// Add a stage
        /// </summary>
        /// <param name="stage"><see cref="ValueTuple"/> pairs to add</param>
        public static void Add(this IStagedStrategyChain<BuilderStrategy, UnityBuildStage> chain, (BuilderStrategy, UnityBuildStage) stage)
            => chain.Add(stage.Item2, stage.Item1);
    }
}
