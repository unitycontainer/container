using System;
using System.Reflection;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericType(this Type type) => type.IsGenericType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType(this Type type) => type.IsValueType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericTypeDefinition(this Type type) => type.IsGenericTypeDefinition;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsGenericParameters(this Type type) => type.ContainsGenericParameters;
#endif
        #endregion


        #region Member Info

#if !NETCOREAPP1_0 && !NETSTANDARD1_6 && !NETSTANDARD1_0

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TInfo? GetMemberFromInfo<TInfo>(this TInfo info, Type type)
            where TInfo : MethodBase => (TInfo?)MethodBase.GetMethodFromHandle(info.MethodHandle, type.TypeHandle);
#endif

        #endregion

    }
}