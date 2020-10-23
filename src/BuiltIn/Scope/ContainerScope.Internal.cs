using System;
using System.Runtime.CompilerServices;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        internal override int IndexOf(Type type, int hash)
        {
            var meta = Meta;
            var target = ((uint)hash) % meta.Length;
            var position = meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal.Contract;

                if (ReferenceEquals(candidate.Type, type) && null == candidate.Name)
                    return position;

                position = meta[position].Next;
            }

            return 0;
        }

        internal override int IndexOf(in Contract contract)
        {
            var meta = Meta;
            var target = ((uint)contract.HashCode) % meta.Length;
            var position = meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal.Contract;

                if (ReferenceEquals(candidate.Type, contract.Type) && contract.Name == candidate.Name)
                    return position;

                position = meta[position].Next;
            }

            return 0;
        }

        internal override int MoveNext(ref Iterator iterator)
        {
            var scope = Unsafe.As<ContainerScope>(iterator.Scope);
            var meta  = scope.Meta;
            var position = meta[iterator.Position].Next;

            while (position > 0)
            {
                ref var candidate = ref scope[position].Internal.Contract;
                if (ReferenceEquals(candidate.Type, iterator.Type) && candidate.Name == null)
                    return position;

                position = meta[position].Next;
            }

            return 0;
        }
    }
}
