using System;
using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope
    {
        private static Metadata[] ArrayToMeta(Scope scope, Type[] types)
        {
            var set = new MetaSet(1);
            set.Data[0].Location = scope.Version;

            var count = ((ContainerScope)scope).ToMetaSet(ref set, types, false);
            if (count < set.Data.Length) Array.Resize(ref set.Data, count);

            set.Data[0].Position = count;
            
            return set.Data;
        }

        private static Metadata[] EnumToMeta(Scope scope, Type[] types)
        {
            var set = new MetaSet(1);
            set.Data[0].Location = scope.Version;

            var count = ((ContainerScope)scope).ToMetaSet(ref set, types, true);
            if (count < set.Data.Length) Array.Resize(ref set.Data, count);

            set.Data[0].Position = count;

            return set.Data;
        }

        private int ToMetaSet(ref MetaSet set, Type[] types, bool @defaults)
        {
            Span<Metadata> span = stackalloc Metadata[Level + 2];
            var count = 0;
            bool named = true;

            foreach (var type in types)
            {
                var scope = this;
                var index = -1;
                Metadata location = default;

                // Get positions within the scope stack
                do
                {
                    location.Location = scope.IndexOf(type, type.GetHashCode());
                    if (0 < location.Location)
                    {
                        span[location.Position++] = new Metadata(scope.Level, location.Location);
                        if (0 > scope.Data[location.Location].Next) break;
                    }
                }
                while (null != (scope = (ContainerScope)scope.Next!));
                
                var stack = span.Slice(0, location.Position);
                location = default;
                scope = this;

                while (0 < stack.Length)
                { 
                    // Next 
                    next: location.Position = named ? scope[location.Position].Next
                                                    : scope.MoveNext(location.Position, type);
                    // Add
                    if (0 < location.Position) goto record;

                    // Level Up
                    if (++index < stack.Length)
                    {
                        location = stack[index];
                        scope = (ContainerScope)Ancestry[location.Location];
                        
                        if (named) location.Position = scope[location.Position].Next;
                        if (0 < location.Position) goto record;
                    }

                    // Switch to named
                    if (named && @defaults)
                    {
                        index = 0;
                        location = stack[index];
                        scope = (ContainerScope)Ancestry[location.Location];
                        named = false;

                        if (0 < location.Position) goto record;
                    }

                    break;

                    record:

                    ref var entry = ref scope[location.Position].Internal;
                    if (null == entry.Manager) continue;

                    var target = (int)(((uint)entry.Contract.HashCode) % set.Meta.Length);
                    if (null != entry.Contract.Name)
                    {
                        var position = set.Meta[target].Position;

                        while (position > 0)
                        {
                            ref var record = ref set.Data[position];
                            ref var contract = ref this[in record].Internal.Contract;

                            if (entry.Contract.HashCode == contract.HashCode && ReferenceEquals(contract.Type, entry.Contract.Type))
                                goto next;

                            position = set.Meta[position].Location;
                        }
                    }

                    if (set.Data.Length <= ++count)
                    {
                        var prime = Storage.Prime.NextUp(set.Meta.Length);
                        var buffer = new Metadata[Storage.Prime.Numbers[prime]];

                        for (var position = 1; position < count; position++)
                        {
                            ref var record = ref set.Data[position];
                            ref var local  = ref buffer[((uint)this[in record].HashCode) % buffer.Length];

                            buffer[position].Location = local.Position;
                            local.Position = position;
                        }

                        set.Data.CopyTo(set.Meta, 0);
                        set.Data = set.Meta;
                        set.Meta = buffer;

                        target = (int)(((uint)entry.Contract.HashCode) % buffer.Length);
                    }

                    // Add new registration
                    ref var bucket = ref set.Meta[target];
                    set.Data[count] = location;
                    set.Meta[count].Location = bucket.Position;
                    bucket.Position = count;
                }
            }

            return count + 1;
        }
    }
}
