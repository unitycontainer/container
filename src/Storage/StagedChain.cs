﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Storage
{
    /// <summary>
    /// Represents a chain of builder strategies partitioned by stages.
    /// </summary>
    /// <typeparam name="TStageEnum">The stage enumeration to partition the strategies.</typeparam>
    /// <typeparam name="TStrategyType"><see cref="Type"/> of strategy</typeparam>
    public class StagedChain<TStageEnum, TStrategyType> : IDictionary<TStageEnum, TStrategyType>,
                                                          IEnumerable<TStrategyType>
        where TStageEnum    : Enum 
        where TStrategyType : class
    {
        #region Fields

        private const string ERROR_MESSAGE = "An element with the same key already exists";
        private static readonly int _size = Enum.GetNames(typeof(TStageEnum)).Length;
        private readonly TStrategyType?[] _stages = new TStrategyType[_size];

        #endregion


        #region Properties

        public int Count
            => _stages.Aggregate(0, (a, s) => null == s ? a : a + 1);

        public bool IsReadOnly => false;

        #endregion

        
        #region Add

        public void Add(TStageEnum key, TStrategyType value)
        {
            ref var position = ref _stages[Convert.ToInt32(key)];
            
            if (null != position) throw new ArgumentException(ERROR_MESSAGE);

            position = value;
        }

        public void Add(KeyValuePair<TStageEnum, TStrategyType> item)
        {
            ref var position = ref _stages[Convert.ToInt32(item.Key)];
            
            if (null != position) throw new ArgumentException(ERROR_MESSAGE);

            position = item.Value;
        }

        public void Add(params KeyValuePair<TStageEnum, TStrategyType>[] items)
        {
            for (var i = 0; i < items.Length; i++)
            {
                ref var pair = ref items[i];
                ref var position = ref _stages[Convert.ToInt32(pair.Key)];
                
                if (null != position) throw new ArgumentException(ERROR_MESSAGE);

                position = pair.Value;
            }
        }

        #endregion


        #region Get/Set

        public bool TryGetValue(TStageEnum key, out TStrategyType value)
        {
            value = _stages[Convert.ToInt32(key)]!;

            return null != value;
        }

        public TStrategyType this[TStageEnum key] 
        { 
            get => _stages[Convert.ToInt32(key)] ?? throw new KeyNotFoundException(); 
            set => _stages[Convert.ToInt32(key)] = value; 
        }

        #endregion


        #region Remove

        public bool Remove(TStageEnum key)
        {
            ref var position = ref _stages[Convert.ToInt32(key)];

            var value = position;
            position = null;

            return null != value;
        }

        public bool Remove(KeyValuePair<TStageEnum, TStrategyType> item)
        {
            ref var position = ref _stages[Convert.ToInt32(item.Key)];

            if (item.Value == position)
            {
                position = null;
                return true;
            }

            return false;
        }

        public void Clear()
            => Array.Clear(_stages, 0, _size);

        #endregion


        #region Contains

        public bool Contains(KeyValuePair<TStageEnum, TStrategyType> item) 
            => item.Value == _stages[Convert.ToInt32(item.Key)];

        public bool ContainsKey(TStageEnum key) 
            => null != _stages[Convert.ToInt32(key)];

        #endregion


        #region Keys/Values collections

        public ICollection<TStageEnum> Keys
        {
            get
            {
                IEnumerable<TStageEnum> enumerable()
                {
                    foreach (TStageEnum stage in Enum.GetValues(typeof(TStageEnum)))
                    {
                        if (null != _stages[Convert.ToInt32(stage)])
                        {
                            yield return stage;
                        }
                    }
                }

                return enumerable().ToArray();
            }
        }

        public ICollection<TStrategyType> Values
            => (from stage in _stages where null != stage select stage).ToArray();

        #endregion


        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<TStrategyType> IEnumerable<TStrategyType>.GetEnumerator() 
            => GetEnumerator();

        public Enumerator GetEnumerator() 
            => new Enumerator(_stages);

        public TStrategyType[] ToArray() 
            => (from stage in _stages where null != stage select stage).ToArray();

        #endregion


        #region Enumerator

        public struct Enumerator : IEnumerator<TStrategyType>
        {
            private readonly TStrategyType?[] _strategies;
            private int _index;

            public Enumerator(TStrategyType?[] strategies)
            {
                _index = -1;
                _strategies = strategies;
                Current = default!;
            }

            public TStrategyType Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                while (++_index < _size)
                {
                    var value = _strategies[_index];
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

        IEnumerator<KeyValuePair<TStageEnum, TStrategyType>> IEnumerable<KeyValuePair<TStageEnum, TStrategyType>>.GetEnumerator() 
            => throw new NotSupportedException();

        public void CopyTo(KeyValuePair<TStageEnum, TStrategyType>[] array, int arrayIndex) 
            => throw new NotSupportedException();

        #endregion
    }
}
