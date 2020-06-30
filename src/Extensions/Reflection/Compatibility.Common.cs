using System;
using System.Runtime.CompilerServices;

namespace Unity
{
    internal static class Compatibility_Common
    {
        #region Type

#if !NETCOREAPP1_0 && !NETSTANDARD1_6 && !NETSTANDARD1_0

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInterface(this Type type) => type.IsInterface;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClass(this Type type) => type.IsClass;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAbstract(this Type type) => type.IsAbstract;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrimitive(this Type type) => type.IsPrimitive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnum(this Type type) => type.IsEnum;

#endif
        #endregion
    }
}