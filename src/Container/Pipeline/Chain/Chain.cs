using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Extension;

namespace Unity.Container
{
    /// <summary>
    /// Represents a chain of builder strategies partitioned by stages.
    /// </summary>
    /// <typeparam name="UnityBuildStage">The stage enumeration to partition the strategies.</typeparam>
    /// <typeparam name="BuilderStrategy"><see cref="Type"/> of strategy</typeparam>
    public partial class StagedStrategyChain : IStagedStrategyChain,
                                               IEnumerable<BuilderStrategy>
    {
        #region Constants

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string ERROR_MESSAGE = "An element with the same key already exists";

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)] 
        private static readonly int _size = Enum.GetNames(typeof(UnityBuildStage)).Length;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsReadOnly => false;

        #endregion


        #region Fields

        private BuilderStrategy[]? _cache;
        private readonly Entry[]  _stages = new Entry[_size];

        #endregion


        #region Constructors

        public StagedStrategyChain()
            => Type = typeof(Type);

        public StagedStrategyChain(Type type) 
            => Type = type;

        #endregion


        #region Properties

        public Type Type { get; }

        public int Count { get; protected set; }

        public int Version { get; protected set; }

        #endregion


        #region Change Event

        /// <inheritdoc/>
        public event StagedChainChagedHandler Invalidated
            = (sender, type) => ((StagedStrategyChain)sender)._cache = null;

        #endregion


        public BuilderStrategy[] ToArray()
            => _cache ??= ((IEnumerable<BuilderStrategy>)this).ToArray();
    }
}
