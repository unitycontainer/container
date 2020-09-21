using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Container
{
    public partial struct PipelineBuilder<T>
    {
        private struct Enumerator : IEnumerator<PipelineVisitor<T>>
        {
            #region Fields

            private int _index;
            private readonly PipelineVisitor<T>[] _visitors;

            #endregion


            #region Constructors

            public Enumerator(PipelineVisitor<T>[] visitors)
            {
                _index = -1;
                _visitors = visitors;
                
                // Undefined
                Current = default!;
            }
            
            #endregion


            #region IEnumerator

            object IEnumerator.Current => Current;

            public PipelineVisitor<T> Current { get; private set; }

            public bool MoveNext()
            {
                while (++_index < _visitors.Length)
                {
                    Current = _visitors[_index];
                    return true;
                }

                return false;
            }

            public void Reset() => throw new NotSupportedException();

            public void Dispose() { }

            #endregion
        }
    }
}
