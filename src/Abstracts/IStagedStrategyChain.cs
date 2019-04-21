using System;

namespace Unity.Storage
{
    /// <summary>
    /// This interface defines a standard method to create multi staged strategy chain.
    /// </summary>
    /// <typeparam name="TStrategyType">The <see cref="System.Type"/> of strategy</typeparam>
    /// <typeparam name="TStageEnum">The stage enum</typeparam>
    public interface IStagedStrategyChain<in TStrategyType, in TStageEnum>
    {

        /// <summary>
        /// Adds a strategy to the chain at a particular stage.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
        /// <param name="stage">The stage to add the strategy.</param>
        void Add(TStrategyType strategy, TStageEnum stage);

        /// <summary>
        /// Signals that chain has been changed
        /// </summary>
        event EventHandler<EventArgs> Invalidated;
    }


    public static class StagedStrategyChainExtensions
    {
        /// <summary>
        /// Add a new strategy for the <paramref name="stage"/>.
        /// </summary>
        /// <typeparam name="TStrategy">The <see cref="System.Type"/> of strategy</typeparam>
        /// <typeparam name="TStageEnum">The stage enum</typeparam>
        /// <param name="chain">The chain this strategy is added to.</param>
        /// <param name="stage">The stage to add the strategy to.</param>
        public static void AddNew<TStrategy, TStageEnum>(this IStagedStrategyChain<TStrategy, TStageEnum> chain, TStageEnum stage)
            where TStrategy : new()
        {
            chain.Add(new TStrategy(), stage);
        }
    }
}
