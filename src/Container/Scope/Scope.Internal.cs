using System;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal abstract Enumerator GetEnumerator(Type type);

        internal abstract (Scope, int) NextAnonymous(ref Enumerator enumerator);

        internal abstract (Scope, int) NextNamed(ref Enumerator enumerator);

        internal abstract (Scope, int) NextDefault(ref Enumerator enumerator);


        #region Enumerator 

        internal struct Enumerator 
        {
            #region Fields

            private bool           _default;
            private readonly Scope _scope;

            public readonly Type Type;
            public readonly int  Hash;

            public Scope Scope;
            public int Positon;
            public readonly int Initial;

            #endregion


            #region Constructors

            public Enumerator(Scope scope, int position, Type type, int hash)
            {
                Hash = hash;
                Type = type;
                Positon  = 0;
                Scope = _scope = scope;

                _default = true;
                Initial = position;
            }

            public Enumerator(Scope scope)
            {
                Hash = 0;
                Type = typeof(Enumerator);
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
                (Scope, Positon) = Scope.NextAnonymous(ref this);

                return 0 != Positon || null == Scope;
            }

            public bool NextNamed()
            {
                (Scope, Positon) = Scope.NextNamed(ref this);

                return 0 != Positon || null == Scope;
            }

            public bool NextDefault()
            {
                (Scope, Positon) = Scope.NextNamed(ref this);

                return 0 != Positon || null == Scope;
            }

            #endregion
        }

        #endregion
    }
}
