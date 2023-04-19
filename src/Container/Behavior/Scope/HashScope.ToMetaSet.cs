using System;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class HashScope
    {
        public override Metadata[] ToArraySet(Type[] types)
        {
            Span<Metadata> span = stackalloc Metadata[(Level + 1) * types.Length];
            var prime = 2;
            var count = 0;
            var index = -1;
            var scope = this;
            var stack = scope.GetDefaultPositions(types, in span);
            var hash = AllocateUninitializedArray<uint>(Storage.Prime.Numbers[prime]);
            var data = AllocateUninitializedArray<Metadata>(Storage.Prime.Numbers[prime++]);
            var meta = new Metadata[Storage.Prime.Numbers[prime++]];

            Metadata location = default;
            data[0].Location = Version;

            // Named registrations
            while (++index < stack.Length)
            {
                location = stack[index];
                scope = (HashScope)Ancestry[location.Location];

                while (0 < (location.Position = scope[location.Position].Next))
                {
                    ref var entry = ref scope[location.Position].Internal.Contract;
                    var code = (uint)(entry.Name?.GetHashCode() ?? 0);
                    var target = code % meta.Length;
                    var position = meta[target].Position;

                    // Check for existing
                    while (position > 0)
                    {
                        ref var record = ref data[position];
                        ref var contract = ref this[in record].Internal.Contract;

                        if (entry.Name == contract.Name) goto next;

                        position = meta[position].Location;
                    }

                    // Expand if required
                    if (data.Length <= ++count)
                    {
                        var buffer = new Metadata[Storage.Prime.Numbers[prime++]];

                        for (var i = 1; i < count; i++)
                        {
                            ref var local = ref buffer[(hash[i]) % buffer.Length];

                            buffer[i].Location = local.Position;
                            local.Position = i;
                        }

                        data.CopyTo(meta, 0);
                        data = meta;
                        meta = buffer;

                        Array.Resize(ref hash, data.Length);
                        target = code % buffer.Length;
                    }

                    // Add new registration
                    ref var bucket = ref meta[target];
                    hash[count] = code;
                    data[count] = location;
                    meta[count].Location = bucket.Position;
                    bucket.Position = count;
                    next:;
                }
            }

            data[0].Position = count;

            return data;
        }

        public override Metadata[] ToEnumerableSet(Type[] types)
        {
            Span<Metadata> span = stackalloc Metadata[(Level + 1) * types.Length];
            var prime = 2;
            var count = 0;
            var index = -1;
            var scope = this;
            var stack = scope.GetDefaultPositions(types, in span);
            var hash = AllocateUninitializedArray<uint>(Storage.Prime.Numbers[prime]);
            var data = AllocateUninitializedArray<Metadata>(Storage.Prime.Numbers[prime++]);
            var meta = new Metadata[Storage.Prime.Numbers[prime++]];

            Metadata location = default;
            data[0].Location = Version;

            // Named registrations
            while (++index < stack.Length)
            {
                location = stack[index];
                scope = (HashScope)Ancestry[location.Location];

                while (0 < (location.Position = scope[location.Position].Next))
                {
                    ref var entry = ref scope[location.Position].Internal.Contract;
                    var code = (uint)(entry.Name?.GetHashCode() ?? 0);
                    var target = code % meta.Length;
                    var position = meta[target].Position;

                    // Check for existing
                    while (position > 0)
                    {
                        ref var record = ref data[position];
                        ref var contract = ref this[in record].Internal.Contract;

                        if (entry.Name == contract.Name) goto next;

                        position = meta[position].Location;
                    }

                    // Expand if required
                    if (data.Length <= ++count)
                    {
                        var buffer = new Metadata[Storage.Prime.Numbers[prime++]];

                        for (var i = 1; i < count; i++)
                        {
                            ref var local = ref buffer[(hash[i]) % buffer.Length];

                            buffer[i].Location = local.Position;
                            local.Position = i;
                        }

                        data.CopyTo(meta, 0);
                        data = meta;
                        meta = buffer;

                        Array.Resize(ref hash, data.Length);
                        target = code % buffer.Length;
                    }

                    // Add new registration
                    ref var bucket = ref meta[target];
                    hash[count] = code;
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
                scope = (HashScope)Ancestry[location.Location];

                ref var entry = ref scope[location.Position].Internal;
                var type = entry.Contract.Type;

                do
                {
                    if (null != entry.Manager)
                    {
                        // Expand if required
                        if (data.Length <= ++count)
                        {
                            if (meta is null)
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

        private Span<Metadata> GetDefaultPositions(Type[] types, in Span<Metadata> span)
        {
            var count = 0;

            foreach (var type in types)
            {
                var scope = this;
                var hash  = type.GetHashCode();

                do
                {
                    var index = scope.IndexOf(type, hash);
                    if (0 < index)
                    {
                        span[count++] = new Metadata(scope.Level, index);
                        if (0 > scope.Data[index].Next) break;
                    }
                }
                while (null != (scope = (HashScope)scope.Next!));
            }

            return span.Slice(0, count);
        }
    }
}
