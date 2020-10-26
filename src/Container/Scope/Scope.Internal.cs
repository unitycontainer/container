using System;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        internal abstract int IndexOf(in Contract contract);

        internal abstract int IndexOf(Type type, int hash);


        internal int GetNextType(int current)
        {
            while (++current <= Index)
            {
                if (null == Data[current].Internal.Contract.Name)
                    return current;
            }

            return 0;
        }

        internal (Scope, Metadata) NextType(in Metadata address)
        {
            var index = address.Position;
            var scope = 0 == index ? this : Ancestry[address.Location];

            do 
            { 
                while (++index <= scope.Index)
                {
                    if (null == scope.Data[index].Internal.Contract.Name)
                        return (scope, new Metadata(scope.Level, index));
                }
            } 
            while (null != (scope = scope.Next));

            return (this, default);
        }

        internal int GetReferences(int index, Metadata[] buffer)
        {
            ref var contract = ref Data[index].Internal.Contract;
            var count = 0;
            var scope = this;

            do
            {
                var position = scope.IndexOf(contract.Type, contract.HashCode);
                if (0 < position)
                {
                    buffer[count++] = new Metadata(scope.Level, position);
                    if (0 > scope.Data[position].Next) return count;
                }
            }
            while (null != (scope = scope.Next));

            return count;
        }

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

        internal ReadOnlyMemory<Metadata> GetReferences(in Contract contract, in Memory<Metadata> buffer)
        {
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
