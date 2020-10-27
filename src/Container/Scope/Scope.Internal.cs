using System;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal abstract int MoveNext(int index, Type type);

        internal abstract int IndexOf(Type type, int hash);

        internal int Total
        {
            get
            {
                var scope = this;
                var length = 0;

                do { length += scope.Count;  } 
                while (null != (scope = scope.Next));

                return length;
            }
        }

    }
}
