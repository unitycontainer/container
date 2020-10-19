using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public static class DefaultsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? Get<T>(this Defaults defaults, Type? target) 
            where T : class => (T?)defaults.Get(target, typeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(this Defaults defaults, Type? target, T value) 
            where T : class => defaults.Set(target, typeof(T), value);
    }
}
