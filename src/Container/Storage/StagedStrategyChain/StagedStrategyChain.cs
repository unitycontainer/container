using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Extension;

namespace Unity.Storage
{
    /// <summary>
    /// Represents a chain of builder strategies partitioned by stages.
    /// </summary>
    /// <typeparam name="UnityBuildStage">The stage enumeration to partition the strategies.</typeparam>
    /// <typeparam name="BuilderStrategy"><see cref="Type"/> of strategy</typeparam>
    public partial class StagedStrategyChain : IStagedStrategyChain,
                                               IEnumerable<BuilderStrategy>
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const string ERROR_MESSAGE = "An element with the same key already exists";

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)] 
        private static readonly int _size = Enum.GetNames(typeof(UnityBuildStage)).Length;
        
        private readonly BuilderStrategy?[] _stages = new BuilderStrategy[_size];

        #endregion


        #region Constructors

        public StagedStrategyChain()
            => Type = typeof(UnityBuildStage);

        public StagedStrategyChain(Type type) 
            => Type = type;

        #endregion


        #region Properties

        public Type Type { get; }

        public int Count
            => _stages.Aggregate(0, (a, s) => null == s ? a : a + 1);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsReadOnly => false;

        #endregion


        #region Add

        /// <inheritdoc/>
        public void Add(UnityBuildStage key, BuilderStrategy value)
        {
            ref var position = ref _stages[Convert.ToInt32(key)];
            
            if (null != position) throw new ArgumentException(ERROR_MESSAGE);

            position = value;

            Invalidated?.Invoke(this, Type);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        public void Add(KeyValuePair<UnityBuildStage, BuilderStrategy> item)
            => Add(item.Key, item.Value);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        public void Add((UnityBuildStage, BuilderStrategy) stage)
            => Add(stage.Item1, stage.Item2);


        /// <inheritdoc/>
        public void Add(params KeyValuePair<UnityBuildStage, BuilderStrategy>[] items)
        {
            for (var i = 0; i < items.Length; i++)
            {
                ref var pair = ref items[i];
                ref var position = ref _stages[Convert.ToInt32(pair.Key)];
                
                if (null != position) throw new ArgumentException(ERROR_MESSAGE);

                position = pair.Value;
            }
            
            Invalidated?.Invoke(this, Type);
        }


        /// <inheritdoc/>
        public void Add(params (UnityBuildStage, BuilderStrategy)[] stages)
        {
            for (var i = 0; i < stages.Length; i++)
            {
                ref var pair = ref stages[i];
                ref var position = ref _stages[Convert.ToInt32(pair.Item1)];

                if (null != position) throw new ArgumentException(ERROR_MESSAGE);

                position = pair.Item2;
            }

            Invalidated?.Invoke(this, Type);
        }

        #endregion


        #region Get/Set

        /// <inheritdoc/>
        public bool TryGetValue(UnityBuildStage key, out BuilderStrategy value)
        {
            value = _stages[Convert.ToInt32(key)]!;

            return null != value;
        }


        /// <inheritdoc/>
        public BuilderStrategy this[UnityBuildStage key]
        {
            get => _stages[Convert.ToInt32(key)] ?? throw new KeyNotFoundException();
            set
            {
                _stages[Convert.ToInt32(key)] = value;
                Invalidated?.Invoke(this, Type);
            }
        }

        #endregion


        #region Remove

        /// <inheritdoc/>
        public bool Remove(UnityBuildStage key)
        {
            ref var position = ref _stages[Convert.ToInt32(key)];

            if (null != position)
            {
                position = null;
                Invalidated?.Invoke(this, Type);
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public bool Remove(KeyValuePair<UnityBuildStage, BuilderStrategy> item)
        {
            ref var position = ref _stages[Convert.ToInt32(item.Key)];

            if (item.Value == position)
            {
                position = null;
                Invalidated?.Invoke(this, Type);
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public void Clear()
        {
            for(var i = 0; i < _size; i++) _stages[i] = null;
        }

        #endregion


        #region Contains

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<UnityBuildStage, BuilderStrategy> item) 
            => item.Value == _stages[Convert.ToInt32(item.Key)];


        /// <inheritdoc/>
        public bool ContainsKey(UnityBuildStage key) 
            => null != _stages[Convert.ToInt32(key)];

        #endregion


        #region Change Event

        /// <inheritdoc/>
        public event StagedChainChagedHandler? Invalidated;

        #endregion


        #region Not Supported

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<UnityBuildStage, BuilderStrategy>> IEnumerable<KeyValuePair<UnityBuildStage, BuilderStrategy>>.GetEnumerator() 
            => throw new NotSupportedException();


        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<UnityBuildStage, BuilderStrategy>[] array, int arrayIndex) 
            => throw new NotSupportedException();

        #endregion
    }
}
