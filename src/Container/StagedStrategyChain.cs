// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder.Strategy;
using Unity.Strategy;

namespace Unity.Container
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies partitioned by stages.
    /// </summary>
    /// <typeparam name="TStageEnum">The stage enumeration to partition the strategies.</typeparam>
    public class StagedStrategyChain<TStageEnum> : IStagedStrategyChain<IBuilderStrategy, TStageEnum>, 
                                                   IEnumerable<IBuilderStrategy>,
                                                   IDisposable
    {
        #region Fields

        private readonly object _lockObject = new object();
        private readonly StagedStrategyChain<TStageEnum> _innerChain;
        private readonly IList<IBuilderStrategy>[] _stages = 
            new IList<IBuilderStrategy>[typeof(TStageEnum).GetTypeInfo()
                                                          .DeclaredFields
                                                          .Count(f => f.IsPublic && f.IsStatic)];

        private IStrategyChain _cache;

        #endregion


        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="StagedStrategyChain{TStageEnum}"/> class.
        /// </summary>
        public StagedStrategyChain()
            : this(null)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="StagedStrategyChain{TStageEnum}"/> class with an inner strategy chain to use when building.
        /// </summary>
        /// <param name="innerChain">The inner strategy chain to use first when finding strategies in the build operation.</param>
        public StagedStrategyChain(StagedStrategyChain<TStageEnum> innerChain)
        {
            if (null != innerChain)
            {
                _innerChain = innerChain;
                _innerChain.Invalidated += OnParentInvalidated;
            }

            for (var i = 0; i < _stages.Length; ++i)
            {
                _stages[i] = new List<IBuilderStrategy>();
            }
        }

        #endregion


        #region Implementation

        private void OnParentInvalidated(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _cache = null;
            }
        }

        private IEnumerable<IBuilderStrategy> Enumerate(int i)
        {
            return (_innerChain?.Enumerate(i) ?? Enumerable.Empty<IBuilderStrategy>()).Concat(_stages[i]);
        }

        #endregion


        #region IStagedStrategyChain


        /// <summary>
        /// Signals that chain has been changed
        /// </summary>
        public event EventHandler<EventArgs> Invalidated;

        /// <summary>
        /// Adds a strategy to the chain at a particular stage.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
        /// <param name="stage">The stage to add the strategy.</param>
        public void Add(IBuilderStrategy strategy, TStageEnum stage)
        {
            lock (_lockObject)
            {
                _stages[Convert.ToInt32(stage)].Add(strategy);
                _cache = null;
                Invalidated?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Clear the current strategy chain list.
        /// </summary>
        /// <remarks>
        /// This will not clear the inner strategy chain if this instance was created with one.
        /// </remarks>
        public void Clear()
        {
            lock (_lockObject)
            {
                foreach (var list in _stages)
                {
                    ((List<IBuilderStrategy>)list).Clear();
                }
                _cache = null;
                Invalidated?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Makes a strategy chain based on this instance.
        /// </summary>
        /// <returns>A new <see cref="StrategyChain"/>.</returns>
        public IStrategyChain MakeStrategyChain()
        {
            lock (_lockObject)
            {
                if (null == _cache)
                {
                    _cache = new StrategyChain(this);
                }

                return _cache;
            }
        }


        #endregion


        #region IEnumerable

        public IEnumerator<IBuilderStrategy> GetEnumerator()
        {
            return Enumerable.Range(0, _stages.Length)
                             .SelectMany(Enumerate)
                             .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            if (null != _innerChain)
            {
                _innerChain.Invalidated -= OnParentInvalidated;
            }
        }

        #endregion
    }
}
