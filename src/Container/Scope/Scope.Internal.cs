using System;
using System.Collections.Generic;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal abstract int IndexOf(Type type, int hash);



        internal abstract Iterator GetIterator(Type type, bool @default = true);

        internal abstract (Scope, int) NextAnonymous(ref Iterator enumerator);

        internal abstract (Scope, int) NextNamed(ref Iterator enumerator);

        internal abstract (Scope, int) NextDefault(ref Iterator enumerator);


        #region Enumerator 

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

            public readonly ref InternalRegistration Current 
                => ref Scope.Data[Positon].Internal;

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


        protected class ScopeRegistration : RegistrationManager
        {
            public ScopeRegistration() => Category = RegistrationCategory.Internal;

            public override object? TryGetValue(ICollection<IDisposable> lifetime) 
                => NoValue;
        }
    }
}
