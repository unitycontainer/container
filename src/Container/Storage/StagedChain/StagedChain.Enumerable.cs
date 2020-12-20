using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Storage
{
    public partial class StagedChain<TStageEnum, TStrategyType>
    {
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
    }
}
