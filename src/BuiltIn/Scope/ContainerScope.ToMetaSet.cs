using System;
using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        private static Metadata[] ArrayToMeta(Scope root, Type[] types)
        {
            Span<Metadata> span = stackalloc Metadata[(root.Level + 1) * types.Length];
            var prime = 2;
            var count = 0;
            var index = -1;
            var scope = (ContainerScope)root;
            var stack = scope.GetDefaultPositions(types, in span);
            var data  = AllocateUninitializedArray<Metadata>(Storage.Prime.Numbers[prime++]);
            var meta  = new Metadata[Storage.Prime.Numbers[prime++]];

            Metadata location = default;
            data[0].Location = root.Version;

            // Named registrations
            while (++index < stack.Length)
            {
                location = stack[index];
                scope = (ContainerScope)root.Ancestry[location.Location];

                while (0 < (location.Position = scope[location.Position].Next))
                {
                    ref var entry = ref scope[location.Position].Internal.Contract;
                    var target = (int)(((uint)entry.HashCode) % meta.Length);
                    var position = meta[target].Position;

                    // Check for existing
                    while (position > 0)
                    {
                        ref var record = ref data[position];
                        ref var contract = ref root[in record].Internal.Contract;

                        if (ReferenceEquals(contract.Type, entry.Type) && entry.Name == contract.Name)
                            goto next;

                        position = meta[position].Location;
                    }

                    // Expand if required
                    if (data.Length <= ++count)
                    {
                        var buffer = new Metadata[Storage.Prime.Numbers[prime++]];

                        for (var i = 1; i < count; i++)
                        {
                            ref var record = ref data[i];
                            ref var local = ref buffer[((uint)root[in record].HashCode) % buffer.Length];

                            buffer[i].Location = local.Position;
                            local.Position = i;
                        }

                        data.CopyTo(meta, 0);
                        data = meta;
                        meta = buffer;

                        target = (int)(((uint)entry.HashCode) % buffer.Length);
                    }

                    // Add new registration
                    ref var bucket = ref meta[target];
                    data[count] = location;
                    meta[count].Location = bucket.Position;
                    bucket.Position = count;
                    next:;
                }
            }

            data[0].Position = count;

            return data;
        }

        private static Metadata[] EnumToMeta(Scope root, Type[] types)
        {
            Span<Metadata> span = stackalloc Metadata[(root.Level + 1) * types.Length];
            var prime = 2;
            var count = 0;
            var index = -1;
            var scope = (ContainerScope)root;
            var stack = scope.GetDefaultPositions(types, in span);
            var data = AllocateUninitializedArray<Metadata>(Storage.Prime.Numbers[prime++]);
            var meta = new Metadata[Storage.Prime.Numbers[prime++]];

            Metadata location = default;
            data[0].Location = root.Version;

            // Named registrations
            while (++index < stack.Length)
            {
                location = stack[index];
                scope = (ContainerScope)root.Ancestry[location.Location];

                while (0 < (location.Position = scope[location.Position].Next))
                {
                    ref var entry = ref scope[location.Position].Internal.Contract;
                    var target = (int)(((uint)entry.HashCode) % meta.Length);
                    var position = meta[target].Position;

                    // Check for existing
                    while (position > 0)
                    {
                        ref var record = ref data[position];
                        ref var contract = ref root[in record].Internal.Contract;

                        if (ReferenceEquals(contract.Type, entry.Type) && entry.Name == contract.Name)
                            goto next;

                        position = meta[position].Location;
                    }

                    // Expand if required
                    if (data.Length <= ++count)
                    {
                        var buffer = new Metadata[Storage.Prime.Numbers[prime++]];

                        for (var i = 1; i < count; i++)
                        {
                            ref var record = ref data[i];
                            ref var local = ref buffer[((uint)root[in record].HashCode) % buffer.Length];

                            buffer[i].Location = local.Position;
                            local.Position = i;
                        }

                        data.CopyTo(meta, 0);
                        data = meta;
                        meta = buffer;

                        target = (int)(((uint)entry.HashCode) % buffer.Length);
                    }

                    // Add new registration
                    ref var bucket = ref meta[target];
                    data[count] = location;
                    meta[count].Location = bucket.Position;
                    bucket.Position = count;
                    next:;
                }
            }

            index = -1;

            // Unnamed registrations
            while (++index < stack.Length)
            {
                location = stack[index];
                scope = (ContainerScope)root.Ancestry[location.Location];
                
                ref var entry = ref scope[location.Position].Internal;
                var type = entry.Contract.Type;

                do
                {
                    if (null != entry.Manager)
                    { 
                        // Expand if required
                        if (data.Length <= ++count)
                        {
                            if (null == meta)
                            {
                                Array.Resize(ref data, Storage.Prime.Numbers[prime++]);
                            }
                            else
                            {
                                data.CopyTo(meta, 0);
                                data = meta;
                                meta = null!;
                            }
                        }

                        // Add new registration
                        data[count] = location;
                    }

                    // Move next
                    var scopeMeta = scope.Meta;
                    location.Position = scopeMeta[location.Position].Location;

                    while (location.Position > 0)
                    {
                        entry = ref scope[location.Position].Internal;
                        if (ReferenceEquals(entry.Contract.Type, type) && entry.Contract.Name == null)
                            break;

                        location.Position = scopeMeta[location.Position].Location;
                    }
                }
                while (0 < location.Position);
            }

            data[0].Position = count;

            return data;
        }

        private int LocationOf(Type type, int hash)
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

        private Span<Metadata> GetDefaultPositions(Type[] types, in Span<Metadata> span)
        {
            var count = 0;

            foreach (var type in types)
            {
                var scope = this;
                var hash  = type.GetHashCode();

                do
                {
                    var index = scope.LocationOf(type, hash);
                    if (0 < index)
                    {
                        span[count++] = new Metadata(scope.Level, index);
                        if (0 > scope.Data[index].Next) break;
                    }
                }
                while (null != (scope = (ContainerScope)scope.Next!));
            }

            return span.Slice(0, count);
        }
    }
}
