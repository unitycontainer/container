using System;
using System.Linq;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        private Metadata[] QueryToTape(bool anonymous, Type[] types)
        {
            var count = 0;
            Metadata[] data = new Metadata[Storage.Prime.Numbers[5]];
            Span<Metadata> meta = stackalloc Metadata[Storage.Prime.Numbers[6]];
            Span<Metadata> span = stackalloc Metadata[Level + 2];

            data.Version(Version);

            // TODO: where
            foreach (var type in types.Where(t => null != t))
            {
                var hash = type.GetHashCode();
                var scope = this;
                var index = -1;
                var current = 0;
                Metadata location = default;

                // Get positions within the scope stack
                do
                {
                    var position = scope.IndexOf(type, hash);
                    if (0 < position)
                    {
                        span[current++] = new Metadata(scope.Level, position);
                        if (0 > scope.Data[position].Next) break;
                    }
                }
                while (null != (scope = (ContainerScope)scope.Next!));
                
                var stack = span.Slice(0, current);
                scope = this;
                current = 0;

                while (0 < stack.Length)
                { 
                    // Next 
                    next: current = anonymous ? scope.MoveNext(current, type)
                                              : scope[current].Next;
                    // Add
                    if (0 < current) goto record;

                    // Level Up
                    if (++index < stack.Length)
                    {
                        location = stack[index];
                        scope = (ContainerScope)Ancestry[location.Location];
                        current = anonymous ? location.Position : scope[location.Position].Next;

                        if (0 < current) goto record;
                    }

                    // Switch to named
                    if (anonymous)
                    {
                        data.Count(count);
                        index = 0;
                        location = stack[index];
                        scope = (ContainerScope)Ancestry[location.Location];
                        current = scope[location.Position].Next;
                        anonymous = false;

                        if (0 < current) goto record;
                    }

                    break;

                    record: 

                    ref var contract = ref scope[current].Internal.Contract;
                    var target = (int)(((uint)contract.HashCode) % meta.Length);

                    if (null != contract.Name)
                    {
                        var position = meta[target].Position;

                        while (position > 0)
                        {
                            ref var record = ref data[position];
                            ref var entry = ref this[in record].Internal.Contract;

                            if (contract.HashCode == entry.HashCode && ReferenceEquals(entry.Type, contract.Type))
                                goto next;

                            position = meta[position].Location;
                        }
                    }

                    if (data.Length <= ++count)
                    {
                        var prime = Storage.Prime.NextUp(meta.Length);

                        Array.Resize(ref data,         Storage.Prime.Numbers[++prime]);
                        var replacement = new Metadata[Storage.Prime.Numbers[++prime]];

                        for (var position = 1; position < count; position++)
                        {
                            ref var record = ref data[position];
                            ref var local  = ref replacement[((uint)this[in record].HashCode) % replacement.Length];

                            replacement[position].Location = local.Position;
                            local.Position = position;
                        }

                        meta = replacement;
                        target = (int)(((uint)contract.HashCode) % replacement.Length);
                    }

                    // Add new registration
                    ref var bucket = ref meta[target];
                    data[count] = new Metadata(scope.Level, current);
                    meta[count].Location = bucket.Position;
                    bucket.Position = count;
                }
            }

            if (++count < data.Length) Array.Resize(ref data, count);

            return data;
        }
    }
}
