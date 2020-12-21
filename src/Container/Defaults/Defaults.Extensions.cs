using System;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public static class DefaultsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<TPolicy>(this Defaults defaults, Type? target, out TPolicy? value)
            where TPolicy : class => null != (value = (TPolicy?)defaults.Get(target, typeof(TPolicy)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<TPolicy>(this Defaults defaults, out TPolicy? value)
            where TPolicy : class => null != (value = (TPolicy?)defaults.Get(typeof(TPolicy)));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this Defaults defaults)
            where TPolicy : class => (TPolicy?)defaults.Get(typeof(TPolicy));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TPolicy>(this Defaults defaults, Type? target)
            where TPolicy : class => (TPolicy?)defaults.Get(target, typeof(TPolicy));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TPolicy? Get<TTarget, TPolicy>(this Defaults defaults)
            where TPolicy : class => (TPolicy?)defaults.Get(typeof(TTarget), typeof(TPolicy));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this Defaults defaults, TPolicy value) 
            where TPolicy : class => defaults.Set(typeof(TPolicy), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TPolicy>(this Defaults defaults, Type? target, TPolicy value)
            where TPolicy : class => defaults.Set(target, typeof(TPolicy), value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<TTarget, TPolicy>(this Defaults defaults, TPolicy value)
            where TPolicy : class => defaults.Set(typeof(TTarget), typeof(TPolicy), value);
    }
}
