using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        public NewEnumerator GetEnumerator() => new NewEnumerator(this);


        public struct NewEnumerator
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

            public NewEnumerator(Scope scope)
            {
                // TODO: Invalid
                Scope = scope;
                Type = typeof(NewEnumerator);

                _index = 0;
                _location = default;
                _anonymous = true;
                _ancestry = scope.Ancestry;
                _buffer = new Metadata[0];
                _count = 0;
            }

            public NewEnumerator(Scope scope, int position, Metadata[] buffer)
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
