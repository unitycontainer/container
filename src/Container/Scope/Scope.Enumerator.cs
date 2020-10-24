using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        public Enumerator GetEnumerator() => new Enumerator(this, RegistrationCategory.Internal);


        public struct Enumerator
        {
            #region Fields

            private int _index;
            private Scope _scope;
            private ScopeSet _set;
            private Iterator _iterator;
            private ReadOnlyMemory<Metadata> _selection;
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
                _selection = default;
                _set = new ScopeSet(scope.Ancestry);
                _buffer = new Metadata[scope.Ancestry.Length];
            }

            #endregion

            public readonly ref ContainerRegistration Registration => ref _iterator.Registration;

            public void Dispose() { }

            public void Reset() => throw new NotSupportedException();

            public bool MoveNext()
            {
                do
                {
                    while (_iterator.MoveNext(_selection.Span))
                    {
                        if (!_set.Add(in _iterator)) continue;

                        if (_cutoff != RegistrationCategory.Uninitialized &&
                           (null == _iterator.Internal.Manager || _cutoff > _iterator.Internal.Manager.Category)) continue;

                        return true;
                    }

                    while (_scope.Count >= ++_index)
                    {
                        ref var candidate = ref _scope.Data[_index].Internal;
                    
                        // Skip named registrations
                        if (null != candidate.Contract.Name || _set.Contains(in candidate.Contract)) 
                            continue;

                        _selection = _scope.GetReferences(in candidate.Contract, in _buffer);
                        _iterator = new Iterator(_scope, candidate.Contract.Type);

                        while (_iterator.MoveNext(_selection.Span))
                        {
                            if (!_set.Add(in _iterator)) continue;

                            if (_cutoff != RegistrationCategory.Uninitialized &&
                               (null == _iterator.Internal.Manager || _cutoff > _iterator.Internal.Manager.Category)) continue;

                            return true;
                        }
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
