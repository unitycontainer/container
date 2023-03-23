using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Unity.Storage
{
    public partial class StagedStrategyChain<TStrategyType, TStageEnum>
    {
        #region Add

        /// <inheritdoc/>
        public void Add(TStageEnum stage, TStrategyType strategy)
        {
            ref var entry = ref _stages[Convert.ToInt32(stage)];

            if (entry.Strategy is not null) throw new ArgumentException(ERROR_MESSAGE);

            entry.Strategy = strategy;

            Count += 1;
            Version += 1;
            Invalidated?.Invoke(this, _marker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        public void Add(KeyValuePair<TStageEnum, TStrategyType> item)
            => Add(item.Key, item.Value);

        #endregion


        #region Contains

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TStageEnum, TStrategyType> item)
            => item.Value == _stages[Convert.ToInt32(item.Key)].Strategy;


        /// <inheritdoc/>
        public bool ContainsKey(TStageEnum key)
            => null != _stages[Convert.ToInt32(key)].Strategy;

        #endregion


        #region Get/Set

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        
        /// <inheritdoc/>
        public bool TryGetValue(TStageEnum key, [MaybeNullWhen(false)] out TStrategyType value)
        {
            value = _stages[Convert.ToInt32(key)].Strategy!;

            return null != value;
        }

#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).


        /// <inheritdoc/>
        public TStrategyType this[TStageEnum key]
        {
            get => _stages[Convert.ToInt32(key)].Strategy ?? throw new KeyNotFoundException();
            set
            {
                if (value is null) throw new ArgumentNullException("The TStrategyType must not be null");
               
                ref var position = ref _stages[Convert.ToInt32(key)];
                if (position.Strategy is null) Count += 1;
                
                position.Strategy = value;
                Version += 1;
                Invalidated?.Invoke(this, _marker);
            }
        }

        #endregion


        #region Remove

        /// <inheritdoc/>
        public bool Remove(TStageEnum key)
        {
            ref var position = ref _stages[Convert.ToInt32(key)];

            if (position.Strategy is not null)
            {
                Count -= 1;
                position.Strategy = default;
                Version += 1;
                Invalidated?.Invoke(this, _marker);
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TStageEnum, TStrategyType> item)
        {
            ref var position = ref _stages[Convert.ToInt32(item.Key)];

            if (item.Value == position.Strategy)
            {
                Count -= 1;
                position.Strategy = default;
                Version += 1;
                Invalidated?.Invoke(this, _marker);
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public void Clear()
        {
            for (var i = 0; i < _size; i++) _stages[i].Strategy = default;

            Count = 0;
            Version += 1;
        }

        #endregion


        #region Keys/Values

        public ICollection<TStageEnum> Keys
        {
            get
            { 
                var array = new TStageEnum[Count];
                int i = 0, index = -1;

                while (++index < _size && i < Count)
                {
                    ref var entry = ref _stages[index];
                    if (entry.Strategy is not null)
                        array[i++] = entry.Stage;
                }

                return array;
            }
        }

        public ICollection<TStrategyType> Values
        {
            get
            {
                var array = new TStrategyType[Count];
                int i = 0, index = -1;

                while (++index < _size && i < Count)
                {
                    ref var entry = ref _stages[index];
                    if (entry.Strategy is not null)
                        array[i++] = entry.Strategy;
                }

                return array;
            }
        }

        #endregion



        public void CopyTo(KeyValuePair<TStageEnum, TStrategyType>[] array, int arrayIndex)
        {
            int i = arrayIndex, index = -1;

            while (++index < _size && i < array.Length)
            {
                ref var entry = ref _stages[index];
                if (entry.Strategy is not null)
                    array[i++] = new KeyValuePair<TStageEnum, TStrategyType>(entry.Stage, entry.Strategy);
            }
        }
    }
}
