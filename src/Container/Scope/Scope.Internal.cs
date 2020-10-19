using System;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal abstract Enumerator GetEnumerator(Type type);

        protected virtual int MoveNext(int start, int current, bool @default) => 0;


        #region Enumerator 

        internal struct Enumerator 
        {
            #region Fields

            int _start;
            int _positon;
            Scope _scope;

            #endregion


            #region Constructors

            public Enumerator(Scope scope)
            {
                _scope = scope;
                _positon = 0;
                _start = 0;
            }

            public Enumerator(Scope scope, int start)
            {
                _scope = scope;
                _positon = -1;
                _start = start;
            }

            #endregion


            #region Properties

            public bool HasNamed => 0 != _start && 0 != _scope.Data[_start].Next;

            #endregion


            #region Enumerator

            public void Reset() 
                => _positon = (0 != _start) ? -1 : 0;

            public readonly ref InternalRegistration Current 
                => ref _scope.Data[_positon].Internal;

            public bool MoveNext(bool @default = false) 
                => 0 != (_positon = _scope.MoveNext(_start, _positon, @default));

            #endregion
        }

        #endregion
    }
}
