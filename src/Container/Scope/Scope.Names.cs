using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        [CLSCompliant(false)]
        protected ref readonly NameInfo GetNameInfo(string name)
        {

            var hash = (uint)name!.GetHashCode();
            var target = hash % NamesMeta.Length;
            var position = NamesMeta[target].Position;
            
            // Check if already registered
            
            while (position > 0)
            {
                ref var candidate = ref NamesData[position];
                if (hash == candidate.Hash && candidate.Name == name)
                    return ref candidate;

                position = NamesMeta[position].Next;
            }

            // Nothing found, add new
            
            NamesCount += 1;

            // Expand if required
            if (NamesData.Length <= NamesCount)
            {
                Array.Resize(ref NamesData, Prime.Numbers[NamesPrime++]);
                var meta = new Metadata[Prime.Numbers[NamesPrime]];

                // Rebuild buckets
                for (var current = START_INDEX; current < NamesCount; current++)
                {
                    target = NamesData[current].Hash % meta.Length;
                    meta[current].Next = meta[target].Position;
                    meta[target].Position = current;
                }

                target = hash % meta.Length;
                NamesMeta = meta;
            }

            ref var entry = ref NamesData[NamesCount];
            ref var bucket = ref NamesMeta[target];

            entry = new NameInfo(hash, name);

            NamesMeta[NamesCount].Next = bucket.Position;
            bucket.Position = NamesCount;

            return ref entry;
        }

        [DebuggerDisplay("{ Name }")]
        [CLSCompliant(false)]
        public struct NameInfo
        {
            public readonly uint Hash;
            public readonly string? Name;
            public int[] References;

            public NameInfo(uint hash, string? name)
            {
                Hash = hash;
                Name = name;
                References = new int[5];
            }

            public void Resize(int required)
            {
                var requiredLength = required + References[0] + 1;

                // Expand references if required
                if (requiredLength >= References.Length)
                    Array.Resize(ref References, (int)(requiredLength * 1.45f));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Register(int position)
                => References[++References[0]] = position;
        }
    }
}
