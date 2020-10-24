using System;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {

        internal abstract int IndexOf(in Contract contract);

        internal abstract int IndexOf(Type type, int hash);

        internal ReadOnlySpan<Metadata> GetReferences(Type type, in Span<Metadata> buffer)
        {
            Debug.Assert(Level < buffer.Length);

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

        internal ReadOnlyMemory<Metadata> GetReferences(in Contract contract, in Memory<Metadata> buffer)
        {
            Debug.Assert(Level < buffer.Length);

            var count = 0;
            var scope = this;

            do
            {
                var position = scope.IndexOf(in contract);
                if (0 < position)
                {
                    buffer.Span[count++] = new Metadata(scope.Level, position);
                    if (0 > scope.Data[position].Next) goto done;
                }
            }
            while (null != (scope = scope.Next));

            done: return buffer.Slice(0, count);
        }
    }
}
