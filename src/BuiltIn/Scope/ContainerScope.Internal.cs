using System;

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

        // TODO: Requires optimization

        internal override int MoveNext(int index, Type type)
        {
            var meta = Meta;
            var position = meta[index].Location;

            while (position > 0)
            {
                ref var candidate = ref this[position].Internal.Contract;
                if (ReferenceEquals(candidate.Type, type) && candidate.Name == null)
                    return position;

                position = meta[position].Location;
            }

            return 0;
        }
    }
}
