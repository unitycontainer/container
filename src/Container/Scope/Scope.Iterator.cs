using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal virtual Iterator GetIterator(Type type, bool @default = true)
        {
            var hash = type.GetHashCode();
            var scope = this;
            var count = 0;
            var buffer = new Metadata[scope.Level + 1];

            do
            {
                var position = Get(type, hash);
                if (0 < position)
                {
                    buffer[count++] = new Metadata(scope.Level, position);
                }
            }
            while (null != (scope = scope.Next));

            return new Iterator(this, type, buffer, count);
        }

        internal virtual Iterator GetIterator(in Contract contract, in Memory<Metadata> buffer, RegistrationCategory cutoff)
        {
            Debug.Assert(Level < buffer.Length);

            var count = 0;
            var scope = this;

            do
            {
                var position = scope.Get(contract.Type, contract.HashCode);
                if (0 < position)
                {
                    buffer.Span[count++] = new Metadata(scope.Level, position);
                    if (0 > scope.Data[position].Next) goto done;
                }
            } 
            while (null != (scope = scope.Next));

            done: return new Iterator(this, contract.Type, buffer.Slice(0, count), cutoff);
        }

        internal abstract int MoveNext(ref Iterator enumerator);

        internal abstract int Get(Type type, int hash);

        #region Iterator 

        internal struct Iterator 
        {
            #region Fields


            private int _level;
            private bool _default;
            
            public Scope Scope;
            public int Position;
            public readonly Type Type;


            private readonly Scope[] _ancestry;
            private readonly Metadata[]? _owner;
            private readonly Memory<Metadata> _buffer;
            private readonly RegistrationCategory _cutoff;

            #endregion


            #region Constructors

            public Iterator(Scope scope, Type type, Metadata[] buffer, int length, RegistrationCategory cutoff = RegistrationCategory.Internal)
            {
                Type = type;
                _level = 0;
                Position = 0;

                _owner = buffer;
                _default = true;
                _ancestry = scope._ancestry;
                _cutoff = cutoff;

                _buffer = new Memory<Metadata>(buffer, 0, length);
                Scope = 0 < length ? _ancestry[_owner[0].Next] : scope;
            }

            public Iterator(Scope scope, Type type, Memory<Metadata> buffer, RegistrationCategory cutoff)
            {
                Type = type;
                _level = 0;
                Position = 0;

                _owner = null;
                _default = true;
                _ancestry = scope._ancestry;
                _cutoff = cutoff;

                _buffer = buffer;
                Scope = 0 < _buffer.Length ? scope._ancestry[_buffer.Span[0].Next] : scope;
            }


            #endregion


            #region Data

            public readonly ref InternalRegistration Internal 
                => ref Scope.Data[Position].Internal;

            public readonly ref ContainerRegistration Registration
                => ref Scope.Data[Position].Registration;

            public readonly ref Entry Entry
                => ref Scope.Data[Position];

            #endregion


            #region MoveNext

            public bool MoveNext(ref ScopeSet set)
            {
                while(IterateNext())
                {
                    if (!set.Add(Scope.Level, Position)) continue;
                    
                    if (_cutoff != RegistrationCategory.Uninitialized &&
                       (null == Internal.Manager || _cutoff > Internal.Manager.Category)) continue;

                    return true;
                }

                return false;
            }

            public bool MoveNext()
            {
                while (IterateNext())
                {
                    if (_cutoff != RegistrationCategory.Uninitialized &&
                       (null == Internal.Manager || _cutoff > Internal.Manager.Category)) continue;

                    return true;
                }

                return false;
            }

            #endregion


            #region Implementation

            private bool IterateNext()
            {
                if (0 == _buffer.Length) return false;

                do 
                {
                    if (0 == Position)
                    {
                        Position = _buffer.Span[_level].Position;
                        Scope = _ancestry[_buffer.Span[_level].Next];
                        return true;
                    }

                    if (0 < (Position = _default ? Scope.MoveNext(ref this)
                                                 : NextNamed()))
                        return true;
                }
                while (_buffer.Length > ++_level);

                if (0 == Position && _default)
                {
                    _default = false;

                    _level = 0;
                    Scope = _ancestry[_buffer.Span[0].Next];
                    Position = 0;
                    Position = NextNamed();
                }

                return 0 != Position;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private int NextNamed()
            {
                var position = 0 == Position ? Scope.Data[_buffer.Span[_level].Position].Next
                                             : Scope.Data[Position].Next;
                
                return position > 0 ? position : 0;
            }

            #endregion
        }
        
        #endregion
    }
}
