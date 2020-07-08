using System;
using System.Runtime.CompilerServices;

namespace Unity
{
    internal static class HashCodeGeneration
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetHashCode(this Type type, uint hash) => (uint)type.GetHashCode() ^ hash;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetHashCode(this Type type, string name) => (uint)type.GetHashCode() ^ (uint)name.GetHashCode();
    }
}
