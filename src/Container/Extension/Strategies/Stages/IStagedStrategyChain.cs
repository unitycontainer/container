using System;
using System.Collections.Generic;
using Unity.Container;

namespace Unity.Extension
{
    /// <summary>
    /// Represents the method that will handle a <see cref="IStagedStrategyChain.Invalidated"/> event
    /// </summary>
    /// <remarks>In normal circumstances the monitoring subscriber does not care what has 
    /// changed. Details of the change are not important, just the fact that change has happened</remarks>
    /// <param name="chain">The chain that has been changed</param>
    /// <param name="type">The marker <see cref="Type"/> of the changed chain, one of 
    /// <see cref="Policies.CategoryType"/>, <see cref="Policies.CategoryFactory"/>, or 
    /// <see cref="Policies.CategoryInstance"/> types</param>
    public delegate void StagedChainChagedHandler(IStagedStrategyChain sender, Type type);


    /// <summary>
    /// This interface provides a method to create multi staged strategy chains
    /// </summary>
    public interface IStagedStrategyChain : IDictionary<UnityBuildStage, BuilderStrategy>
    {
        /// <summary>
        /// Signals that the chain has been changed
        /// </summary>
        event StagedChainChagedHandler Invalidated;

        /// <summary>
        /// Add a stage
        /// </summary>
        /// <param name="stage"><see cref="ValueTuple"/> pairs to add</param>
        void Add((UnityBuildStage, BuilderStrategy) stage);

        /// <summary>
        /// Add a range of build stages
        /// </summary>
        /// <remarks>
        /// This method adds stages and fires notifications only once
        /// </remarks>
        /// <param name="stages">Array of <see cref="ValueTuple"/> pairs to add</param>
        void Add((UnityBuildStage, BuilderStrategy)[] stages);
    }


    public static class StagedStrategyChainExtensions
    {
        /// <summary>
        /// Add a new strategy for the <paramref name="stage"/>.
        /// </summary>
        /// <typeparam name="TStrategy">The <see cref="System.Type"/> of strategy</typeparam>
        /// <param name="chain">The chain this strategy is added to.</param>
        /// <param name="stage">The stage to add the strategy to.</param>
        public static void AddNew<TStrategy>(this IStagedStrategyChain chain, UnityBuildStage stage)
            where TStrategy : BuilderStrategy, new()
        {
            chain.Add(stage, new TStrategy());
        }


        /// <summary>
        /// Add a stage
        /// </summary>
        /// <param name="stage"><see cref="ValueTuple"/> pairs to add</param>
        public static void Add(this IStagedStrategyChain chain, (BuilderStrategy, UnityBuildStage) stage)
            => chain.Add(stage.Item2, stage.Item1);
    }
}
