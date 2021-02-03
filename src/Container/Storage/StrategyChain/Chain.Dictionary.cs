using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Extension;


namespace Unity.Container
{
    public partial class StagedStrategyChain
    {
        #region Add

        /// <inheritdoc/>
        public void Add(UnityBuildStage key, BuilderStrategy value)
        {
            if (_stages[Convert.ToInt32(key)].Strategy is not null) throw new ArgumentException(ERROR_MESSAGE);

            Count += 1;
            _stages[Convert.ToInt32(key)] = new Entry(value);

            Version += 1;
            Invalidated.Invoke(this, Type);
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
                
                if (position.Strategy is not null) throw new ArgumentException(ERROR_MESSAGE);

                Count += 1;
                position = new Entry(pair.Value); 
            }
            
            Version += 1;
            Invalidated.Invoke(this, Type);
        }


        /// <inheritdoc/>
        public void Add(params (UnityBuildStage, BuilderStrategy)[] stages)
        {
            for (var i = 0; i < stages.Length; i++)
            {
                ref var pair = ref stages[i];
                ref var position = ref _stages[Convert.ToInt32(pair.Item1)];

                if (position.Strategy is not null) throw new ArgumentException(ERROR_MESSAGE);

                Count += 1;
                position = new Entry(pair.Item2);
            }

            Version += 1;
            Invalidated.Invoke(this, Type);
        }

        #endregion


        #region Get/Set

        /// <inheritdoc/>
        public bool TryGetValue(UnityBuildStage key, out BuilderStrategy value)
        {
            value = _stages[Convert.ToInt32(key)].Strategy!;

            return null != value;
        }


        /// <inheritdoc/>
        public BuilderStrategy this[UnityBuildStage key]
        {
            get => _stages[Convert.ToInt32(key)].Strategy ?? throw new KeyNotFoundException();
            set
            {
                if (value is null) throw new ArgumentNullException("Invalid BuilderStrategy");
                ref var position = ref _stages[Convert.ToInt32(key)];
                if (position.Strategy is null) Count += 1;
                position = new Entry(value);
                Version += 1;
                Invalidated.Invoke(this, Type);
            }
        }

        #endregion


        #region Remove

        /// <inheritdoc/>
        public bool Remove(UnityBuildStage key)
        {
            ref var position = ref _stages[Convert.ToInt32(key)];

            if (position.Strategy is not null)
            {
                Count -= 1;
                position = default;
                Version += 1;
                Invalidated.Invoke(this, Type);
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public bool Remove(KeyValuePair<UnityBuildStage, BuilderStrategy> item)
        {
            ref var position = ref _stages[Convert.ToInt32(item.Key)];

            if (item.Value == position.Strategy)
            {
                Count -= 1;
                position = default;
                Version += 1;
                Invalidated.Invoke(this, Type);
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public void Clear()
        {
            for (var i = 0; i < _size; i++) _stages[i] = default;
            
            Count = 0;
            Version += 1;
        }

        #endregion


        #region Contains

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<UnityBuildStage, BuilderStrategy> item) 
            => item.Value == _stages[Convert.ToInt32(item.Key)].Strategy;


        /// <inheritdoc/>
        public bool ContainsKey(UnityBuildStage key) 
            => null != _stages[Convert.ToInt32(key)].Strategy;

        #endregion


        #region Keys/Values

        /// <inheritdoc/>
        public ICollection<UnityBuildStage> Keys
        {
            get
            {
                return enumerable().ToArray();

                IEnumerable<UnityBuildStage> enumerable()
                {
                    foreach (UnityBuildStage stage in (UnityBuildStage[])Enum.GetValues(typeof(UnityBuildStage)))
                    {
                        if (null != _stages[Convert.ToInt32(stage)].Strategy)
                            yield return stage;
                    }
                }

            }
        }


        /// <inheritdoc/>
        public ICollection<BuilderStrategy> Values
            => _cache ??= ((IEnumerable<BuilderStrategy>)this).ToArray();

        #endregion


        #region IEnumerable

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <inheritdoc/>
        IEnumerator<BuilderStrategy> IEnumerable<BuilderStrategy>.GetEnumerator()
            => GetEnumerator();


        /// <inheritdoc/>
        public Enumerator GetEnumerator()
            => new Enumerator(this);

        #endregion


        #region Enumerator

        public struct Enumerator : IEnumerator<BuilderStrategy>
        {
            private readonly Entry[] _strategies;
            private int _index;

            public Enumerator(StagedStrategyChain strategies)
            {
                _index = -1;
                _strategies = strategies._stages;
                Current = default!;
            }

            public BuilderStrategy Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                while (++_index < _size)
                {
                    var value = _strategies[_index].Strategy;
                    if (null != value)
                    {
                        Current = value;
                        return true;
                    }
                }

                return false;
            }

            public void Reset() => _index = -1;
        }

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
