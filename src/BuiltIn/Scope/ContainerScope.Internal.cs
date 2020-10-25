using System;
using System.Runtime.CompilerServices;
using Unity.Container;

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

                position = meta[position].Location;
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

                position = meta[position].Location;
            }

            return 0;
        }

        internal override int MoveNext(Scope scope, int index, Type type)
        {
            var meta = Unsafe.As<ContainerScope>(scope).Meta;
            var position = meta[index].Location;

            while (position > 0)
            {
                ref var candidate = ref scope[position].Internal.Contract;
                if (ReferenceEquals(candidate.Type, type) && candidate.Name == null)
                    return position;

                position = meta[position].Location;
            }

            return 0;
        }

    }
}
