﻿using System;
using System.Collections.Generic;

namespace Unity.Storage
{
    /// <summary>
    /// This interface defines a standard method to convert any <see cref="StagedStrategyChain{TStageEnum}"/> regardless
    /// of the stage enum into a regular, flat strategy chain.
    /// </summary>
    public interface IStagedStrategyChain
    {
        public int Version { get; }

        /// <summary>
        /// Signals that chain has been changed
        /// </summary>
        event EventHandler Invalidated;
    }


    /// <summary>
    /// This interface defines a standard method to create multi staged strategy chain.
    /// </summary>
    /// <typeparam name="TStrategyType">The type of a strategy</typeparam>
    /// <typeparam name="TStageEnum">The stage enum</typeparam>
    public interface IStagedStrategyChain<TStrategyType, TStageEnum> : IStagedStrategyChain, 
                                                                       IDictionary<TStageEnum, TStrategyType>
        where TStrategyType : class
        where TStageEnum    : Enum
    {
        /// <summary>
        /// Adds a strategy to the chain at a particular stage.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
        /// <param name="stage">The stage to add the strategy.</param>
        void Add(TStrategyType strategy, TStageEnum stage);
        
        public void Add(params (TStageEnum, TStrategyType)[] stages);
    }
}
