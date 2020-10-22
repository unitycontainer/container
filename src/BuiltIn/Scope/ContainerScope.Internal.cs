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
                ref var candidate = ref Data[position].Internal;

                if (ReferenceEquals(candidate.Contract.Type, type) && null == candidate.Contract.Name)
                    return position;

                position = meta[position].Next;
            }

            return 0;
        }

        internal override Iterator GetIterator(Type type, bool @default = true)
        {
            var hash = type.GetHashCode();
            var scope = this;

            do
            {
                var meta = scope.Meta;
                var target = ((uint)hash) % meta.Length;
                var position = meta[target].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope.Data[position];

                    if (ReferenceEquals(candidate.Internal.Contract.Type, type) &&
                        candidate.Internal.Contract.Name == null)
                    {
                        return new Iterator(scope, position, type, hash, @default);
                    }

                    position = meta[position].Next;
                }
            }
            while (null != (scope = Unsafe.As<ContainerScope>(scope.Next)));

            return new Iterator(this);
        }

        internal override (Scope, int) NextAnonymous(ref Iterator enumerator)
        {
            var scope = Unsafe.As<ContainerScope>(enumerator.Scope);
            var meta = scope.Meta;
            var position = (0 == enumerator.Positon)
                    ? enumerator.Initial
                    : meta[enumerator.Positon].Next;
            do
            {
                while (position > 0)
                {
                    ref var candidate = ref scope.Data[position].Internal;

                    if (null != candidate.Manager && ReferenceEquals(candidate.Contract.Type, enumerator.Type) &&
                        candidate.Contract.Name == null)
                    {
                        return (scope, position);
                    }

                    position = meta[position].Next;
                }

                if (0 >= scope.Level || null == (scope = Unsafe.As<ContainerScope>(scope.Next)))
                    return (scope!, 0);

                meta = scope.Meta;
                position = meta[((uint)enumerator.Hash) % meta.Length].Position;
            }
            while (true);
        }

        internal override (Scope, int) NextNamed(ref Iterator enumerator)
        {
            var scope = Unsafe.As<ContainerScope>(enumerator.Scope);
            var position = 0 == enumerator.Positon
                ? scope.Data[enumerator.Initial].Next
                : scope.Data[enumerator.Positon].Next;
            do
            {
                if (position > 0) return (scope, position);

                if (0 >= scope.Level || null == (scope = Unsafe.As<ContainerScope>(scope.Next)))
                    return (scope!, 0);

                var meta = scope.Meta;
                position = meta[((uint)enumerator.Hash) % meta.Length].Position;

                while (position > 0)
                {
                    ref var candidate = ref scope.Data[position].Internal;

                    if (ReferenceEquals(candidate.Contract.Type, enumerator.Type) &&
                        candidate.Contract.Name == null)
                    {
                        goto SetNamedPosition;
                    }

                    position = meta[position].Next;
                }

                SetNamedPosition: position = scope.Data[position].Next;
            }
            while (true);
        }
    }
}
