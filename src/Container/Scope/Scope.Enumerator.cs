using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        public OldEnumerator GetOldEnumerator() => new OldEnumerator(this, RegistrationCategory.Internal);


        // TODO: remove
        public struct OldEnumerator
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

            public OldEnumerator(Scope scope, RegistrationCategory cutoff)
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
                next: while (_iterator.MoveNext(_selection.Span))
                {
                    if (!_set.Add(in _iterator)) continue;

                    if (_cutoff != RegistrationCategory.Uninitialized &&
                       (null == _iterator.Internal.Manager || _cutoff > _iterator.Internal.Manager.Category)) continue;

                    return true;
                }

                do
                {
                    while (_scope.Count >= ++_index)
                    {
                        ref var candidate = ref _scope.Data[_index].Internal;
                    
                        // Skip named registrations
                        if (null != candidate.Contract.Name || _set.Contains(in candidate.Contract)) 
                            continue;

                        _selection = _scope.GetReferences(in candidate.Contract, in _buffer);
                        _iterator  = new Iterator(_scope, candidate.Contract.Type);
                        
                        goto next;
                    }

                    _index = 0;
                }
                while (null != (_scope = _scope.Next!));

                return false;
            }
        }


        public struct Enumerator
        {
            #region Fields

            public Scope Scope;
            public readonly Type Type;

            private int _index;
            private bool _anonymous;
            private Metadata _location;
            private readonly int _count;
            private readonly Scope[] _ancestry;
            private readonly Metadata[] _buffer;

            #endregion


            #region Constructors

            public Enumerator(Scope scope, int position, Metadata[] buffer)
            {
                Scope = scope;
                Type = scope[position].Internal.Contract.Type;

                _index = 0;
                _location = default;
                _anonymous = true;
                _ancestry = scope.Ancestry;
                _buffer = buffer;
                _count = scope.GetReferences(position, buffer);
            }

            #endregion


            #region Data

            public int Level => _location.Location;

            public int Position => _location.Position;

            public Metadata Location => _location;

            internal readonly ref InternalRegistration Internal
                => ref Scope.Data[_location.Position].Internal;

            public readonly ref ContainerRegistration Registration
                => ref Scope.Data[_location.Position].Registration;

            public readonly ref Entry Entry
                => ref Scope.Data[_location.Position];

            #endregion


            #region MoveNext

            public bool MoveNext()
            {
                if (null == _ancestry) return false;

                while (_count > _index)
                {
                    if (0 == _location.Position)
                    {
                        _location = _buffer[_index];
                        Scope = _ancestry[_location.Location];
                        return true;
                    }

                    if (0 < (_location.Position = _anonymous ? Scope.MoveNext(Scope, _location.Position, Type)
                                                             : Entry.Next))
                        return true;

                    _index += 1;
                }

                if (0 == _location.Position && _anonymous)
                {
                    _index = 0;
                    _anonymous = false;
                    _location = _buffer[_index];

                    Scope = _ancestry[_location.Location];
                    _location.Position = Entry.Next;
                }

                return 0 < _location.Position;
            }

            #endregion
        }



    }
}
