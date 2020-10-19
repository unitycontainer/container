using System;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {

        internal override Enumerator GetEnumerator(Type type)
        {
            var meta = Meta;
            var hash = (uint)type.GetHashCode();
            var target = hash % meta.Length;
            var position = meta[target].Position;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;
                if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, type) &&
                    candidate.Contract.Name == null)
                {
                    return new Enumerator(this, position);
                }

                position = meta[position].Next;
            }

            return new Enumerator(this);
        }

        protected override int MoveNext(int start, int current, bool @default)
        {
            if ( 0 == current) return 0;
            if (-1 == current) return @default ? start : Data[start].Next;
            if (!@default)     return Data[current].Next;

            var meta = Meta;
            var position = meta[current].Next;
            ref var entry = ref Data[current].Internal;

            while (position > 0)
            {
                ref var candidate = ref Data[position].Internal;

                if (null != candidate.Manager && 
                    candidate.Contract.HashCode == entry.Contract.HashCode && 
                    ReferenceEquals(candidate.Contract.Type, entry.Contract.Type))
                {
                    return position;
                }

                position = meta[position].Next;
            }

            return 0;
        }
    }
}
