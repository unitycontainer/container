// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Strategy;

namespace Unity.Container
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public class StrategyChain : IStrategyChain
    {
        private readonly IBuilderStrategy[] _strategies;

        /// <summary>
        /// Initialize a new instance of the <see cref="StrategyChain"/> class with a collection of strategies.
        /// </summary>
        /// <param name="strategies">A collection of strategies to initialize the chain.</param>
        public StrategyChain(IEnumerable<IBuilderStrategy> strategies)
        {
            _strategies = strategies.ToArray();
        }

        /// <summary>
        /// Execute this strategy chain against the given context to build up.
        /// </summary>
        /// <param name="builderContext">Context for the build processes.</param>
        /// <returns>The build up object</returns>
        public void BuildUp(IBuilderContext builderContext)
        {
            var context = builderContext ?? throw new ArgumentNullException(nameof(builderContext));
            var i = 0;

            try
            {
                while (!context.BuildComplete && i < _strategies.Length)
                {
                    _strategies[i++].PreBuildUp(context);
                }

                while (--i >= 0)
                {
                    _strategies[i].PostBuildUp(context);
                }
            }
            catch (Exception)
            {
                context.RecoveryStack.ExecuteRecovery();
                throw;
            }
        }

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<IBuilderStrategy> IEnumerable<IBuilderStrategy>.GetEnumerator()
        {
            return _strategies.Cast<IBuilderStrategy>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        ///
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return _strategies.GetEnumerator();
        }

        #endregion
    }
}
