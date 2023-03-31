using System.Collections;
using System.Diagnostics;

namespace Unity.Storage
{
    [DebuggerDisplay("StagedStrategyChain: Count = {Count}, Version = {Version}")]
    public partial class StagedStrategyChain<TStrategyType, TStageEnum> 
    {
        #region IEnumerable

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<TStageEnum, TStrategyType>> GetEnumerator()
            => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<KeyValuePair<TStageEnum, TStrategyType>> IEnumerable<KeyValuePair<TStageEnum, TStrategyType>>.GetEnumerator()
            => new Enumerator(this);


        #endregion


        #region Storage

        public struct Enumerator : IEnumerator<KeyValuePair<TStageEnum, TStrategyType>>
        {
            private readonly Entry[] _stages;
            private int _index;

            public Enumerator(StagedStrategyChain<TStrategyType, TStageEnum> parent)
            {
                _index = -1;
                _stages = parent._stages;

                Current = default!;
            }

            public KeyValuePair<TStageEnum, TStrategyType> Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                while (++_index < _stages.Length)
                {
                    ref var entry = ref _stages[_index];
                    if (entry.Strategy is not null)
                    {
                        Current = new KeyValuePair<TStageEnum, TStrategyType>(entry.Stage, entry.Strategy);
                        return true;
                    }
                }

                return false;
            }

            public void Reset() => _index = -1;
        }

        private struct Entry
        {
            public TStageEnum     Stage;
            public TStrategyType? Strategy;
        }

        #endregion
    }
}
