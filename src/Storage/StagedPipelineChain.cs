using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Storage
{
    /// <summary>
    /// Represents a chain of responsibility for builder strategies partitioned by stages.
    /// </summary>
    /// <typeparam name="TStageEnum">The stage enumeration to partition the strategies.</typeparam>
    /// <typeparam name="Pipeline"></typeparam>
    public class StagedPipelineChain<TStageEnum> : IStagedStrategyChain<Pipeline, TStageEnum>
    {
        #region Fields

        private static readonly int _size = typeof(TStageEnum).GetTypeInfo().DeclaredFields.Count(f => f.IsPublic && f.IsStatic);
        private readonly object _lockObject = new object();
        private readonly StagedPipelineChain<TStageEnum>? _innerChain;
        private readonly IList<Pipeline>[] _stages = new IList<Pipeline>[_size];

        private Pipeline[]? _cache;

        #endregion


        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="StagedStrategyChain{Pipeline,TStageEnum}"/> class.
        /// </summary>
        public StagedPipelineChain()
        {
            for (var i = 0; i < _stages.Length; ++i)
            {
                _stages[i] = new List<Pipeline>();
            }
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="StagedStrategyChain{Pipeline, TStageEnum}"/> class with an inner strategy chain to use when building.
        /// </summary>
        /// <param name="innerChain">The inner strategy chain to use first when finding strategies in the build operation.</param>
        public StagedPipelineChain(IStagedStrategyChain<Pipeline,TStageEnum>? innerChain = null)
        {
            if (null != innerChain)
            {
                _innerChain = (StagedPipelineChain<TStageEnum>)innerChain;
                _innerChain.Invalidated += OnParentInvalidated;
            }

            for (var i = 0; i < _stages.Length; ++i)
            {
                _stages[i] = new List<Pipeline>();
            }
        }

        #endregion


        #region Implementation

        private void OnParentInvalidated(object? sender, EventArgs e)
        {
            lock (_lockObject)
            {
                _cache = null;
            }
        }

        private IEnumerable<Pipeline> Enumerate(int i)
        {
            return (_innerChain?.Enumerate(i) ?? Enumerable.Empty<Pipeline>()).Concat(_stages[i]);
        }

        #endregion


        #region IStagedStrategyChain


        /// <summary>
        /// Signals that chain has been changed
        /// </summary>
        public event EventHandler<EventArgs>? Invalidated;

        /// <summary>
        /// Adds a strategy to the chain at a particular stage.
        /// </summary>
        /// <param name="strategy">The strategy to add to the chain.</param>
        /// <param name="stage">The stage to add the strategy.</param>
        public void Add(Pipeline strategy, TStageEnum stage)
        {
            lock (_lockObject)
            {
                _stages[Convert.ToInt32(stage)].Add(strategy);
                _cache = null;
                Invalidated?.Invoke(this, new EventArgs());
            }
        }

        #endregion


        #region IEnumerable

        public Pipeline[] ToArray()
        {
            var cache = _cache;
            if (null != cache) return cache;

            lock (_lockObject)
            {
                if (null == _cache)
                {
                    _cache = Enumerable.Range(0, _stages.Length)
                        .SelectMany(Enumerate)
                        .ToArray();
                }

                cache = _cache;
            }
            return cache;
        }

        public IEnumerator<Pipeline> GetEnumerator()
        {
            var cache = _cache;
            if (null == cache)
            {
                lock (_lockObject)
                {
                    if (null == _cache)
                    {
                        _cache = Enumerable.Range(0, _stages.Length)
                                           .SelectMany(Enumerate)
                                           .ToArray();
                    }
                    cache = _cache;
                }
            }

            foreach (var strategyType in cache)
            {
                yield return strategyType;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
