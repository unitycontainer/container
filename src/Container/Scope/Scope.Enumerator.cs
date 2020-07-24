using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope : IEnumerable<ContainerRegistration>
    {
        protected abstract bool MoveNext(ref int index, ref ContainerRegistration registration);

        public virtual IEnumerator<ContainerRegistration> GetEnumerator() 
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();




        #region Enumerator

        public struct Enumerator : IEnumerator<ContainerRegistration>
        {
            #region Constants
            
            private const string EnumFailedVersion = "Collection was modified; enumeration operation may not execute";
            
            #endregion


            #region Fields

            private readonly Scope _scope;
            private readonly int _version;

            private ContainerRegistration _current;
            private int _index;

            #endregion


            #region Constructors

            public Enumerator(Scope scope)
            {
                _index = 0;
                _scope = scope;
                _version = scope._version;
                _current = default;
            }

            #endregion



            public ContainerRegistration Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (_version != _scope._version) throw new InvalidOperationException(EnumFailedVersion);

                return _scope.MoveNext(ref _index, ref _current);
            }

            public void Reset()
            {
                if (_version != _scope._version) throw new InvalidOperationException(EnumFailedVersion);

                _index = 0;
                _current = default;
            }
        }

        #endregion
    }
}
