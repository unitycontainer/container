using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal abstract int MoveNext(ref Iterator enumerator);

        #region Iterator 

        internal struct Iterator 
        {
            #region Fields

            public Scope Scope;
            public readonly Type Type;

            private int _index;
            private bool _anonymous;
            private Metadata _location;
            private readonly Scope[] _ancestry;

            #endregion


            #region Constructors

            public Iterator(Scope scope, Type type, bool includeAnonymous = true)
            {
                Type = type;
                Scope = scope;

                _index = 0;
                _location = default;
                _ancestry = scope.Ancestry;
                _anonymous = includeAnonymous;
            }

            #endregion


            #region Data

            public int Level => _location.Location;

            public int Position => _location.Position;

            public Metadata Location => _location;

            public readonly ref InternalRegistration Internal 
                => ref Scope.Data[_location.Position].Internal;

            public readonly ref ContainerRegistration Registration
                => ref Scope.Data[_location.Position].Registration;

            public readonly ref Entry Entry
                => ref Scope.Data[_location.Position];

            #endregion


            #region MoveNext

            public bool MoveNext(in ReadOnlySpan<Metadata> span)
            {
                if (0 == span.Length || null == _ancestry) return false;

                do
                {
                    if (0 == _location.Position)
                    {
                        _location = span[_index];
                        Scope = _ancestry[_location.Location];
                        return true;
                    }

                    if (0 < (_location.Position = _anonymous ? Scope.MoveNext(ref this)
                                                             : Entry.Next))
                        return true;
                }
                while (span.Length > ++_index);

                if (0 == _location.Position && _anonymous)
                {
                    _index = 0;
                    _anonymous = false;
                    _location = span[_index];

                    Scope = _ancestry[_location.Location];
                    _location.Position = Entry.Next;
                }

                return 0 < _location.Position;
            }

            #endregion
        }
        
        #endregion
    }
}
