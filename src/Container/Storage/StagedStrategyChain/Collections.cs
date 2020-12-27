using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Extension;

namespace Unity.Storage
{
    public partial class StagedStrategyChain
    {
        #region Keys/Values collections

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
                        if (null != _stages[Convert.ToInt32(stage)])
                            yield return stage;
                    }
                }

            }
        }


        /// <inheritdoc/>
        public ICollection<BuilderStrategy> Values
            => (from stage in _stages where null != stage select stage).ToArray();

        #endregion


        #region IEnumerable

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <inheritdoc/>
        IEnumerator<BuilderStrategy> IEnumerable<BuilderStrategy>.GetEnumerator() 
            => GetEnumerator();


        /// <inheritdoc/>
        public Enumerator GetEnumerator() 
            => new Enumerator(_stages);


        public BuilderStrategy[] ToArray() 
            => (from stage in _stages where null != stage select stage).ToArray();

        #endregion


        #region Enumerator

        public struct Enumerator : IEnumerator<BuilderStrategy>
        {
            private readonly BuilderStrategy?[] _strategies;
            private int _index;

            public Enumerator(BuilderStrategy?[] strategies)
            {
                _index = -1;
                _strategies = strategies;
                Current = default!;
            }

            public BuilderStrategy Current { get; private set; }

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
