using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal abstract int MoveNext(int index, Type type);

        internal abstract int IndexOf(Type type, int hash);

        internal ReadOnlySpan<Metadata> GetReferences(Type type, in Span<Metadata> buffer)
        {
            var hash = type.GetHashCode();
            var count = 0;
            var scope = this;

            do
            {
                var position = scope.IndexOf(type, hash);
                if (0 < position)
                {
                    buffer[count++] = new Metadata(scope.Level, position);
                    if (0 > scope.Data[position].Next) goto done;
                }
            }
            while (null != (scope = scope.Next));

            done: return buffer.Slice(0, count);
        }

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
