using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        #region Enumerator

        internal TypeEnumerator GetTypes(int start) => new TypeEnumerator(this, start, true);

        internal struct TypeEnumerator
        {
            #region Fields

            int _index;
            bool _default;
            Scope _scope;

            #endregion


            #region Constructors

            public TypeEnumerator(Scope scope, int start, bool @default = false)
            {
                Registrations = default;

                _index = start;
                _scope = scope;
                _default = @default;
            }

            #endregion

            public Iterator Registrations;

            public Type Type => _scope.Data[_index].Internal.Contract.Type;

            public readonly ref Contract Contract => ref _scope.Data[_index].Internal.Contract;

            public void Dispose() { }

            public void Reset() => _index = 0;

            public bool MoveNext()
            {
                while (_scope.Count >= ++_index)
                {
                    ref var candidate = ref _scope.Data[_index].Internal;

                    if (null != candidate.Contract.Type && null == candidate.Contract.Name)
                    {
                        Registrations = new Iterator(_scope, _index, candidate.Contract.Type, candidate.Contract.HashCode, _default);
                        return true;
                    }
                }

                return false;
            }

        }


        internal RegistrationEnumerator GetRegistrations(int count, int start = 0)
            => new RegistrationEnumerator(this, count, start);

        internal struct RegistrationEnumerator
        {
            #region Constants

            private const string INVALID_ENUMERATOR = "Collection was modified; enumeration operation may not execute";

            #endregion


            #region Fields

            int _index;
            int _limit;
            int _version;
            Scope _scope;

            #endregion


            #region Constructors

            public RegistrationEnumerator(Scope scope, int count, int start)
            {
                _index = start;
                _limit = start + count;
                _scope = scope;
                _version = scope.Version;
            }

            #endregion

            public readonly ref ContainerRegistration Registration
                => ref _scope.Data[_index].Registration;


            public void Dispose() { }

            public void Reset() => _index = 0;

            public bool MoveNext()
            {
                if (_version != _scope.Version)
                    throw new InvalidOperationException(INVALID_ENUMERATOR);

                if (_limit >= ++_index && _scope.Data.Length > _index)
                    return true;

                return false;
            }

        }

        #endregion




        internal abstract int IndexOf(Type type, int hash);



        internal abstract Iterator GetIterator(Type type, bool @default = true);

        internal abstract (Scope, int) NextAnonymous(ref Iterator enumerator);

        internal abstract (Scope, int) NextNamed(ref Iterator enumerator);


        #region Iterator 

        internal struct Iterator 
        {
            #region Fields

            private bool _default;
            private readonly Scope _scope;

            public readonly int Initial;

            public int Positon;
            public Scope Scope;

            public readonly int  Hash;
            public readonly Type Type;

            #endregion


            #region Constructors

            public Iterator(Scope scope, int position, Type type, int hash, bool @default)
            {
                Hash = hash;
                Type = type;
                Positon  = 0;
                Scope = _scope = scope;

                _default = @default;
                Initial  = position;
            }

            public Iterator(Scope scope)
            {
                Hash = 0;
                Type = typeof(Iterator);
                Positon = 0;
                Scope = _scope = scope;

                Initial = 0;
                _default = false;
            }

            #endregion


            #region Enumerator

            public void Reset()
            {
                _default = true;
                Positon = 0;
                Scope = _scope;
            }

            public readonly ref InternalRegistration Internal 
                => ref Scope.Data[Positon].Internal;

            public readonly ref ContainerRegistration Registration
                => ref Scope.Data[Positon].Registration;

            public readonly ref Entry Entry
                => ref Scope.Data[Positon];

            public bool MoveNext()
            {
                if (0 == Initial) return false;

                (Scope, Positon) = _default ? Scope.NextAnonymous(ref this)
                                            : Scope.NextNamed(ref this);

                if (0 == Positon && _default)
                {
                    Scope = _scope;
                    Positon = 0;
                    _default = false;
                    
                    (Scope, Positon) = Scope.NextNamed(ref this);
                }

                return 0 != Positon;
            }

            public bool NextAnonymous()
            {
                if (0 == Initial) return false;

                (Scope, Positon) = Scope.NextAnonymous(ref this);

                return 0 != Positon || null == Scope;
            }

            public bool NextNamed()
            {
                if (0 == Initial) return false;

                (Scope, Positon) = Scope.NextNamed(ref this);

                return 0 != Positon || null == Scope;
            }

            public bool NextDefault()
            {
                if (0 == Initial) return false;

                (Scope, Positon) = Scope.NextNamed(ref this);

                return 0 != Positon || null == Scope;
            }

            #endregion
        }

        #endregion


        // TODO: move where appropriate
        protected class InternalRegistrationManager : RegistrationManager
        {
            public override object? TryGetValue(ICollection<IDisposable> lifetime) 
                => NoValue;
        }
    }
}
