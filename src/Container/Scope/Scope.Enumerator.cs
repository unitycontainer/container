using System.Collections;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope : IEnumerable<ContainerRegistration>
    {
        public virtual IEnumerator<ContainerRegistration> GetEnumerator() 
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();


        #region Enumerator

        public struct Enumerator : IEnumerator<ContainerRegistration>
        {
            #region Fields

            private readonly Scope _scope;
            private int _index;

            #endregion


            #region Constructors

            public Enumerator(Scope scope)
            {
                Current = default;
                _scope = scope;
                _index = 0;
            }

            #endregion

            public ContainerRegistration Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                var span = _scope.Memory.Span;

                if (++_index > _scope.RunningIndex) return false;

                Current = span[_index];
                
                return true;
            }

            public void Reset()
            {
                _index = 0;
                Current = default;
            }
        }

        #endregion
    }
}
