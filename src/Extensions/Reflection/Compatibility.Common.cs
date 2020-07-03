using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace Unity
{
    [SecuritySafeCritical]
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


        #region Array

#if NET45 || NET46 || NET47 || NET48
        public static void Fill(this int[] array, int value)
        {
            unsafe
            {
                fixed (int* bucketsPtr = array)
                {
                    int* ptr = bucketsPtr;
                    var end = bucketsPtr + array.Length;
                    while (ptr < end) *ptr++ = value;
                }
            }
        }
#elif NETCOREAPP1_0 || NETSTANDARD1_6 || NETSTANDARD2_0
        public static void Fill(this int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill(this int[] array, int value) => Array.Fill(array, value);
#endif

        #endregion
    }
}