using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    internal static class InternalExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateDelegate<T>(this MethodInfo method, Type type) where T : Delegate
            => (T)method.CreateDelegate(type);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateDelegate<T>(this MethodInfo method, Type type, object? target) where T : Delegate
            => (T)method.CreateDelegate(type, target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<BuilderContext> CreatePipeline(this MethodInfo method)
            => (ResolveDelegate<BuilderContext>)method.CreateDelegate(typeof(ResolveDelegate<BuilderContext>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<BuilderContext> CreatePipeline(this MethodInfo method, object? target)
            => (ResolveDelegate<BuilderContext>)method.CreateDelegate(typeof(ResolveDelegate<BuilderContext>), target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<BuilderContext> CreatePipeline(this MethodInfo method, Type type)
            => (ResolveDelegate<BuilderContext>)method.MakeGenericMethod(type)
                                                       .CreateDelegate(typeof(ResolveDelegate<BuilderContext>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<BuilderContext> CreatePipeline(this MethodInfo method, Type type, object? target)
            => (ResolveDelegate<BuilderContext>)method.MakeGenericMethod(type)
                                                       .CreateDelegate(typeof(ResolveDelegate<BuilderContext>), target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<BuilderContext> CreatePipeline(this MethodInfo method, Type element, Type service, object? target)
            => (ResolveDelegate<BuilderContext>)method.MakeGenericMethod(element, service)
                                                       .CreateDelegate(typeof(ResolveDelegate<BuilderContext>), target);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<TContext> CreatePipeline<TContext>(this MethodInfo method, Type type)
            where TContext : IBuilderContext => (ResolveDelegate<TContext>)method.MakeGenericMethod(type)
                                                                                 .CreateDelegate(typeof(ResolveDelegate<TContext>));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResolveDelegate<TContext> CreatePipeline<TContext>(this MethodInfo method, Type type, object? target)
            where TContext : IBuilderContext 
            => (ResolveDelegate<TContext>)method.MakeGenericMethod(type)
                                                .CreateDelegate(typeof(ResolveDelegate<TContext>), target);
    }
}
