using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Container
{
    public abstract partial class Scope
    {
        protected ref readonly NameInfo GetNameInfo(string name)
        {
            var hash = (uint)name!.GetHashCode();

            // Check if already registered
            var target = hash % _namesMeta.Length;
            var position = _namesMeta[target].Position;
            while (position > 0)
            {
                ref var candidate = ref _namesData[position];
                if (hash == candidate.Hash && candidate.Name == name)
                    return ref candidate;

                position = _namesMeta[position].Next;
            }

            // Expand if required
            if (_namesCount >= _namesMeta.MaxIndex())
            {
                var size = Prime.Numbers[++_namesPrime];

                _namesMeta = new Metadata[size];
                _namesMeta.Setup(LoadFactor);

                Array.Resize(ref _namesData, _namesMeta.Capacity());

                // Rebuild buckets
                for (var current = START_INDEX; current <= _namesCount; current++)
                {
                    target = _namesData[current].Hash % size;
                    _namesMeta[current].Next = _namesMeta[target].Position;
                    _namesMeta[target].Position = current;
                }

                target = hash % _namesMeta.Length;
            }

            ref var entry = ref _namesData[++_namesCount];

            entry = new NameInfo(hash, name);

            ref var bucket = ref _namesMeta[target];
            _namesMeta[_namesCount].Next = bucket.Position;
            bucket.Position = _namesCount;

            return ref entry;
        }


        [DebuggerDisplay("{ Name }")]
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
