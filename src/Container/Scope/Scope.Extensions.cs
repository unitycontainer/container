using System.Runtime.CompilerServices;
using Unity.Storage;

namespace Unity.Container
{
    internal static class ScopeExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Setup(this Metadata[] meta, float factor) 
            => meta[0].Position = (int)(meta.Length * factor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BufferLength(this Metadata[] meta, int max)
            => meta[0].Position = max - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxIndex(this Metadata[] meta) 
            => meta[0].Position;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Capacity(this Metadata[] meta)
            => meta[0].Position + 1;
    }
}
