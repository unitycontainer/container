using System;

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
            private Location _location;
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

            public int Level => _location.Level;

            public int Position => _location.Position;

            public Location Location => _location;

            public readonly ref InternalRegistration Internal 
                => ref Scope.Data[_location.Position].Internal;

            public readonly ref ContainerRegistration Registration
                => ref Scope.Data[_location.Position].Registration;

            public readonly ref Entry Entry
                => ref Scope.Data[_location.Position];

            #endregion


            #region MoveNext

            public bool MoveNext(in ReadOnlySpan<Location> span)
            {
                if (0 == span.Length || null == _ancestry) return false;

                do
                {
                    if (0 == _location.Position)
                    {
                        _location = span[_index];
                        Scope = _ancestry[_location.Level];
                        return true;
                    }

                    if (0 < (_location.Position = _anonymous ? Scope.MoveNext(ref this)
                                                           : Scope.Data[_location.Position].Next))
                        return true;
                }
                while (span.Length > ++_index);

                if (0 == _location.Position && _anonymous)
                {
                    _index = 0;
                    _anonymous = false;
                    _location = span[_index];

                    Scope = _ancestry[_location.Level];
                    _location.Position = Scope.Data[_location.Position].Next;
                }

                return 0 < _location.Position;
            }

            #endregion
        }
        
        #endregion
    }
}
