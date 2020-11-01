using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public static class DefaultsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>(this Defaults defaults, Type? target, out T? value)
            where T : class => null != (value = (T?)defaults.Get(target, typeof(T)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>(this Defaults defaults, out T? value)
            where T : class => null != (value = (T?)defaults.Get(typeof(T)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Get<T>(this Defaults defaults, Type? target) 
            where T : class => (T?)defaults.Get(target, typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this Defaults defaults, T value) 
            where T : class => defaults.Set(typeof(T), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this Defaults defaults, Type? target, T value)
            where T : class => defaults.Set(target, typeof(T), value);
    }
}
