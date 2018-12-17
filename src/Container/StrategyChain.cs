using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using Unity.Storage;

namespace Unity.Container
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public class StrategyChain : IStrategyChain
    {
        private readonly BuilderStrategy[] _strategies;

        /// <summary>
        /// Initialize a new instance of the <see cref="StrategyChain"/> class with a collection of strategies.
        /// </summary>
        /// <param name="strategies">A collection of strategies to initialize the chain.</param>
        public StrategyChain(IEnumerable<BuilderStrategy> strategies)
        {
            _strategies = strategies.ToArray();
        }

        /// <summary>
        /// Execute this strategy chain against the given context to build up.
        /// </summary>
        /// <param name="context">Context for the build processes.</param>
        /// <returns>The build up object</returns>
        public void BuildUp<TBuilderContext>(ref TBuilderContext context) where TBuilderContext : IBuilderContext
        {
            var i = -1;

            try
            {
                while (!context.BuildComplete && ++i < _strategies.Length)
                {
                    _strategies[i].PreBuildUp(ref context);
                }

                while (--i >= 0)
                {
                    _strategies[i].PostBuildUp(ref context);
                }
            }
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
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
        IEnumerator<BuilderStrategy> IEnumerable<BuilderStrategy>.GetEnumerator()
        {
            return _strategies.Cast<BuilderStrategy>().GetEnumerator();
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
