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

        // TODO: move where appropriate
        protected class InternalRegistrationManager : RegistrationManager
        {
            public override object? TryGetValue(ICollection<IDisposable> lifetime) 
                => NoValue;
        }
    }
}
