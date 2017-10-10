// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity.Builder;
using Unity.Builder.Strategy;

namespace Unity.Strategy
{
    public interface IStagedStrategyChain
    {

        /// <summary>
        /// Convert this <see cref="IStagedStrategyChain{TStageEnum}"/> into
        /// a flat <see cref="IStrategyChain"/>.
        /// </summary>
        /// <returns>The flattened <see cref="IStrategyChain"/>.</returns>
        IStrategyChain MakeStrategyChain();
    }



    /// <summary>
    /// This interface defines a standard method to convert any <see cref="StagedStrategyChain{TStageEnum}"/> regardless
    /// of the stage enum into a regular, flat strategy chain.
    /// </summary>
    public interface IStagedStrategyChain<TStageEnum> : IStagedStrategyChain
    {

        /// <summary>
        /// Adds a strategy to the chain at a particular stage.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
        /// <param name="stage">The stage to add the strategy.</param>
        void Add(IBuilderStrategy strategy, TStageEnum stage);
    }



    public static class StagedStrategyChainExtensions
    {
        /// <summary>
        /// Add a new strategy for the <paramref name="stage"/>.
        /// </summary>
        /// <typeparam name="TStrategy">The <see cref="System.Type"/> of <see cref="IBuilderStrategy"/></typeparam>
        /// <param name="chain"></param>
        /// <param name="stage">The stage to add the strategy.</param>
        public static void AddNew<TStrategy>(this IStagedStrategyChain<UnityBuildStage> chain, UnityBuildStage stage)
            where TStrategy : IBuilderStrategy, new()
        {
            chain.Add(new TStrategy(), stage);
        }
    }
}
