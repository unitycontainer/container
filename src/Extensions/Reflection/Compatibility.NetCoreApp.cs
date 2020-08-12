using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity
{
    internal static class Compatibility_NetCoreApp_1_0
    {
        #region Type

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInterface(this Type type) => type.GetTypeInfo().IsInterface;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClass(this Type type) => type.GetTypeInfo().IsClass;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrimitive(this Type type) => type.GetTypeInfo().IsPrimitive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

        #endregion
    }

    internal static class Delegate
    {
        public static object CreateDelegate(Type type, object target, MethodInfo info) 
            => info.CreateDelegate(type, target);
    }
}
