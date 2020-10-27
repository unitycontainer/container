using System;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal Enumerator GetEnumerator<TTarget>(bool anonumous, in Span<Metadata> span)
        {
            var hash = typeof(TTarget).GetHashCode();
            var count = 0;
            var scope = this;

            do
            {
                var position = scope.IndexOf(typeof(TTarget), hash);
                if (0 < position)
                {
                    span[count++] = new Metadata(scope.Level, position);
                    if (0 > scope.Data[position].Next) goto done;
                }
            }
            while (null != (scope = scope.Next));

            done: return new Enumerator(this, typeof(TTarget), anonumous, in span, count);
        }

        [DebuggerDisplay("Current = {_position}, {_location}")]
        internal ref struct Enumerator 
        {
            #region Fields

            private bool _anonymous;
            private int _index;
            private int _position;
            private Type _type;
            private Scope _scope;
            private Metadata _location;
            private readonly Scope _root;
            private readonly ReadOnlySpan<Metadata> _stack;

            #endregion


            #region Constructors


            public Enumerator(Scope scope, Type type, bool includeAnonymous, in Span<Metadata> span, int count)
            {
                _type = type;
                _scope = _root = scope;
                _anonymous = includeAnonymous;
                _stack = span.Slice(0, count);

                _index    = -1;
                _position = default;
                _location = default;
            }

            #endregion


            #region Current

            public readonly ref Contract Contract
                => ref _scope[_position].Internal.Contract;

            internal RegistrationManager? Manager
                => _scope[_position].Internal.Manager;

            public readonly ref ContainerRegistration Registration
                => ref _scope[_position].Registration;

            public Metadata Location
                => new Metadata(_scope.Level, _position);

            #endregion


            #region MoveNext

            public bool MoveNext()
            {
                // Next 
                _position = _anonymous ? _scope.MoveNext(_position, _type)
                                       : _scope[_position].Next;

                if (0 < _position) return true;

                // Level Up
                if (++_index < _stack.Length)
                {
                    _location = _stack[_index];
                    _scope = _root.Ancestry[_location.Location];
                    _position = _anonymous ? _location.Position : _scope[_location.Position].Next;

                    if (0 < _position) return true;
                }

                // Switch to named
                if (_anonymous && (0 < _stack.Length))
                {
                    _index = 0;
                    _location = _stack[_index];
                    _scope = _root.Ancestry[_location.Location];
                    _position = _scope[_location.Position].Next;
                    _anonymous = false;

                    if (0 < _position) return true;
                }

                return false;
            }

            #endregion
        }
    }
}
