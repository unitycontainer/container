using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        public Enumerator GetEnumerator(RegistrationCategory cutoff = RegistrationCategory.Internal) => new Enumerator(this, cutoff);


        public struct Enumerator
        {
            #region Fields

            private int _index;
            private Scope _scope;
            private ScopeSet _set;
            private Iterator _iterator;
            private readonly Memory<Metadata> _buffer;
            private readonly RegistrationCategory _cutoff;

            #endregion


            #region Constructors

            public Enumerator(Scope scope, RegistrationCategory cutoff)
            {
                _index = 0;
                _scope = scope;
                _cutoff = cutoff;
                _iterator = default;
                _set = new ScopeSet(scope._ancestry);
                _buffer = new Metadata[scope._ancestry.Length];
            }

            #endregion

            public readonly ref ContainerRegistration Registration => ref _iterator.Registration;

            public void Dispose() { }

            public void Reset() => throw new NotSupportedException();

            public bool MoveNext()
            {
                do
                {
                    if (_iterator.MoveNext(ref _set)) return true;

                    while (_scope.Count >= ++_index)
                    {
                        ref var candidate = ref _scope.Data[_index].Internal;
                    
                        // Skip named registrations
                        if (null != candidate.Contract.Name || _set.Contains(in candidate.Contract)) 
                            continue;

                        _iterator = _scope.GetIterator(in candidate.Contract, in _buffer, _cutoff);

                        if (_iterator.MoveNext(ref _set)) return true;
                    }

                    _index    = 0;
                    _iterator = default;
                }
                while (null != (_scope = _scope.Next!));

                return false;
            }
        }
    }
}
