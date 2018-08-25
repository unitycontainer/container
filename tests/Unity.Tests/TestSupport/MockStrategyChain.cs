using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Strategy;

namespace Unity.Tests.v5.TestSupport
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies.
    /// </summary>
    public class MockStrategyChain : IStrategyChain
    {
        private readonly List<BuilderStrategy> _strategies = new List<BuilderStrategy>();

        /// <summary>
        /// Initialize a new instance of the <see cref="MockStrategyChain"/> class.
        /// </summary>
        public MockStrategyChain() { }

        /// <summary>
        /// Initialize a new instance of the <see cref="MockStrategyChain"/> class with a collection of strategies.
        /// </summary>
        /// <param name="strategies">A collection of strategies to initialize the chain.</param>
        public MockStrategyChain(IEnumerable strategies)
        {
            AddRange(strategies);
        }

        /// <summary>
        /// Adds a strategy to the chain.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
        public void Add(BuilderStrategy strategy)
        {
            _strategies.Add(strategy);
        }

        /// <summary>
        /// Adds strategies to the chain.
        /// </summary>
        /// <param name="strategyEnumerable">The strategies to add to the chain.</param>
        public void AddRange(IEnumerable strategyEnumerable)
        {
            foreach (BuilderStrategy strategy in strategyEnumerable 
                                               ?? throw new ArgumentNullException(nameof(strategyEnumerable)))
            {
                Add(strategy);
            }
        }

        /// <summary>
        /// Reverse the order of the strategy chain.
        /// </summary>
        /// <returns>The reversed strategy chain.</returns>
        public IStrategyChain Reverse()
        {
            List<BuilderStrategy> reverseList = new List<BuilderStrategy>(_strategies);
            reverseList.Reverse();
            return new MockStrategyChain(reverseList);
        }

        /// <summary>
        /// Execute this strategy chain against the given context to build up.
        /// </summary>
        /// <param name="context">Context for the build processes.</param>
        /// <returns>The build up object</returns>
        public void BuildUp(IBuilderContext builderContext)
        {
            var context = builderContext ?? throw new ArgumentNullException(nameof(builderContext));
            int i = 0;

            try
            {
                for (; i < _strategies.Count; ++i)
                {
                    if (context.BuildComplete)
                    {
                        break;
                    }
                    _strategies[i].PreBuildUp(context);
                }

                if (context.BuildComplete)
                {
                    --i; // skip shortcutting strategy's post
                }

                for (--i; i >= 0; --i)
                {
                    _strategies[i].PostBuildUp(context);
                }
            }
            catch (Exception)
            {
                context.RequiresRecovery?.Recover();
                throw;
            }
        }

        #region IEnumerable<IBuilderStrategy> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<BuilderStrategy> IEnumerable<BuilderStrategy>.GetEnumerator()
        {
            return _strategies.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

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
